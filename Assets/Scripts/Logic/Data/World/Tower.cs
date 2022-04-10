using System;
using System.Linq;
using Logic.Event.World.Tower;

namespace Logic.Data.World {
public class Tower : Building {
	#region Properties

	public ITowerTypeData Type { get; private set; }

	//TODO we need a CanBeNull annotation or some documentation here
	public Unit Target { get; private set; }

	public float RemainingCooldownTime { get; private set; }

	public bool IsOnCooldown => RemainingCooldownTime > 0;

	#endregion

	#region Methods

	internal Tower(GameWorld world, TilePosition position, Color owner, ITowerTypeData type)
		: base(world, position, owner) {
		Type = type;
	}

	internal void Upgrade() {
		if (Type.AfterUpgradeType == null) throw new InvalidOperationException($"{Type} is not upgradeable");
		ITowerTypeData oldType = Type;
		Type = Type.AfterUpgradeType;
		World.Overview.Events.Raise(new TowerUpgradedEvent(this, oldType));
	}

	internal void UpdateTarget() {
		if (Target != null && Position.ToVectorCentered().Distance(Target.Position) <= Type.Range
			&& Target.IsAlive)
			return;
		Unit oldTarget = Target;
		Target = null;
		foreach (Unit unit in World.Units.OrderBy(unit => Position.ToVectorCentered().Distance(unit.Position))) {
			if (unit.Owner != Owner) {
				Target = Position.ToVectorCentered().Distance(unit.Position) <= Type.Range ? unit : null;
				break;
			}
		}

		if (Target != oldTarget) World.Overview.Events.Raise(new TowerTargetChangedEvent(this, oldTarget));
	}

	internal void UpdateCooldown(float delta) {
		if (RemainingCooldownTime == 0) return;
		RemainingCooldownTime = Math.Max(RemainingCooldownTime - delta, 0);
		if (!IsOnCooldown) World.Overview.Events.Raise(new TowerCooledDownEvent(this));
	}

	internal void ResetCooldown() {
		RemainingCooldownTime = 0;
	}

	internal void Shoot() {
		if (Target == null) throw new InvalidOperationException("No target in tower range");
		if (IsOnCooldown) throw new InvalidOperationException($"Shooting is on cooldown: {RemainingCooldownTime}");
		RemainingCooldownTime = Type.CooldownTime;
		World.Overview.Events.Raise(new TowerShotEvent(this, Target));
	}

	#endregion
}
}
