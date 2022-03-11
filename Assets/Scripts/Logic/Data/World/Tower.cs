using System;

namespace Logic.Data.World {
public class Tower : Building {

	#region Properties

	public ITowerTypeData Data { get; }

	public Unit Target { get; }

	public int Level { get; }

	public float RemainingCooldownTime { get; }

	#endregion

	#region Methods

	public void Upgrade() {
		throw new NotImplementedException();
	}
	public void UpdateTarget() {
		throw new NotImplementedException();
	}

	public void UpdateCooldown(float delta) {
		throw new NotImplementedException();
	}

	public void Shoot() {
		throw new NotImplementedException();
	}

	#endregion
}
}
