using System;
using Logic.Command;
using Logic.Data.World;
using Logic.Event;

namespace Logic.Data {
public class GameOverview {

	#region Properties

	public EventDispatcher Events { get; }

	public CommandDispatcher Commands { get; }

	public GameWorld World { get; }

	public GamePhase CurrentPhase { get; }

	public Random Random { get; }

	#endregion

	#region Methods

	public GameTeam GetTeam(Color color) {
		throw new NotImplementedException();
	}

	public void AdvancePhase() {
		throw new NotImplementedException();
	}

	public float TimeLeftFromPhase() {
		throw new NotImplementedException();
	}

	#endregion
}
}
