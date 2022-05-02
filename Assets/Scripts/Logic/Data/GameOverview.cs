using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Command;
using Logic.Data.World;
using Logic.Event;
using Logic.Handler;
using Logic.System;

namespace Logic.Data {

/// <summary>
/// Implementation of <see cref="IGameOverview"/>.
/// The interface was created to allow testing (mocking).
/// </summary>
public class GameOverview : IGameOverview {
	#region Fields

	private readonly IList<BaseSystem> _systems = new List<BaseSystem>();
	private readonly IList<BaseHandler> _handlers = new List<BaseHandler>();
	private readonly IGameOverviewConfig _config;
	private readonly GameTeam _redTeam;
	private readonly GameTeam _blueTeam;

	#endregion

	#region Properties

	public EventDispatcher Events { get; }

	public CommandDispatcher Commands { get; }

	public GameWorld World { get; }

	public GamePhase CurrentPhase { get; private set; } = GamePhase.Prepare;

	public float TimeLeftFromPhase { get; private set; } = float.PositiveInfinity;

	public Random Random { get; }

	public IEnumerable<GameTeam> Teams => new[] { _redTeam, _blueTeam };

	public IGameEconomyConfig EconomyConfig { get; }

	#endregion

	#region Methods

	/// <summary>
	/// Creates a new game instance.
	/// Also creates and initializes all aggregated game components (e.g. <see cref="GameWorld"/>).
	/// </summary>
	/// <param name="eventExceptionHandler">the callback that gets called when an unexpected
	/// error occurs in the game logic (this callback should log the error or close the app)</param>
	/// <param name="rngSeed">the seed to use for <see cref="Random"/></param>
	/// <param name="overviewConfig">container of configuration entries related to this class</param>
	/// <param name="economyConfig">the value for <see cref="EconomyConfig"/></param>
	/// <param name="worldConfig">the value for <see cref="GameWorld.Config"/></param>
	public GameOverview(Action<Exception> eventExceptionHandler, int rngSeed,
		IGameOverviewConfig overviewConfig, IGameEconomyConfig economyConfig, IGameWorldConfig worldConfig) {
		_config = overviewConfig;
		EconomyConfig = economyConfig;

		Events = new EventDispatcher((exceptions => {
			foreach (EventDispatcher.EventInvocationException exception in exceptions) {
				eventExceptionHandler.Invoke(exception);
			}
		}));
		RegisterSystems();

		Commands = new CommandDispatcher();
		RegisterHandlers();

		Random = new Random(rngSeed);

		World = new GameWorld(this, worldConfig);

		GameTeam CreateTeam(Color color) {
			return new GameTeam(this, color,
				World.GetTileObjectsOfType<Castle>().First(c => c.OwnerColor == color),
				World.GetTileObjectsOfType<Barrack>().Where(b => b.OwnerColor == color));
		}

		_redTeam = CreateTeam(Color.Red);
		_blueTeam = CreateTeam(Color.Blue);
	}

	public GameTeam GetTeam(Color color) {
		return color switch {
			Color.Red => _redTeam,
			Color.Blue => _blueTeam,
			_ => throw new Exception($"Unexpected color: {color}")
		};
	}

	public GameTeam GetEnemyTeam(GameTeam team) {
		if (team == _redTeam) return _blueTeam;
		if (team == _blueTeam) return _redTeam;
		throw new Exception($"Unexpected team: {team}");
	}

	/// <summary>
	/// Tries to update the value of <see cref="GamePhase"/> based on its current value.
	/// Also updates <see cref="TimeLeftFromPhase"/>.
	/// </summary>
	/// <exception cref="Exception">if advancing from the current phase is not possible</exception>
	internal void AdvancePhase() {
		GamePhase oldPhase = CurrentPhase;

		if (CurrentPhase == GamePhase.Prepare) {
			CurrentPhase = GamePhase.Fight;
			TimeLeftFromPhase = Math.Max(_config.FightingPhaseDuration,
				World.GetTileObjectsOfType<Barrack>()
					.Max(barrack =>
						barrack.QueuedUnits.Count * World.Config.BarrackSpawnCooldownTime
						+ barrack.Ordinal * World.Config.BarrackSpawnTimeOffset));
		} else if (CurrentPhase == GamePhase.Fight) {
			if (Teams.Any(t => t.Castle.IsDestroyed)) {
				CurrentPhase = GamePhase.Finished;
				TimeLeftFromPhase = float.PositiveInfinity;
			} else {
				CurrentPhase = GamePhase.Prepare;
				TimeLeftFromPhase = float.PositiveInfinity;
			}
		} else {
			throw new Exception($"Unexpected phase: {CurrentPhase}");
		}

		Events.Raise(new PhaseAdvancedEvent(this, oldPhase));
	}

	/// <summary>
	/// Decreases the value of <see cref="TimeLeftFromPhase"/>,
	/// calling <see cref="AdvancePhase"/> if it reaches 0.
	/// </summary>
	/// <param name="deltaTime">the amount of time that has passed</param>
	internal void DecreaseTimeLeftFromPhase(float deltaTime) {
		TimeLeftFromPhase -= deltaTime;
		if (TimeLeftFromPhase <= 0) {
			AdvancePhase();
		}
	}

	private void RegisterSystems() {
		_systems.Add(new DestroyUnitSystem());
		_systems.Add(new InvalidateCachesSystem());
		_systems.Add(new StartNextRoundSystem());
		_systems.Add(new TeamStatisticsSystem());
		_systems.Add(new TowerDamagesUnitSystem());
		_systems.Add(new UnitDamagesCastleSystem());
		foreach (BaseSystem system in _systems) system.RegisterListeners(Events);
	}

	private void RegisterHandlers() {
		_handlers.Add(new AdvancePhaseHandler());
		_handlers.Add(new AdvanceTimeHandler());
		_handlers.Add(new ManageBarrackHandler());
		_handlers.Add(new ManageTowerHandler());
		_handlers.Add(new ManageUnitHandler());
		foreach (BaseHandler handler in _handlers) handler.RegisterConsumers(Commands);
	}

	#endregion
}

}
