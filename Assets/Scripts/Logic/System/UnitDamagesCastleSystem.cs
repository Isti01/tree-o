using Logic.Data.World;
using Logic.Event;
using Logic.Event.World.Unit;

namespace Logic.System {

/// <summary>
/// System responsible for dealing damage to a <see cref="Castle"/> when a <see cref="Unit"/> reaches it.
/// This system is also responsible for destroying the <see cref="Unit"/> when this happens.
/// </summary>
internal class UnitDamagesCastleSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		//Call later than usual: let listeners get notified about the damaged event
		// before the destroyed event is invoked.
		dispatcher.AddListener<UnitMovedTileEvent>(EventDispatcher.Ordering.Later, On);
	}

	private void On(UnitMovedTileEvent e) {
		Unit unit = e.Unit;
		Castle enemyCastle = unit.World.Overview.GetEnemyTeam(unit.Owner).Castle;

		if (unit.TilePosition.Equals(enemyCastle.Position)) {
			if (!enemyCastle.IsDestroyed) {
				enemyCastle.Damage(unit, unit.Type.Damage);
			}

			unit.DestroyWithoutDamage();
			unit.World.DestroyUnit(unit);
		}
	}
}

}
