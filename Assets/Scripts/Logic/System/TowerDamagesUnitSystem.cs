using Logic.Event;
using Logic.Event.World.Tower;

namespace Logic.System {
internal class TowerDamagesUnitSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		//Call later than usual: let listeners get notified about the shot event
		// before the damaged event is invoked.
		dispatcher.AddListener<TowerShotEvent>(EventDispatcher.Ordering.Later, On);
	}

	private void On(TowerShotEvent e) {
		e.Target.InflictDamage(e.Tower,e.Tower.Type.Damage);
	}
}
}
