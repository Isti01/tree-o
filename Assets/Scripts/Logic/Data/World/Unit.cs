using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Event.World.Unit;

namespace Logic.Data.World {
public class Unit {
	#region Fields

	private readonly IList<TilePosition> _checkPoints;

	#endregion

	#region Properties

	public GameTeam Owner { get; }

	public TilePosition NextCheckpoint => _checkPoints[0];

	public GameWorld World { get; }

	public Vector2 Position { get; private set; }

	public TilePosition TilePosition => Position.ToTilePosition();

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
		TilePosition oldPosition = TilePosition;
		List<Vector2> path = World.Navigation.TryGetPathDeltas(Position, NextCheckpoint.ToVectorCentered(), 0);

		float dist = Type.Speed * delta;
		float nextlength = path.First().Length;
		while (dist > nextlength && path.Count() > 0) {
			Position = Position.Added(path.First());
			path.RemoveAt(0);

			dist -= nextlength;
			nextlength = path.First().Length;
		}

		Position = Position.Added(path.First().Multiplied(dist / nextlength));
		if (!oldPosition.Equals(TilePosition)) {
			World.Overview.Events.Raise(new UnitMovedTileEvent(this, oldPosition));
		}
	}

	public void UpdatePlannedPath() {
		//TODO invalidate cached navigation data, if such a cache exists
		throw new NotImplementedException();
	}

	#endregion
}
}
