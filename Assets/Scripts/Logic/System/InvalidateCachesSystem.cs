using Logic.Data;
using Logic.Data.World;
using Logic.Event;
using Logic.Event.World.Tower;

namespace Logic.System {

public class InvalidateCachesSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<PhaseAdvancedEvent>(OnPreparePhaseStarted);
		dispatcher.AddListener<PhaseAdvancedEvent>(OnFightPhaseStarted);
		dispatcher.AddListener<TowerBuiltEvent>(e => RecalculateAvailableTowerPositions(e.Tower.World.Overview));
		dispatcher.AddListener<TowerDestroyedEvent>(e => RecalculateAvailableTowerPositions(e.Tower.World.Overview));
	}

	private void OnPreparePhaseStarted(PhaseAdvancedEvent e) {
		if (e.Overview.CurrentPhase != GamePhase.Prepare) return;
		IGameOverview overview = e.Overview;

		RecalculateAvailableTowerPositions(overview);
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

	private void RecalculateAvailableTowerPositions(IGameOverview overview) {
		foreach (GameTeam team in overview.Teams) {
			team.InvalidateCachedAvailableTowerPositions();
		}
	}
}

}
