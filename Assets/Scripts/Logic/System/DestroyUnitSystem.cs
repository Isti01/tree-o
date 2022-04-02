using Logic.Data;
using Logic.Event;
using Logic.Event.World.Unit;

namespace Logic.System {
public class DestroyUnitSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<UnitDamagedEvent>(On);
	}

	private void On(UnitDamagedEvent e) {
		IGameOverview overview = e.Unit.World.Overview;
		if (!e.Unit.IsAlive) e.Unit.World.DestroyUnit(e.Unit);
		overview.GetEnemyTeam(e.Unit.Owner).GiveMoney(overview.EconomyConfig.NewUnitsKilledPay);
	}
}
}
