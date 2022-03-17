using Logic.Data;

namespace Logic.Event {

public class PhaseAdvancedEvent : BaseEvent {
	public IGameOverview Overview { get; }
	public GamePhase OldPhase { get; }

	public PhaseAdvancedEvent(IGameOverview overview, GamePhase oldPhase) {
		Overview = overview;
		OldPhase = oldPhase;
	}
}

}
