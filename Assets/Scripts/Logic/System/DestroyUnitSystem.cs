using Logic.Data;
using Logic.Data.World;
using Logic.Event;
using Logic.Event.World.Unit;

namespace Logic.System {

/// <summary>
/// System responsible for dealing with the destruction of <see cref="Unit"/> instances:
/// when <see cref="UnitDamagedEvent"/> is invoked and the <see cref="Unit"/> ends up getting killed,
/// this system is (indirectly) responsible for raising <see cref="UnitDestroyedEvent"/>
/// and executing relevant actions.
/// </summary>
internal class DestroyUnitSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		//Call later than usual: let listeners get notified about the damaged event
		// before the destroyed event is invoked.
		dispatcher.AddListener<UnitDamagedEvent>(EventDispatcher.Ordering.Later, On);
		dispatcher.AddListener<UnitDestroyedEvent>(On);
	}

	private void On(UnitDamagedEvent e) {
		if (!e.Unit.IsAlive) e.Unit.World.DestroyUnit(e.Unit);
	}

	private void On(UnitDestroyedEvent e) {
		IGameOverview overview = e.Unit.World.Overview;
		overview.GetEnemyTeam(e.Unit.Owner).GiveMoney(overview.EconomyConfig.NewUnitsKilledPay);
	}
}

}
