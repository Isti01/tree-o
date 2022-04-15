using Logic.Data;

namespace Logic.Event {

/// <summary>
/// Raised when the value of <see cref="IGameOverview.CurrentPhase"/> changes.
/// </summary>
public class PhaseAdvancedEvent : BaseEvent {
	public IGameOverview Overview { get; }
	public GamePhase OldPhase { get; }

	public PhaseAdvancedEvent(IGameOverview overview, GamePhase oldPhase) {
		Overview = overview;
		OldPhase = oldPhase;
	}
}

}
