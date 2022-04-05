using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Presentation.World {
[RequireComponent(typeof(Light2D))]
public class TileHighlight : MonoBehaviour {
	private TileHighlightData _highlightData;
	private Light2D _light;
	private bool _dimmed;

	private void Start() {
		_light = GetComponent<Light2D>();
	}

	public void SetDimmed(bool dimmed = true) {
		_dimmed = dimmed;
		UpdateTile();
	}

	public void SetData(TileHighlightData data) {
		_highlightData = data;
		_dimmed = false;
		UpdateTile();
	}

	private void UpdateTile() {
		_light.color = _highlightData.LightColor;
		_light.intensity = _dimmed ? _highlightData.DimmedIntensity : _highlightData.Intensity;
	}
}
}
