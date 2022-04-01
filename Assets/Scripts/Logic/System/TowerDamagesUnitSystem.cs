using System;
using Logic.Data.World;
using Logic.Event;
using Logic.Event.World.Tower;
using Logic.Event.World.Unit;

namespace Logic.System {
public class TowerDamagesUnitSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<TowerShotEvent>(On);
	}

	private void On(TowerShotEvent e) {
		e.Target.InflictDamage(e.Tower,e.Tower.Type.Damage);
	}
}
}
