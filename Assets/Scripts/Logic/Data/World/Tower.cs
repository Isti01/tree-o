using System;
using System.Linq;
using Logic.Event.World.Tower;

namespace Logic.Data.World {
public class Tower : Building {
	#region Properties

	public ITowerTypeData Type { get; private set; }

	/// <summary>
	///     Stores a reference to the targeted unit. This getter might return null.
	/// </summary>
	public Unit Target { get; private set; }

	/// <summary>
	/// The closest enemy unit to the tower.
	/// </summary>
	public Unit ClosestEnemy {
		get {
			Unit closest = null;
			float closestDistance = float.PositiveInfinity;
			Vector2 turretPosition = Position.ToVectorCentered();
			foreach (Unit unit in World.Units) {
				if (unit.Owner == Owner) continue;
				float distance = unit.Position.Distance2(turretPosition);
				if (closestDistance > distance) {
					closestDistance = distance;
					closest = unit;
				}
			}

			return closest;
		}
	}

	/// <summary>
	/// Remaining cooldown time until the tower can shoot again.
	/// </summary>
	public float RemainingCooldownTime { get; private set; }

	/// <summary>
	/// False if cooldown is over.
	/// </summary>
	public bool IsOnCooldown => RemainingCooldownTime > 0;

	#endregion

	#region Methods

	/// <summary>
	/// Creates a tower.
	/// </summary>
	/// <param name="world">The <see cref="GameWorld"/> in which the tower will be created.</param>
	/// <param name="position">The <see cref="TilePosition"/> of the tower.</param>
	/// <param name="owner">The <see cref="Color"/> of the tower owner's team.</param>
	/// <param name="type">The <see cref="ITowerTypeData"/> of the tower.</param>
	internal Tower(GameWorld world, TilePosition position, Color owner, ITowerTypeData type)
		: base(world, position, owner) {
		Type = type;
	}

	/// <summary>
	/// Upgrades the tower to the next level.
	/// </summary>
	/// <exception cref="InvalidOperationException">If the tower is not upgradable.</exception>
	internal void Upgrade() {
		if (Type.AfterUpgradeType == null) throw new InvalidOperationException($"{Type} is not upgradeable");
		ITowerTypeData oldType = Type;
		Type = Type.AfterUpgradeType;
		World.Overview.Events.Raise(new TowerUpgradedEvent(this, oldType));
	}

	/// <summary>
	/// Finds a new target if the current target died or moved out of range.
	/// </summary>
	internal void UpdateTarget() {
		if (Target != null && Position.ToVectorCentered().Distance(Target.Position) <= Type.Range
			&& Target.IsAlive)
			return;
		Unit oldTarget = Target;
		Target = null;
		foreach (Unit unit in World.Units.OrderBy(unit => Position.ToVectorCentered().Distance(unit.Position)))
			if (unit.Owner != Owner) {
				Target = Position.ToVectorCentered().Distance(unit.Position) <= Type.Range ? unit : null;
				break;
			}

		if (Target != oldTarget) World.Overview.Events.Raise(new TowerTargetChangedEvent(this, oldTarget));
	}

	/// <summary>
	/// Updates the remaining cooldown time as time passes.
	/// </summary>
	/// <param name="delta">The amount of time passed.</param>
	internal void UpdateCooldown(float delta) {
		if (RemainingCooldownTime == 0) return;
		RemainingCooldownTime = Math.Max(RemainingCooldownTime - delta, 0);
		if (!IsOnCooldown) World.Overview.Events.Raise(new TowerCooledDownEvent(this));
	}

	/// <summary>
	/// Sets the remaining cooldown time for the start of a new round.
	/// </summary>
	internal void ResetCooldown() {
		RemainingCooldownTime = 0;
	}

	/// <summary>
	/// The tower at the target.
	/// </summary>
	/// <exception cref="InvalidOperationException">If the tower has no targets or is on cooldown.</exception>
	internal void Shoot() {
		if (Target == null) throw new InvalidOperationException("No target in tower range");
		if (IsOnCooldown) throw new InvalidOperationException($"Shooting is on cooldown: {RemainingCooldownTime}");
		RemainingCooldownTime = Type.CooldownTime;
		World.Overview.Events.Raise(new TowerShotEvent(this, Target));
	}

	#endregion
}
}
