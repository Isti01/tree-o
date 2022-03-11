using System;
using Logic.Data.World;

namespace Logic.Data {
public class GameOverview {

	#region Properties

	public GameWorld World { get; }

	public GamePhase CurrentPhase { get; }



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
