using System;
using System.Collections.Generic;
using System.Linq;
using Logic.Event.World.Barrack;
using Logic.Event.World.Unit;

namespace Logic.Data.World {
public class Barrack : Building {
	#region Fields

	private readonly IList<TilePosition> _checkPoints = new List<TilePosition>();

	private readonly IList<IUnitTypeData> _queuedUnits = new List<IUnitTypeData>();

	#endregion

	#region Properties

	public IReadOnlyCollection<TilePosition> CheckPoints => new List<TilePosition>(_checkPoints);

	public float RemainingCooldownTime { get; private set; }

	public bool IsOnCooldown => RemainingCooldownTime > 0;

	public IReadOnlyCollection<IUnitTypeData> QueuedUnits => new List<IUnitTypeData>(_queuedUnits);

	#endregion

	#region Methods

	internal Barrack(GameWorld world, TilePosition position, Color owner)
		: base(world, position, owner) {}

	internal void QueueUnit(IUnitTypeData type) {
		_queuedUnits.Add(type);
		World.Overview.Events.Raise(new UnitQueuedEvent(type, this));
	}

	internal void PushCheckPoint(TilePosition tile) {
		if (_checkPoints.Contains(tile)) {
			throw new ArgumentException("Position is already a checkpoint");
		}

		_checkPoints.Add(tile);
		World.Overview.Events.Raise(new BarrackCheckpointCreatedEvent(this, tile));
	}

	internal void DeleteCheckPoint(TilePosition tile) {
		if (!_checkPoints.Remove(tile)) {
			throw new ArgumentException("Position is not a checkpoint");
		}

		World.Overview.Events.Raise(new BarrackCheckpointRemovedEvent(this, tile));
	}

	internal void UpdateCooldown(float delta) {
		RemainingCooldownTime -= delta;
		if (RemainingCooldownTime < 0) {
			RemainingCooldownTime = 0;
		}
	}

	internal void ResetCooldown() {
		RemainingCooldownTime = 0;
	}

	internal void Spawn() {
		if (!QueuedUnits.Any()) throw new InvalidOperationException($"No queued units exist; nothing to spawn");

		if (IsOnCooldown) throw new InvalidOperationException($"Spawning is on cooldown: {RemainingCooldownTime}");

		RemainingCooldownTime = World.Config.BarrackSpawnCooldownTime;
		IUnitTypeData type = _queuedUnits[0];
		_queuedUnits.RemoveAt(0);
		World.DeployUnit(this, type);
	}

	#endregion
}
}
