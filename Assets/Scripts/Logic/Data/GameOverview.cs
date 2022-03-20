using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Command;
using Logic.Data.World;
using Logic.Event;
using Logic.Handler;
using Logic.System;

namespace Logic.Data {

public class GameOverview : IGameOverview {
	#region Fields

	private readonly IList<BaseSystem> _systems = new List<BaseSystem>();
	private readonly IList<BaseHandler> _handlers = new List<BaseHandler>();
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

	#endregion

	#region Methods

	public GameOverview(Action<Exception> eventExceptionHandler, int seed,
		int worldWidth, int worldHeight) {
		Events = new EventDispatcher((exceptions => {
			foreach (EventDispatcher.EventInvocationException exception in exceptions) {
				eventExceptionHandler.Invoke(exception);
			}
		}));
		RegisterSystems();

		Commands = new CommandDispatcher();
		RegisterHandlers();

		Random = new Random(seed);

		World = new GameWorld(this, worldWidth, worldHeight);

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
		if (team == _redTeam) {
			return _blueTeam;
		} else if (team == _blueTeam) {
			return _redTeam;
		} else {
			throw new Exception($"Unexpected team: {team}");
		}
	}

	public void AdvancePhase() {
		GamePhase oldPhase = CurrentPhase;

		if (CurrentPhase == GamePhase.Prepare) {
			CurrentPhase = GamePhase.Fight;
			TimeLeftFromPhase = 100; //TODO don't hardcode this value
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

	public void DecreaseTimeLeftFromPhase(float deltaTime) {
		TimeLeftFromPhase -= deltaTime;
		if (TimeLeftFromPhase <= 0) {
			AdvancePhase();
		}
	}

	private void RegisterSystems() {
		_systems.Add(new UnitDamagesCastleSystem());
		_systems.Add(new InvalidateCachesSystem());
		foreach (BaseSystem system in _systems) system.RegisterListeners(Events);
	}

	private void RegisterHandlers() {
		_handlers.Add(new AdvancePhaseHandler());
		_handlers.Add(new AdvanceTimeHandler());
		_handlers.Add(new ManageTowerHandler());
		_handlers.Add(new ManageUnitHandler());
		foreach (BaseHandler handler in _handlers) handler.RegisterConsumers(Commands);
	}

	#endregion
}

}
