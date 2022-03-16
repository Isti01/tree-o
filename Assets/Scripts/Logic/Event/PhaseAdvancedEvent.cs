using Logic.Data;

namespace Logic.Event {

public class PhaseAdvancedEvent : BaseEvent {
	public GameOverview Overview { get; }
	public GamePhase OldPhase { get; }

	public PhaseAdvancedEvent(GameOverview overview, GamePhase oldPhase) {
		Overview = overview;
		OldPhase = oldPhase;
	}
}

}
