using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Presentation.World {
[RequireComponent(typeof(Light2D))]
public class TileHighlight : MonoBehaviour {
	[SerializeField]
	private TileHighlightData highlightData;

	private bool _dimmed;

	private Light2D _light;
	private float _scale = 1;

	private void Awake() {
		_light = GetComponent<Light2D>();
		if (highlightData) SetData(highlightData);
	}

	/// <summary>
	///     Sets the highlight intensity to dimmed
	/// </summary>
	public void SetDimmed(bool dimmed = true) {
		_dimmed = dimmed;
		UpdateTile();
	}

	/// <summary>
	///     Sets the highlight radius
	/// </summary>
	public void SetRadius(float outerRadius, float innerRadius = 0.0f) {
		_light.pointLightOuterRadius = outerRadius;
		_light.pointLightInnerRadius = innerRadius;
	}

	/// <summary>
	///     Scales the highlight intensity
	/// </summary>
	public void ScaleIntensity(float scale) {
		_scale = scale;
		UpdateTile();
	}

	/// <summary>
	///     Updates the displayed highlight type
	/// </summary>
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
