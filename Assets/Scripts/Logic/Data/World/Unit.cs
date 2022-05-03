using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Event.World.Unit;

namespace Logic.Data.World {

public class Unit {
	#region Fields

	private readonly IList<TilePosition> _checkPoints;

	private CachedPath _cachedCheckpointPath;

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

	internal Unit(IUnitTypeData type, GameTeam owner, GameWorld world, Vector2 position,
		IEnumerable<TilePosition> checkpoints) {
		Type = type;
		Owner = owner;
		CurrentHealth = Type.Health;
		World = world;
		Position = position;
		_checkPoints = new List<TilePosition>(checkpoints);
		_checkPoints.Add(Owner.Overview.GetEnemyTeam(Owner).Castle.Position);
	}

	internal void Move(float delta) {
		TilePosition oldPosition = TilePosition;

		float remainingDistance = Type.Speed * delta;
		while (remainingDistance > 0 && _checkPoints.Any()
			&& MoveTowardsNextCheckpoint(ref remainingDistance)) {
			_checkPoints.RemoveAt(0);
		}

		if (!oldPosition.Equals(TilePosition)) {
			World.Overview.Events.Raise(new UnitMovedTileEvent(this, oldPosition));
		}
	}

	private bool MoveTowardsNextCheckpoint(ref float remainingDistance) {
		if (_cachedCheckpointPath == null || !_cachedCheckpointPath.Checkpoint.Equals(NextCheckpoint)) {
			_cachedCheckpointPath = new CachedPath(NextCheckpoint,
				World.Navigation.TryGetPathDeltas(Position, NextCheckpoint.ToVectorCentered(), 0));
		}

		while (_cachedCheckpointPath.Path.Any()) {
			Vector2 segment = _cachedCheckpointPath.Path[0];
			float length = segment.Length;
			if (remainingDistance > length) {
				remainingDistance -= length;
				Position = Position.Added(segment);
				_cachedCheckpointPath.Path.RemoveAt(0);
			} else {
				Vector2 doneSegment = segment.Multiplied(remainingDistance / length);
				Position = Position.Added(doneSegment);
				_cachedCheckpointPath.Path[0] = segment.Subtracted(doneSegment);
				return false; //Checkpoint not reached
			}
		}

		return true; //Checkpoint reached
	}

	internal void UpdatePlannedPath() {
		_cachedCheckpointPath = null;

		HashSet<TilePosition> checkpointsToCheck = new HashSet<TilePosition>(_checkPoints);
		checkpointsToCheck.RemoveWhere(pos => World[pos] != null);
		ISet<TilePosition> reachableCheckpoints = World.Navigation.GetReachablePositionSubset(TilePosition,
			checkpointsToCheck);

		//No need to check the last checkpoint: it is the enemy castle
		for (int i = 0; i < _checkPoints.Count - 1; i++) {
			if (reachableCheckpoints.Contains(_checkPoints[i])) continue;
			_checkPoints.RemoveAt(i);
			i--;
		}
	}

	internal void InflictDamage(Tower attacker, float damage) {
		if (damage >= CurrentHealth) {
			damage = CurrentHealth;
			CurrentHealth = 0;
		} else {
			CurrentHealth -= damage;
		}
		World.Overview.Events.Raise(new UnitDamagedEvent(this, damage, attacker));
	}

	internal void DestroyWithoutDamage() {
		CurrentHealth = 0;
	}

	#endregion

	private class CachedPath {
		public TilePosition Checkpoint { get; }
		public IList<Vector2> Path { get; }

		public CachedPath(TilePosition checkpoint, IList<Vector2> path) {
			Checkpoint = checkpoint;
			Path = path;
		}
	}
}

}
