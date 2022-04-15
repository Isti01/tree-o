namespace Logic.Event.World.Castle {

/// <summary>
/// Raised when a castle's <see cref="Data.World.Castle.Health"/> changes.
/// </summary>
public class CastleDamagedEvent : BaseEvent, ICastleEvent {
	public Data.World.Castle Castle { get; }
	public Data.World.Unit Attacker { get; }
	public float Damage { get; }

	public CastleDamagedEvent(Data.World.Castle castle, Data.World.Unit attacker, float damage) {
		Castle = castle;
		Attacker = attacker;
		Damage = damage;
	}
}

}
