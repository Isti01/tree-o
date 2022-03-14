namespace Logic.Data.World {
public class Castle : Building {

	#region Properties

	public float Health { get; }

	#endregion

	#region Methods

	public Castle(GameWorld world, TilePosition position, Color owner)
		: base(world, position, owner) {}

	#endregion
}
}
