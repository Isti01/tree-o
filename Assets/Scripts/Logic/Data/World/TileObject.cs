namespace Logic.Data.World {

public abstract class TileObject {
	public GameWorld World { get; }
	public TilePosition Position { get; }

	private protected TileObject(GameWorld world, TilePosition position) {
		World = world;
		Position = position;
	}

	public override string ToString() {
		return $"{GetType().Name}@{Position}";
	}
}

}
