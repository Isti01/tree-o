namespace Logic.Data.World {
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

	/// <summary>
	/// Formats the object for printing on screen.
	/// </summary>
	/// <returns>Printable format of the TileObject.</returns>
	public override string ToString() {
		return $"{GetType().Name}@{Position}";
	}
}
}
