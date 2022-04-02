using Logic.Data;
using UnityEngine;

namespace Presentation.World {

[CreateAssetMenu(fileName = "New Overview Config", menuName = "World/Config/Overview Config", order = 1)]
public class OverviewConfig : ScriptableObject, IGameOverviewConfig {
	public float fightingPhaseDuration;

	public float FightingPhaseDuration => fightingPhaseDuration;
}

}
