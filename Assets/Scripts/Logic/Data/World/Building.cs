namespace Logic.Data.World {

public abstract class Building : TileObject {
	public Color OwnerColor { get; }

	//Color is saved instead of GameTeam because in order to create a GameTeam
	// a Castle needed, so a Castle mustn't require a GameTeam instance.
	public GameTeam Owner => World.Overview.GetTeam(OwnerColor);

	private protected Building(GameWorld world, TilePosition position, Color owner)
		: base(world, position) {
		OwnerColor = owner;
	}
}

}
