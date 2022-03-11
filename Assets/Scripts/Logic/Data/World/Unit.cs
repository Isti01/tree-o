using System;
using System.Collections.Generic;

namespace Logic.Data.World {
public class Unit {

	#region Fields

	private IList<TilePosition> _checkPoints;

	#endregion

	#region Properties

	public GameTeam Owner { get; }

	public TilePosition NextCheckpoint { get; }

	public Vector2 Position { get; }

	public IUnitTypeData Data { get; }

	#endregion

	#region Methods

	public void Move(float delta) {
		throw new NotImplementedException();
	}

	public void SkipUnreachableCheckpoints() {
		throw new NotImplementedException();
	}


	#endregion

}
}
