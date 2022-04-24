using UnityEngine;

namespace Presentation.World {
/// <summary>
/// Enables the obstacle visualization settings to be configured in the Unity Editor
/// </summary>
[CreateAssetMenu(fileName = "New Obstacle", menuName = "World/Structures/Obstacle", order = 1)]
public class ObstacleData : ScriptableObject {
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
