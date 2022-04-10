using Logic.Data.World;
using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New World Config", menuName = "World/Config/World Config", order = 1)]
public class WorldConfig : ScriptableObject, IGameWorldConfig {
	[SerializeField]
	[Min(8)]
	private int width;

	[SerializeField]
	[Min(8)]
	private int height;

	[SerializeField]
	[Min(0.01f)]
	private float barrackSpawnCooldownTime;

	[SerializeField]
	[Min(0.01f)]
	private float castleStartingHealth;

	[SerializeField]
	[Min(0)]
	private int maxBuildingDistance;

	public int Width => width;
	public int Height => height;
	public float BarrackSpawnCooldownTime => barrackSpawnCooldownTime;
	public float CastleStartingHealth => castleStartingHealth;
	public int MaxBuildingDistance => maxBuildingDistance;
}
}
