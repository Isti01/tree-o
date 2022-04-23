namespace Logic.Data.World {
public interface IGameWorldConfig {
	public int Width { get; }
	public int Height { get; }
	public float BarrackSpawnCooldownTime { get; }
	public float CastleStartingHealth { get; }
	public int MaxBuildingDistance { get; }
	public bool GenerateObstacles { get; }
	public float BarrackSpawnOffset { get; }
}
}
