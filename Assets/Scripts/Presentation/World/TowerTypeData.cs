using Logic.Data.World;
using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Tower Type", menuName = "World/Structures/Tower Type", order = 1)]
public class TowerTypeData : ScriptableObject, ITowerTypeData {
	public new string name;

	public float damage;

	public float range;

	public float cooldownTime;

	public int buildingCost;

	public int upgradeCost;
	public string Name => name;
	public float Damage => damage;
	public float Range => range;
	public float CooldownTime => cooldownTime;
	public int BuildingCost => buildingCost;
	public int UpgradeCost => upgradeCost;
}
}
