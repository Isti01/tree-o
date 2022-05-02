using System;
using System.Collections.Generic;
using Logic.Command;
using Logic.Data.World;
using Logic.Event;

namespace Logic.Data {

/// <summary>
/// Container of everything related to the game.
/// This class is responsible for integrating the different components of the game: the world, the teams.
/// It also implements a feature: the phase management.
/// </summary>
public interface IGameOverview {
	/// <summary>
	/// The common event dispatcher used everywhere in this game instance.
	/// </summary>
	public EventDispatcher Events { get; }

	/// <summary>
	/// The common command dispatcher used everywhere in this game instance.
	/// </summary>
	public CommandDispatcher Commands { get; }

	/// <summary>
	/// The world of this game instance.
	/// </summary>
	public GameWorld World { get; }

	/// <summary>
	/// The current phase of this game instance.
	/// </summary>
	/// <seealso cref="TimeLeftFromPhase"/>
	public GamePhase CurrentPhase { get; }

	/// <summary>
	/// The time until the team is updated "automatically".
	/// The value is never negative and may be infinite.
	/// </summary>
	public float TimeLeftFromPhase { get; }

	/// <summary>
	/// The random instance used as the randomness source in this game instance.
	/// This random should be used directly to get random values
	/// or it can be used to seed a new random instance.
	/// </summary>
	public Random Random { get; }

	/// <summary>
	/// The unique teams in this game instance.
	/// The count of elements is always exactly 2.
	/// </summary>
	public IEnumerable<GameTeam> Teams { get; }

	/// <summary>
	/// Container of configuration entries related to economy (money management) features.
	/// </summary>
	public IGameEconomyConfig EconomyConfig { get; }

	/// <summary>
	/// Gets the team whose <see cref="GameTeam.TeamColor"/> equals the specified value.
	/// </summary>
	/// <param name="color">the color whose associated team to get</param>
	/// <returns>the <see cref="GameTeam"/> associated with the specified color</returns>
	/// <exception cref="Exception">if the specified color is invalid</exception>
	public GameTeam GetTeam(Color color);

	/// <summary>
	/// Gets the <see cref="GameTeam"/> which isn't the specified team.
	/// This works because there are only 2 teams.
	/// </summary>
	/// <param name="team">the team whose enemy to get</param>
	/// <returns>the enemy of the specified team</returns>
	/// <exception cref="Exception">if the specified team is invalid</exception>
	public GameTeam GetEnemyTeam(GameTeam team);
}

}
