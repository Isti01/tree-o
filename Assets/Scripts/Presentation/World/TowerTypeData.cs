using Logic.Data.World;
using UnityEngine;

namespace Presentation.World {

[CreateAssetMenu(fileName = "New Tower Type", menuName = "World/Structures/Tower Type", order = 1)]
public class TowerTypeData : ScriptableObject, ITowerTypeData {
	[Header("Presentation-specific values")]
	[SerializeField]
	private Sprite sprite; //TODO can we use the same sprite in the UI and in the "world"?

	[SerializeField]
	private Color blueColor;

	[SerializeField]
	private Color redColor;

	[SerializeField]
	private GameObject shootingEffect;

	[SerializeField]
	private GameObject ambientEffect;

	public Sprite Sprite => sprite;
	public Color BlueColor => blueColor;
	public Color RedColor => redColor;
	public GameObject ShootingEffect => shootingEffect;
	public GameObject AmbientEffect => ambientEffect;

	[Header("ITowerTypeData values")]
	[SerializeField]
	private new string name;

	[SerializeField]
	[Min(0)]
	private float damage;

	[SerializeField]
	[Min(0)]
	private float range;

	[SerializeField]
	[Min(0)]
	private float cooldownTime;

	[SerializeField]
	[Min(0)]
	private int buildingCost;

	[SerializeField]
	[Min(0)]
	private int destroyRefund;

	[SerializeField]
	[Min(0)]
	private int upgradeCost;

	[SerializeField]
	private TowerTypeData afterUpgradeType;

	public string Name => name;
	public float Damage => damage;
	public float Range => range;
	public float CooldownTime => cooldownTime;
	public int BuildingCost => buildingCost;
	public int DestroyRefund => destroyRefund;
	public int UpgradeCost => upgradeCost;
	public ITowerTypeData AfterUpgradeType => afterUpgradeType;
}

}
