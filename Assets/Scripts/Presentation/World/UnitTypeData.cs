using Logic.Data.World;
using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Unit Type", menuName = "World/Unit Type", order = 1)]
public class UnitTypeData : ScriptableObject, IUnitTypeData {
	[Header("Presentation-specific values")]
	[SerializeField]
	private Color blueColor;

	[SerializeField]
	private Color redColor;

	[SerializeField]
	private Sprite aliveSprite; //TODO can we use this sprite in the UI?

	[SerializeField]
	private Sprite destroyedSprite;

	[SerializeField]
	private GameObject attackEffect;

	[SerializeField]
	private GameObject destroyedEffect;

	public Color BlueColor => blueColor;
	public Color RedColor => redColor;
	public Sprite AliveSprite => aliveSprite;
	public Sprite DestroyedSprite => destroyedSprite;
	public GameObject AttackEffect => attackEffect;
	public GameObject DestroyedEffect => destroyedEffect;

	[Header("IUnitTypeData values")]
	[SerializeField]
	private new string name;

	[SerializeField]
	[Min(0.01f)]
	private float health;

	[SerializeField]
	[Min(0)]
	private float damage;

	[SerializeField]
	[Min(0)]
	private float speed;

	[SerializeField]
	[Min(0)]
	private int cost;

	public string Name => name;
	public float Health => health;
	public float Damage => damage;
	public float Speed => speed;
	public int Cost => cost;
}
}
