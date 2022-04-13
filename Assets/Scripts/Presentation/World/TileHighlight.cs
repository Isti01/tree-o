using UnityEngine;


namespace Presentation.World {
[RequireComponent(typeof(UnityEngine.Rendering.Universal.Light2D))]
public class TileHighlight : MonoBehaviour {
	[SerializeField]
	private TileHighlightData highlightData;

	private bool _dimmed;

	private UnityEngine.Rendering.Universal.Light2D _light;
	private float _scale = 1;

	private void Awake() {
		_light = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
		if (highlightData) SetData(highlightData);
	}

	public void SetDimmed(bool dimmed = true) {
		_dimmed = dimmed;
		UpdateTile();
	}

	public void SetRadius(float outerRadius, float innerRadius = 0.0f) {
		_light.pointLightOuterRadius = outerRadius;
		_light.pointLightInnerRadius = innerRadius;
	}

	public void ScaleIntensity(float scale) {
		_scale = scale;
		UpdateTile();
	}

	public void SetData(TileHighlightData data) {
		highlightData = data;
		_dimmed = false;
		UpdateTile();
	}

	private void UpdateTile() {
		_light.color = highlightData.LightColor;
		_light.intensity = (_dimmed ? highlightData.DimmedIntensity : highlightData.Intensity) * _scale;
	}
}
}
