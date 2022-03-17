using Logic.Data;
using Logic.Data.World;
using Logic.Event;

namespace Logic.System {

public class UpdateUnitPathSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<PhaseAdvancedEvent>(On);
	}

	private void On(PhaseAdvancedEvent e) {
		if (e.Overview.CurrentPhase != GamePhase.Fight) return;

		GameWorld world = e.Overview.World;
		foreach (Unit unit in world.Units) {
			unit.UpdatePlannedPath();
		}
	}
}

}
