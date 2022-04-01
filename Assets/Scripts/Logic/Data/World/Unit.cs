using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Event.World.Unit;

namespace Logic.Data.World {
public class Unit {
	#region Fields

	private readonly IList<TilePosition> _checkPoints;

	private List<Vector2> _cachedPath;

	#endregion

	#region Properties

	public GameTeam Owner { get; }

	public float CurrentHealth { get; private set; }

	public TilePosition NextCheckpoint => _checkPoints[0];

	public GameWorld World { get; }

	public Vector2 Position { get; private set; }

	public TilePosition TilePosition => Position.ToTilePosition();

	public IUnitTypeData Type { get; }

	public bool IsAlive => CurrentHealth > 0;

	#endregion

	#region Methods

	public Unit(IUnitTypeData type, GameTeam owner, GameWorld world, Vector2 position,
		IEnumerable<TilePosition> checkpoints) {
		Type = type;
		Owner = owner;
		CurrentHealth = Type.Health;
		World = world;
		Position = position;
		_checkPoints = new List<TilePosition>(checkpoints);
		_checkPoints.Add(Owner.Overview.GetEnemyTeam(Owner).Castle.Position);
	}

	public void Move(float delta) {
		TilePosition oldPosition = TilePosition;

		if (_checkPoints.Count > 1) throw new NotImplementedException();

		if (_cachedPath == null) {
			_cachedPath = World.Navigation.TryGetPathDeltas(Position, NextCheckpoint.ToVectorCentered(), 0);
		}

		float remainingDistance = Type.Speed * delta;
		while (_cachedPath.Any()) {
			Vector2 segment = _cachedPath[0];
			float length = segment.Length;
			if (remainingDistance > length) {
				remainingDistance -= length;
				Position = Position.Added(segment);
				_cachedPath.RemoveAt(0);
			} else {
				Vector2 doneSegment = segment.Multiplied(remainingDistance / length);
				Position = Position.Added(doneSegment);
				_cachedPath[0] = segment.Subtracted(doneSegment);
				break;
			}
		}

		if (!oldPosition.Equals(TilePosition)) {
			World.Overview.Events.Raise(new UnitMovedTileEvent(this, oldPosition));
		}
	}

	public void UpdatePlannedPath() {
		_cachedPath = null;

		if (_checkPoints.Count > 1) throw new NotImplementedException();
		//TODO delete unreachable checkpoints
	}

	public void InflictDamage(float damage) {
		CurrentHealth = Math.Max(CurrentHealth - damage, 0);
	}

	#endregion
}
}
