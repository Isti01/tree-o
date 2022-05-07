using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Event.World.Barrack;
using Logic.Event.World.Unit;

namespace Logic.Data.World {
/// <summary>
/// Represents a barrack in the game.
/// </summary>
public class Barrack : Building {
	#region Fields

	private readonly IList<TilePosition> _checkPoints = new List<TilePosition>();

	private readonly IList<IUnitTypeData> _queuedUnits = new List<IUnitTypeData>();

	#endregion

	#region Properties

	/// <summary>
	/// The checkpoints for the units that spawn from this barrack.
	/// </summary>
	public IReadOnlyCollection<TilePosition> CheckPoints => new List<TilePosition>(_checkPoints);

	/// <summary>
	/// The time remaining until the barrack can spawn a unit.
	/// </summary>
	public float RemainingCooldownTime { get; private set; }

	/// <summary>
	///Returns false if the barrack cooled down and can spawn a unit.
	/// </summary>
	public bool IsOnCooldown => RemainingCooldownTime > 0;

	/// <summary>
	/// List of units that will be spawned from this barrack.
	/// </summary>
	public IReadOnlyCollection<IUnitTypeData> QueuedUnits => new List<IUnitTypeData>(_queuedUnits);

	/// <summary>
	/// A unique number for each barrack from the same team.
	/// </summary>
	public int Ordinal { get; }

	#endregion

	#region Methods

	/// <summary>
	/// Creates a new barrack.
	/// </summary>
	/// <param name="world">The <see cref="GameWorld"/> in which the barrack will be created.</param>
	/// <param name="position">The <see cref="TilePosition"/> of the barrack.</param>
	/// <param name="owner">The <see cref="Color"/> of the barrack owner's team.</param>
	/// <param name="ordinal">A unique number for each barrack from the same team.</param>
	internal Barrack(GameWorld world, TilePosition position, Color owner, int ordinal)
		: base(world, position, owner) {
		Ordinal = ordinal;
	}

	/// <summary>
	/// Adds a unit's type data to the end of the barrack's spawn queue. These units will be spawned in Fight phase.
	/// </summary>
	/// <param name="type">The type of the unit.</param>
	internal void QueueUnit(IUnitTypeData type) {
		_queuedUnits.Add(type);
		World.Overview.Events.Raise(new UnitQueuedEvent(type, this));
	}

	/// <summary>
	/// Puts a new checkpoint to the end of the checkpoints.
	/// </summary>
	/// <param name="tile">The position of the new checkpoint.</param>
	/// <exception cref="ArgumentException">If the checkpoint is already in the list.</exception>
	internal void PushCheckPoint(TilePosition tile) {
		if (_checkPoints.Contains(tile)) {
			throw new ArgumentException("Position is already a checkpoint");
		}

		_checkPoints.Add(tile);
		World.Overview.Events.Raise(new BarrackCheckpointCreatedEvent(this, tile));
	}

	/// <summary>
	/// Removes a checkpoint from the checkpoints.
	/// </summary>
	/// <param name="tile">The position of the checkpoint.</param>
	/// <exception cref="ArgumentException">If the position is not a checkpoint.</exception>
	internal void DeleteCheckPoint(TilePosition tile) {
		if (!_checkPoints.Remove(tile)) {
			throw new ArgumentException("Position is not a checkpoint");
		}

		World.Overview.Events.Raise(new BarrackCheckpointRemovedEvent(this, tile));
	}

	/// <summary>
	/// Updates the remaining cooldown time as time passes.
	/// </summary>
	/// <param name="delta">The amount of time passed.</param>
	internal void UpdateCooldown(float delta) {
		RemainingCooldownTime -= delta;
		if (RemainingCooldownTime < 0) {
			RemainingCooldownTime = 0;
		}
	}

	/// <summary>
	/// Sets the remaining cooldown time for the start of a new round.
	/// </summary>
	internal void ResetCooldown() {
		RemainingCooldownTime = Ordinal * World.Config.BarrackSpawnTimeOffset;
	}

	/// <summary>
	/// Spawns the next queued unit.
	/// </summary>
	/// <exception cref="InvalidOperationException">If there are no units queued or the barrack is on cooldown.</exception>
	internal void Spawn() {
		if (!QueuedUnits.Any()) throw new InvalidOperationException("No queued units exist; nothing to spawn");

		if (IsOnCooldown) throw new InvalidOperationException($"Spawning is on cooldown: {RemainingCooldownTime}");

		RemainingCooldownTime = World.Config.BarrackSpawnCooldownTime;
		IUnitTypeData type = _queuedUnits[0];
		_queuedUnits.RemoveAt(0);
		World.DeployUnit(this, type);
	}

	/// <summary>
	/// Removes checkpoints from the list that became unreachable due to new buildings.
	/// </summary>
	internal void DeleteUnreachableCheckpoints() {
		HashSet<TilePosition> oldCheckpoints = new HashSet<TilePosition>(_checkPoints);
		oldCheckpoints.RemoveWhere(pos => World[pos] != null);
		ISet<TilePosition> reachableCheckpoints = World.Navigation.GetReachablePositionSubset(Position,
			oldCheckpoints);
		TilePosition enemyCastle = World.Overview.GetEnemyTeam(Owner).Castle.Position;
		reachableCheckpoints = World.Navigation.GetReachablePositionSubset(enemyCastle, reachableCheckpoints);

		for (int i = 0; i < _checkPoints.Count; i++) {
			if (reachableCheckpoints.Contains(_checkPoints[i])) continue;
			_checkPoints.RemoveAt(i);
			i--;
		}

		oldCheckpoints.ExceptWith(reachableCheckpoints);
		foreach (TilePosition unreachable in oldCheckpoints) {
			World.Overview.Events.Raise(new BarrackCheckpointRemovedEvent(this, unreachable));
		}
	}

	#endregion
}
}
