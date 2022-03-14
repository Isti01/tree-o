using System;
using System.Collections.Generic;
using Logic.Event.World.Unit;

namespace Logic.Data.World {
public class Barrack : Building {

	#region Fields

	private IList<TilePosition> _checkPoints;

	private readonly IList<IUnitTypeData> _queuedUnits = new List<IUnitTypeData>();

	#endregion

	#region Properties

	public IReadOnlyCollection<TilePosition> CheckPoints { get; }

	public float SpawnCooldownTime { get; }

	public float RemainingCooldownTime { get; }

	public IReadOnlyCollection<IUnitTypeData> QueuedUnits => new List<IUnitTypeData>(_queuedUnits);

	#endregion

	#region Methods

	public Barrack(GameWorld world, TilePosition position, Color owner)
		: base(world, position, owner) {}

	public void QueueUnit(IUnitTypeData type) {
		_queuedUnits.Add(type);
		World.Overview.Events.Raise(new UnitQueuedEvent(type, this));
	}
	public void PushCheckPoint(TilePosition tile) {
		throw new NotImplementedException();
	}

	public void DeleteCheckPoint(TilePosition tile) {
		throw new NotImplementedException();
	}
	public void spawn() {
		throw new NotImplementedException();
	}

	#endregion
}
}
