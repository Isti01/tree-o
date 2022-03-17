using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Tower", menuName = "World/Structures/Tower", order = 1)]
public class TowerData : ScriptableObject {
	public Color Color;
	public Sprite Sprite;
	public GameObject ShootingEffect;
	public GameObject AmbientEffect;
}
}
