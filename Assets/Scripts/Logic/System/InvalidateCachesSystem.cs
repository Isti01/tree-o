using Logic.Data;
using Logic.Data.World;
using Logic.Event;
using Logic.Event.World.Tower;
using Logic.Event.World.Unit;

namespace Logic.System {
/// <summary>
/// System responsible for clearing/recalculating cached values. These include:
/// <list type="bullet">
/// <item>Positions where towers can be placed need to be updated after the <see cref="GamePhase.Fight"/>
/// phase (because <see cref="Unit"/> instances might be occupying them)
/// and whenever a <see cref="Tower"/> is built/destroyed.</item>
/// <item>Updating the planned paths of <see cref="Unit"/> instances when the <see cref="GamePhase.Fight"/>
/// phase starts, because towers might have been built/destroyed since the last path planning.</item>
/// <item>Resetting <see cref="Tower.RemainingCooldownTime"/> and
/// <see cref="Barrack.RemainingCooldownTime"/> when the  <see cref="GamePhase.Fight"/> phase starts.</item>
/// <item>Unreachable entries in <see cref="Barrack.CheckPoints"/> need to be removed
/// whenever a <see cref="Tower"/> is built.</item>
/// <item>Upon the destruction of a <see cref="Unit"/> the values of <see cref="Tower.Target"/>
/// need to be sanitized: destroyed <see cref="Unit"/> instances mustn't be targeted.</item>
/// </list>
/// </summary>
internal class InvalidateCachesSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<PhaseAdvancedEvent>(OnPreparePhaseStarted);
		dispatcher.AddListener<PhaseAdvancedEvent>(OnFightPhaseStarted);
		dispatcher.AddListener<TowerBuiltEvent>(OnTowerBuilt);
		dispatcher.AddListener<TowerDestroyedEvent>(OnTowerDestroyed);
		dispatcher.AddListener<UnitDestroyedEvent>(OnUnitDestroyed);
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

	private void OnTowerBuilt(TowerBuiltEvent e) {
		foreach (Barrack barrack in e.Tower.World.GetTileObjectsOfType<Barrack>()) {
			barrack.DeleteUnreachableCheckpoints();
		}

		RecalculateAvailableTowerPositions(e.Tower.World.Overview);
	}

	private void OnTowerDestroyed(TowerDestroyedEvent e) {
		RecalculateAvailableTowerPositions(e.Tower.World.Overview);
	}

	private void RecalculateAvailableTowerPositions(IGameOverview overview) {
		foreach (GameTeam team in overview.Teams) {
			team.InvalidateCachedAvailableTowerPositions();
		}
	}

	private void OnUnitDestroyed(UnitDestroyedEvent e) {
		foreach (Tower tower in e.Unit.World.GetTileObjectsOfType<Tower>()) {
			if (tower.Target == e.Unit) tower.UpdateTarget();
		}
	}
}
}
