using UnityEngine;

namespace Presentation.World {
/// <summary>
/// Enables the castle visualization settings to be configured in the Unity Editor
/// </summary>
[CreateAssetMenu(fileName = "New Castle", menuName = "World/Structures/Castle", order = 1)]
public class CastleData : ScriptableObject {
	[SerializeField]
	private Color color;

	[SerializeField]
	private Sprite intactSpriteConstant;

	[SerializeField]
	private Sprite intactSpriteColored;

	[SerializeField]
	private Sprite destroyedSpriteConstant;

	[SerializeField]
	private Sprite destroyedSpriteColored;

	[SerializeField]
	private GameObject ambientEffect;

	[SerializeField]
	private GameObject explosionEffect;

	public Color Color => color;
	public Sprite IntactSpriteConstant => intactSpriteConstant;
	public Sprite IntactSpriteColored => intactSpriteColored;
	public Sprite DestroyedSpriteConstant => destroyedSpriteConstant;
	public Sprite DestroyedSpriteColored => destroyedSpriteColored;
	public GameObject AmbientEffect => ambientEffect;
	public GameObject ExplosionEffect => explosionEffect;
}
}
