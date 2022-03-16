﻿using System;

namespace Logic.Data.World {
public class Tower : Building {

	#region Properties

	public ITowerTypeData Data { get; }

	//TODO we need a CanBeNull annotation or some documentation here
	public Unit Target { get; }

	public int Level { get; }

	public float RemainingCooldownTime { get; }

	public bool IsOnCooldown => RemainingCooldownTime > 0;

	#endregion

	#region Methods

	public Tower(GameWorld world, TilePosition position, Color owner, ITowerTypeData data)
		: base(world, position, owner) {
		Data = data;
	}

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
