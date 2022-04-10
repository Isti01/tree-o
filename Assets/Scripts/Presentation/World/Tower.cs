using UnityEngine;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class Tower : Structure {
	[SerializeField]
	private Transform rotatingTurret;

	private Logic.Data.World.Tower _data;

	private SpriteRenderer _spriteRenderer;

	private World _world;

	private void FixedUpdate() {
		if (_data.Target != null) {
			Vector3 target = _world.LogicToPresentation(_data.Target).transform.position;
			Vector3 direction = (target - rotatingTurret.position).normalized;
			float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
			rotatingTurret.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
		}
	}

	public void SetData(Logic.Data.World.Tower data) {
		_data = data;
		var type = (TowerTypeData) data.Type;
		Color color = _data.OwnerColor == Logic.Data.Color.Blue ? type.BlueColor : type.RedColor;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = type.Sprite;
		_spriteRenderer.color = color;

		_world = GetComponentInParent<World>();

		var laserRenderer = GetComponent<LineRenderer>();
		var laserGradient = new Gradient();
		laserGradient.SetKeys(new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
			new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
		laserRenderer.colorGradient = laserGradient;
	}

	public void DestroyTower() {
		Destroy(gameObject);
	}
}
}
