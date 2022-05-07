namespace Logic.Data.World {
/// <summary>
/// Base class of every Building and obstacle of the world.
/// </summary>
public abstract class TileObject {
	/// <summary>
	/// The world in which the TileObject is located.
	/// </summary>
	public GameWorld World { get; }

	/// <summary>
	/// The position of the TileObject.
	/// </summary>
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
