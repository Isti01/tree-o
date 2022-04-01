using System.Linq;
using Logic.Event;
using Logic.Event.World.Unit;

namespace Logic.System {
public class DestroyUnitSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<UnitDamagedEvent>(On);
	}

	private void On(UnitDamagedEvent e) {

		if (!e.Unit.IsAlive) e.Unit.World.DestroyUnit(e.Unit);
		e.Unit.Owner.Overview.GetEnemyTeam(e.Unit.Owner).GiveMoney(50); //TODO: don't hardcode this

	}
}
}
