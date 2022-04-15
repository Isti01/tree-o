using Logic.Data;
using UnityEngine;

namespace Presentation.World.Config {
[CreateAssetMenu(fileName = "New Overview Config", menuName = "World/Config/Overview Config", order = 1)]
public class OverviewConfig : ScriptableObject, IGameOverviewConfig {
	[SerializeField]
	[Min(0.1f)]
	public float fightingPhaseDuration;

	public float FightingPhaseDuration => fightingPhaseDuration;
}
}
