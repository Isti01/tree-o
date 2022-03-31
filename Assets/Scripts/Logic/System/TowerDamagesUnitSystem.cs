using System;
using Logic.Event;
using Logic.Event.World.Tower;

namespace Logic.System {
public class TowerDamagesUnitSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<TowerShotEvent>(On);
	}

	private void On(TowerShotEvent e) {
		e.Target.InflictDamage(e.Tower.Type.Damage);

		if (e.Target.CurrentHealth == 0) e.Target.World.DestroyUnit(e.Target);
	}
}
}
