using System;
using Logic.Event.World.Castle;

namespace Logic.Data.World {
public class Castle : Building {
	#region Properties

	public float Health { get; private set; }

	public bool IsDestroyed => Health <= 0;

	#endregion

	#region Methods

	internal Castle(GameWorld world, TilePosition position, Color owner)
		: base(world, position, owner) {
		Health = world.Config.CastleStartingHealth;
	}

	internal void Damage(Unit attacker, float damage) {
		if (damage <= 0) throw new ArgumentException("Damage must be positive");
		if (IsDestroyed) throw new InvalidOperationException("Castle is already destroyed");

		Health = Math.Max(Health - damage, 0);
		World.Overview.Events.Raise(new CastleDamagedEvent(this, attacker, damage));

		if (IsDestroyed) {
			//I don't think we should remove the Castle from the GameWorld:
			// we can just replace the texture with different one.
			World.Overview.Events.Raise(new CastleDestroyedEvent(this));
		}
	}

	#endregion
}
}
