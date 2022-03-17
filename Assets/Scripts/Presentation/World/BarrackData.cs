using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Barrack", menuName = "World/Structures/Barrack", order = 1)]
public class BarrackData : ScriptableObject {
	public Color Color;
	public Sprite Sprite;
	public GameObject AmbientEffect;
}
}
