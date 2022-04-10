using Logic.Data;
using Logic.Event;
using Logic.Event.World.Unit;

namespace Logic.System {
internal class DestroyUnitSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		//Call later than usual: let listeners get notified about the damaged event
		// before the destroyed event is invoked.
		dispatcher.AddListener<UnitDamagedEvent>(EventDispatcher.Ordering.Later, On);
	}

	private void On(UnitDamagedEvent e) {
		IGameOverview overview = e.Unit.World.Overview;
		if (!e.Unit.IsAlive) e.Unit.World.DestroyUnit(e.Unit);
		overview.GetEnemyTeam(e.Unit.Owner).GiveMoney(overview.EconomyConfig.NewUnitsKilledPay);
	}
}
}
