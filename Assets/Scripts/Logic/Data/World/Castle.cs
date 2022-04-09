using System;
using Logic.Event.Team;
using Logic.Event.World.Castle;

namespace Logic.Data.World {
public class Castle : Building {
	#region Properties

	public float Health { get; private set; }

	public bool IsDestroyed => Health <= 0;

	#endregion

	#region Methods

	public Castle(GameWorld world, TilePosition position, Color owner)
		: base(world, position, owner) {
		Health = world.Config.CastleStartingHealth;
	}

	public void Damage(Unit attacker, float damage) {
		if (damage <= 0) throw new ArgumentException("Damage must be positive");
		if (IsDestroyed) throw new InvalidOperationException("Castle is already destroyed");

		Health -= damage;
		World.Overview.Events.Raise(new CastleDamagedEvent(this, attacker, damage));

		if (Health <= 0) {
			Health = 0;
			//I don't think we should remove the Castle from the GameWorld:
			// we can just replace the texture with different one.
			World.Overview.Events.Raise(new CastleDestroyedEvent(this));
		}

		World.Overview.Events.Raise(new TeamStatisticsUpdatedEvent(attacker.Owner));
		World.Overview.Events.Raise(new TeamStatisticsUpdatedEvent(Owner));
	}

	#endregion
}
}
