using Logic.Event;
using Logic.Event.World.Tower;

namespace Logic.System {
internal class TowerDamagesUnitSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<TowerShotEvent>(On);
	}

	private void On(TowerShotEvent e) {
		e.Target.InflictDamage(e.Tower,e.Tower.Type.Damage);
	}
}
}
