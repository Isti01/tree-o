using System;
using System.Collections.Generic;

namespace Logic.Data.World {
public class Unit {

	#region Fields

	private readonly IList<TilePosition> _checkPoints;

	#endregion

	#region Properties

	public GameTeam Owner { get; }

	public TilePosition NextCheckpoint => _checkPoints[0];

	public GameWorld World { get; }

	public Vector2 Position { get; }

	public IUnitTypeData Type { get; }

	#endregion

	#region Methods

	public Unit(IUnitTypeData type, GameTeam owner, GameWorld world, Vector2 position,
		IEnumerable<TilePosition> checkpoints) {
		Type = type;
		Owner = owner;
		World = world;
		Position = position;
		_checkPoints = new List<TilePosition>(checkpoints);
		_checkPoints.Add(Owner.Overview.GetEnemyTeam(Owner).Castle.Position);
	}

	public void Move(float delta) {
		throw new NotImplementedException();
	}

	public void SkipUnreachableCheckpoints() {
		throw new NotImplementedException();
	}


	#endregion

}
}
