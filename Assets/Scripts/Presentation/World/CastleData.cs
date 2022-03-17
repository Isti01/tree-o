using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Castle", menuName = "World/Structures/Castle", order = 1)]
public class CastleData : ScriptableObject {
	public Color Color;
	public Sprite IntactSprite;
	public Sprite DestroyedSprite;
	public GameObject AmbientEffect;
	public GameObject ExplosionEffect;
}
}
