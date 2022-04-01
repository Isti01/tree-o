using Logic.Data.World;
using UnityEngine;

namespace Presentation.World {

[CreateAssetMenu(fileName = "New World Config", menuName = "World/Config/World Config", order = 1)]
public class WorldConfig : ScriptableObject, IGameWorldConfig {
	public float barrackSpawnCooldownTime;
	public float castleStartingHealth;

	public float BarrackSpawnCooldownTime => barrackSpawnCooldownTime;
	public float CastleStartingHealth => castleStartingHealth;
}

}
