using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(HealthBarController))]
public class Unit : MonoBehaviour {
	private Logic.Data.World.Unit _data;

	private Vector3 _lastPosition = Vector3.zero;

	[SerializeField]
	private Transform rotatingChild;

	[SerializeField]
	private SpriteRenderer constantSprite;

	[SerializeField]
	private SpriteRenderer coloredSprite;

	[SerializeField]
	private Light2D pointLight;

	private HealthBarController _healthBarController;

	private void Awake() {
		_healthBarController = GetComponent<HealthBarController>();
	}

	private void FixedUpdate() {
		var newPosition = new Vector3(_data.Position.X - 0.5f, _data.Position.Y - 0.5f);
		if (_lastPosition == newPosition) return;
		Vector3 direction = (newPosition - _lastPosition).normalized;
		float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
		_lastPosition = newPosition;
		rotatingChild.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
		transform.localPosition = newPosition;
	}

	/// <summary>
	/// Sets the displayed unit type
	/// </summary>
	public void SetData(Logic.Data.World.Unit data) {
		_data = data;
		var unitData = (UnitTypeData) _data.Type;
		if (unitData.Airborne) {
			constantSprite.sortingLayerName = "UnitAirborne";
			coloredSprite.sortingLayerName = "UnitAirborne";
		} else {
			constantSprite.sortingLayerName = "UnitGround";
			coloredSprite.sortingLayerName = "UnitGround";
		}

		var color = _data.Owner.TeamColor == Color.Blue ? unitData.BlueColor : unitData.RedColor;

		constantSprite.sprite = unitData.AliveSpriteConstant;
		coloredSprite.sprite = unitData.AliveSpriteColored;
		coloredSprite.color = color;
		pointLight.color = color;
		UpdateHealth();
	}

	/// <summary>
	/// Updates the displayed health
	/// </summary>
	public void UpdateHealth() {
		_healthBarController.SetHealth(_data.CurrentHealth / _data.Type.Health);
	}

	/// <summary>
	/// Removes the unit from the scene
	/// </summary>
	public void DestroyUnit() {
		Destroy(gameObject);
	}
}
}
