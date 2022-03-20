using Logic.Data.World;
using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Unit Type", menuName = "World/Unit Type", order = 1)]
public class UnitTypeData : ScriptableObject, IUnitTypeData {
	public Sprite sprite;
	public new string name;
	public float health;
	public float damage;
	public float speed;
	public int cost;

	public string Name => name;
	public float Health => health;
	public float Damage => damage;
	public float Speed => speed;
	public int Cost => cost;
}
}
