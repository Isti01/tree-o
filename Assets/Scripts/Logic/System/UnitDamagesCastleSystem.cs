using Logic.Data.World;
using Logic.Event;
using Logic.Event.World.Unit;

namespace Logic.System {

public class UnitDamagesCastleSystem : BaseSystem {
	public override void RegisterListeners(EventDispatcher dispatcher) {
		dispatcher.AddListener<UnitMovedTileEvent>(On);
	}

	private void On(UnitMovedTileEvent e) {
		Unit unit = e.Unit;
		Castle enemyCastle = unit.World.Overview.GetEnemyTeam(unit.Owner).Castle;
		if (enemyCastle.Health <= 0) return;

		if (unit.TilePosition.Equals(enemyCastle.Position)) {
			enemyCastle.Damage(unit, unit.Type.Damage);
			unit.World.DestroyUnit(unit);
		}
	}
}

}
