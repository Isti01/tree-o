using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(HealthbarController))]
public class Unit : MonoBehaviour {
	private Logic.Data.World.Unit _data;

	private Vector3 _lastPosition = Vector3.zero;

	[SerializeField]
	private Transform rotatingChild;

	private SpriteRenderer _spriteRenderer;
	private HealthbarController _healthbarController;

	private void Awake() {
		_spriteRenderer = rotatingChild.GetComponent<SpriteRenderer>();
		_healthbarController = GetComponent<HealthbarController>();
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

	public void SetData(Logic.Data.World.Unit data) {
		_data = data;
		var unitData = (UnitTypeData) _data.Type;
		_spriteRenderer.sprite = unitData.AliveSprite;
		_spriteRenderer.color = _data.Owner.TeamColor == Color.Blue ? unitData.BlueColor : unitData.RedColor;
		UpdateHealth();
	}

	public void UpdateHealth() {
		_healthbarController.SetHealth(_data.CurrentHealth / _data.Type.Health);
	}

	public void DestroyUnit() {
		Destroy(gameObject);
	}
}
}
