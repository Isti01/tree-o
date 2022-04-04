namespace Logic.Data.World {

public interface IGameWorldConfig {
	public float BarrackSpawnCooldownTime { get; }
	public float CastleStartingHealth { get; }

	public int MaxBuildingDistance { get; }
}

}
