namespace Logic.Data.World {
/// <summary>
/// Abstract class of every building of the world
/// </summary>
public abstract class Building : TileObject {
	/// <summary>
	/// Color of the owner's team
	/// </summary>
	public Color OwnerColor { get; }

	//Color is saved instead of GameTeam because in order to create a GameTeam
	// a Castle needed, so a Castle mustn't require a GameTeam instance.
	/// <summary>
	/// Owner of the building.
	/// </summary>
	public GameTeam Owner => World.Overview.GetTeam(OwnerColor);

	private protected Building(GameWorld world, TilePosition position, Color owner)
		: base(world, position) {
		OwnerColor = owner;
	}
}
}
