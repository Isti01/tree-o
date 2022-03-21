using System;

namespace Logic.Data.World {
public class Tower : Building {

	#region Properties

	public ITowerTypeData Type { get; }

	//TODO we need a CanBeNull annotation or some documentation here
	public Unit Target { get; }

	public int Level { get; }

	public float RemainingCooldownTime { get; private set; }

	public bool IsOnCooldown => RemainingCooldownTime > 0;

	#endregion

	#region Methods

	public Tower(GameWorld world, TilePosition position, Color owner, ITowerTypeData type)
		: base(world, position, owner) {
		Type = type;
	}

	public void Upgrade() {
		throw new NotImplementedException();
	}
	public void UpdateTarget() {
		// TODO implement UpdateTarget
	}

	public void UpdateCooldown(float delta) {
		throw new NotImplementedException();
	}

	public void ResetCooldown() {
		RemainingCooldownTime = 0;
	}

	public void Shoot() {
		throw new NotImplementedException();
	}

	#endregion
}
}
