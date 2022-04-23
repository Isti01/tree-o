using UnityEngine;

namespace Presentation.World {
/// <summary>
/// Enables the barrack visualization settings to be configured in the Unity Editor
/// </summary>
[CreateAssetMenu(fileName = "New Barrack", menuName = "World/Structures/Barrack", order = 1)]
public class BarrackData : ScriptableObject {
	[SerializeField]
	private Color color;

	[SerializeField]
	private Sprite spriteConstant;

	[SerializeField]
	private Sprite spriteColored;

	[SerializeField]
	private GameObject ambientEffect;

	public Color Color => color;
	public Sprite SpriteConstant => spriteConstant;
	public Sprite SpriteColored => spriteColored;
	public GameObject AmbientEffect => ambientEffect;
}
}
