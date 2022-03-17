using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Obstacle", menuName = "World/Structures/Obstacle", order = 1)]
public class ObstacleData : ScriptableObject {
	public Color Color;
	public Sprite Sprite;
	public GameObject AmbientEffect;
}
}
