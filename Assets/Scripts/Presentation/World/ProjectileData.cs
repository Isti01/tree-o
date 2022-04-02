using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Projectile", menuName = "World/Projectile", order = 1)]
public class ProjectileData : ScriptableObject {
	[SerializeField]
	private Color color;

	[SerializeField]
	private Sprite sprite;

	[SerializeField]
	private GameObject flyingEffect;

	[SerializeField]
	private GameObject impactEffect;

	public Color Color => color;
	public Sprite Sprite => sprite;
	public GameObject FlyingEffect => flyingEffect;
	public GameObject ImpactEffect => impactEffect;
}
}
