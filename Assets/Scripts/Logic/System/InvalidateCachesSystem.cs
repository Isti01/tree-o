using Logic.Data;
using Logic.Data.World;
using Logic.Event;

namespace Logic.System {

public class InvalidateCachesSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<PhaseAdvancedEvent>(OnFightPhaseStarted);
	}

	private void OnFightPhaseStarted(PhaseAdvancedEvent e) {
		if (e.Overview.CurrentPhase != GamePhase.Fight) return;
		GameWorld world = e.Overview.World;

		foreach (Unit unit in world.Units) {
			unit.UpdatePlannedPath();
		}

		foreach (Tower tower in world.GetTileObjectsOfType<Tower>()) {
			tower.ResetCooldown();
		}

		foreach (Barrack barrack in world.GetTileObjectsOfType<Barrack>()) {
			barrack.ResetCooldown();
		}
	}
}

}
