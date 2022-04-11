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
	private Sprite previewSprite;

	[SerializeField]
	private Sprite aliveSpriteConstant;

	[SerializeField]
	private Sprite aliveSpriteColored;

	[SerializeField]
	private Sprite destroyedSprite;

	[SerializeField]
	private GameObject attackEffect;

	[SerializeField]
	private GameObject destroyedEffect;

	[SerializeField]
	private bool airborne;

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

	public Color BlueColor => blueColor;
	public Color RedColor => redColor;
	public Sprite PreviewSprite => previewSprite;
	public Sprite AliveSpriteConstant => aliveSpriteConstant;
	public Sprite AliveSpriteColored => aliveSpriteColored;
	public Sprite DestroyedSprite => destroyedSprite;
	public GameObject AttackEffect => attackEffect;
	public GameObject DestroyedEffect => destroyedEffect;
	public bool Airborne => airborne;

	public string Name => name;
	public float Health => health;
	public float Damage => damage;
	public float Speed => speed;
	public int Cost => cost;
}
}
