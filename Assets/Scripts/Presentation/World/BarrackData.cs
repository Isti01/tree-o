using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Barrack", menuName = "World/Structures/Barrack", order = 1)]
public class BarrackData : ScriptableObject {
	[SerializeField]
	private Color color;

	[SerializeField]
	private Sprite sprite;

	[SerializeField]
	private GameObject ambientEffect;

	public Color Color => color;
	public Sprite Sprite => sprite;
	public GameObject AmbientEffect => ambientEffect;
}
}
