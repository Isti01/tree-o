namespace Logic.Data.World {
/// <summary>
/// Represents an obstacle of the world.
/// </summary>
public class Obstacle : TileObject {
	/// <summary>
	/// Creates an obstacle.
	/// </summary>
	/// <param name="world">The world in which it will be created.</param>
	/// <param name="position">The position of the obstacle.</param>
	internal Obstacle(GameWorld world, TilePosition position)
		: base(world, position) {}
}

}
