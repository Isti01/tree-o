using System;
using Logic.Event.World.Castle;

namespace Logic.Data.World {
public class Castle : Building {
	#region Properties

	/// <summary>
	/// Remaining health of the castle.
	/// </summary>
	public float Health { get; private set; }

	/// <summary>
	/// Returns true if the castle is destroyed.
	/// </summary>
	public bool IsDestroyed => Health <= 0;

	#endregion

	#region Methods

	/// <summary>
	/// Creates a new castle.
	/// </summary>
	/// <param name="world">The <see cref="GameWorld"/> in which the castle will be created.</param>
	/// <param name="position">The <see cref="TilePosition"/> of the castle.</param>
	/// <param name="owner">The <see cref="Color"/> of the castle owner's team.</param>
	internal Castle(GameWorld world, TilePosition position, Color owner)
		: base(world, position, owner) {
		Health = world.Config.CastleStartingHealth;
	}

	/// <summary>
	/// Damages the castle. Raises an event if the castle gets destroyed.
	/// </summary>
	/// <param name="attacker">The unit that attacked the castle.</param>
	/// <param name="damage">The amount of damage inflicted.</param>
	/// <exception cref="ArgumentException">If the damage was negative.</exception>
	/// <exception cref="InvalidOperationException">If the castle is already destroyed.</exception>
	internal void Damage(Unit attacker, float damage) {
		if (damage < 0) throw new ArgumentException("Damage must not be negative");
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
