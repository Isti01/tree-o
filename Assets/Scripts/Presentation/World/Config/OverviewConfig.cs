using Logic.Data;
using UnityEngine;

namespace Presentation.World.Config {
/// <summary>
///     Enables the overall game settings to be configured in the Unity Editor
/// </summary>
[CreateAssetMenu(fileName = "New Overview Config", menuName = "World/Config/Overview Config", order = 1)]
public class OverviewConfig : ScriptableObject, IGameOverviewConfig {
	[SerializeField]
	[Min(0.1f)]
	private float fightingPhaseDuration;

	public float FightingPhaseDuration => fightingPhaseDuration;
}
}
