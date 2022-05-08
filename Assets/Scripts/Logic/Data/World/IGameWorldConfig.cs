namespace Logic.Data.World {
/// <summary>
/// Container of configuration entries related to the <see cref="GameWorld"/> class.
/// </summary>
public interface IGameWorldConfig {
	/// <summary>
	/// The width of the world.
	/// </summary>
	public int Width { get; }

	/// <summary>
	/// The height of the world.
	/// </summary>
	public int Height { get; }

	/// <summary>
	/// The cooldown time of the barracks between two spawning.
	/// </summary>
	public float BarrackSpawnCooldownTime { get; }

	/// <summary>
	/// The starting health of the castles.
	/// </summary>
	public float CastleStartingHealth { get; }

	/// <summary>
	/// The maximum distance a tower can be built from the team's buildings.
	/// </summary>
	public int MaxBuildingDistance { get; }

	/// <summary>
	/// True if obstacles will be generated into the world.
	/// </summary>
	public bool GenerateObstacles { get; }

	/// <summary>
	/// Base offset of the barracks' spawning.
	/// </summary>
	public float BarrackSpawnTimeOffset { get; }
}
}
