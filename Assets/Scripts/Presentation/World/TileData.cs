using UnityEngine;

namespace Presentation.World {
[CreateAssetMenu(fileName = "New Tile", menuName = "World/Tile", order = 1)]
public class TileData : ScriptableObject {
	public Color Color;
	public Sprite Sprite;
	public GameObject AmbientEffect;
}
}
