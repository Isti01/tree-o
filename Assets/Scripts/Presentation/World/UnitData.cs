using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Unit", menuName = "World/Unit", order = 1)]
public class UnitData : ScriptableObject {
	public Color Color;
	public Sprite AliveSprite;
	public Sprite DestroyedSprite;
	public GameObject AttackEffect;
	public GameObject DestroyedEffect;
}
}
