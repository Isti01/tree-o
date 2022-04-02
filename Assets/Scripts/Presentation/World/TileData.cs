using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Tile", menuName = "World/Tile", order = 1)]
public class TileData : ScriptableObject {
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
