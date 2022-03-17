using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Projectile", menuName = "World/Projectile", order = 1)]
public class ProjectileData : ScriptableObject {
	public Color Color;
	public Sprite Sprite;
	public GameObject FlyingEffect;
	public GameObject ImpactEffect;
}
}
