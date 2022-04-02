using Logic.Data.World;
using UnityEngine;

namespace Presentation.World {

[CreateAssetMenu(fileName = "New World Config", menuName = "World/Config/World Config", order = 1)]
public class WorldConfig : ScriptableObject, IGameWorldConfig {
	[SerializeField]
	[Min(0.01f)]
	private float barrackSpawnCooldownTime;

	[SerializeField]
	[Min(0.01f)]
	private float castleStartingHealth;

	public float BarrackSpawnCooldownTime => barrackSpawnCooldownTime;
	public float CastleStartingHealth => castleStartingHealth;
}

}
