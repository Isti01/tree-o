namespace Logic.Data {

/// <summary>
/// Represents the current state the of the game.
/// </summary>
public enum GamePhase {
	/// <summary>
	/// The simulation is paused, teams can build towers and purchase units.
	/// The next phase is always <see cref="Fight"/>.
	/// </summary>
	Prepare,

	/// <summary>
	/// A simulation is running: units are advancing, towers are shooting.
	/// The next phase can either be <see cref="Prepare"/> or <see cref="Finished"/>
	/// depending on whether any castle got destroyed.
	/// </summary>
	Fight,

	/// <summary>
	/// The game is over: one of the teams won (or they tied).
	/// No (modifying) actions are possible.
	/// The only way to "move forward" is to create a new game.
	/// No next phase exists: advancing the phase isn't possible.
	/// </summary>
	Finished
}

}
