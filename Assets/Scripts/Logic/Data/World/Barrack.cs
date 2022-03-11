using System;
using System.Collections.Generic;

namespace Logic.Data.World {
public class Barrack : Building {

	#region Fields

	private IList<TilePosition> _checkPoints;

	#endregion

	#region Properties

	public IReadOnlyCollection<TilePosition> CheckPoints { get; }

	public float SpawnCooldownTime { get; }

	public float RemainingCooldownTime { get; }

	public IReadOnlyCollection<IUnitTypeData> QueuedUnits { get; }

	#endregion

	#region Methods

	public void QueueUnit(IUnitTypeData unit) {
		throw new NotImplementedException();
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
