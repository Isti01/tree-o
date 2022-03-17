using System;
using System.Collections.Generic;
using Logic.Command;
using Logic.Data.World;
using Logic.Event;

namespace Logic.Data {

//Interface only has 1 implementation, but is required for mocking (testing).
public interface IGameOverview {
	EventDispatcher Events { get; }
	CommandDispatcher Commands { get; }
	GameWorld World { get; }
	GamePhase CurrentPhase { get; }
	float TimeLeftFromPhase { get; }
	Random Random { get; }
	public IEnumerable<GameTeam> Teams { get; }
	GameTeam GetTeam(Color color);
	GameTeam GetEnemyTeam(GameTeam team);
	void AdvancePhase();
	void DecreaseTimeLeftFromPhase(float deltaTime);
}

}
