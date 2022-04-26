using UnityEngine;

namespace Presentation.World {
/// <summary>
///     Enables the tile visualization settings to be configured in the Unity Editor
/// </summary>
[CreateAssetMenu(fileName = "New Tile", menuName = "World/Tile", order = 1)]
public class TileData : ScriptableObject {
	[SerializeField]
	private Color evenColor;

	[SerializeField]
	private Color oddColor;

	[SerializeField]
	private Sprite sprite;

	public Color EvenColor => evenColor;
	public Color OddColor => oddColor;
	public Sprite Sprite => sprite;
}
}
