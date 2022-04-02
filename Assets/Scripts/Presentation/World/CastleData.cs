using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Castle", menuName = "World/Structures/Castle", order = 1)]
public class CastleData : ScriptableObject {
	[SerializeField]
	private Color color;

	[SerializeField]
	private Sprite intactSprite;

	[SerializeField]
	private Sprite destroyedSprite;

	[SerializeField]
	private GameObject ambientEffect;

	[SerializeField]
	private GameObject explosionEffect;

	public Color Color => color;
	public Sprite IntactSprite => intactSprite;
	public Sprite DestroyedSprite => destroyedSprite;
	public GameObject AmbientEffect => ambientEffect;
	public GameObject ExplosionEffect => explosionEffect;
}
}
