using UnityEngine;

namespace Presentation.World {
/// <summary>
/// Enables the tile highlight visualization settings to be configured in the Unity Editor
/// </summary>
[CreateAssetMenu(fileName = "New Tile Highlight", menuName = "World/Structures/Tile Highlight", order = 1)]
public class TileHighlightData : ScriptableObject {
	[SerializeField]
	private Color lightColor = Color.white;

	[SerializeField]
	private float intensity = 1f;

	[SerializeField]
	private float dimmedIntensity = .5f;

	public Color LightColor => lightColor;
	public float Intensity => intensity;
	public float DimmedIntensity => dimmedIntensity;
}
}
