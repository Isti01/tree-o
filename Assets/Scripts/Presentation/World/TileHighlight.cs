using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Presentation.World {
[RequireComponent(typeof(Light2D))]
public class TileHighlight : MonoBehaviour {
	[SerializeField]
	private TileHighlightData highlightData;

	private bool _dimmed;

	private Light2D _light;

	private void Awake() {
		_light = GetComponent<Light2D>();
		if (highlightData) SetData(highlightData);
	}

	public void SetDimmed(bool dimmed = true) {
		_dimmed = dimmed;
		UpdateTile();
	}

	public void SetData(TileHighlightData data) {
		highlightData = data;
		_dimmed = false;
		UpdateTile();
	}

	private void UpdateTile() {
		_light.color = highlightData.LightColor;
		_light.intensity = _dimmed ? highlightData.DimmedIntensity : highlightData.Intensity;
	}
}
}
