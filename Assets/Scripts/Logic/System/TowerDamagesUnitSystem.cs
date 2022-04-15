using Logic.Data.World;
using Logic.Event;
using Logic.Event.World.Tower;

namespace Logic.System {

/// <summary>
/// System responsible for damaging <see cref="Unit"/> instances when <see cref="TowerShotEvent"/> is called.
/// This separate class is necessary because, as outlined in the architecture document,
/// separate data classes mustn't modify each other directly (only systems can do that).
/// </summary>
internal class TowerDamagesUnitSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		//Call later than usual: let listeners get notified about the shot event
		// before the damaged event is invoked.
		dispatcher.AddListener<TowerShotEvent>(EventDispatcher.Ordering.Later, On);
	}

	private void On(TowerShotEvent e) {
		e.Target.InflictDamage(e.Tower, e.Tower.Type.Damage);
	}
}

}
