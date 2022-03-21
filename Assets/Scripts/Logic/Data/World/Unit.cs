using System;
using System.Collections.Generic;
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

		if (_checkPoints.Count > 1) throw new NotImplementedException();

		List<Vector2> path = World.Navigation.TryGetPathDeltas(Position, NextCheckpoint.ToVectorCentered(), 0);
		float remainingDistance = Type.Speed * delta;
		foreach (Vector2 segment in path) {
			float length = segment.Length;
			if (remainingDistance > length) {
				remainingDistance -= length;
				Position = Position.Added(segment);
			} else {
				Position = Position.Added(segment.Multiplied(remainingDistance / length));
				break;
			}
		}

		if (!oldPosition.Equals(TilePosition)) {
			World.Overview.Events.Raise(new UnitMovedTileEvent(this, oldPosition));
		}
	}

	public void UpdatePlannedPath() {
		if (_checkPoints.Count > 1) throw new NotImplementedException();
		//TODO delete unreachable checkpoints
	}

	#endregion
}
}
