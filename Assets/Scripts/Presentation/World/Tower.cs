using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Presentation.World {
[RequireComponent(typeof(LineRenderer))]
public class Tower : Structure {
	[SerializeField]
	private Transform rotatingTurret;

	private Logic.Data.World.Tower _data;

	[SerializeField]
	private SpriteRenderer backgroundSprite;

	[SerializeField]
	private SpriteRenderer constantSprite;

	[SerializeField]
	private SpriteRenderer coloredSprite;

	[SerializeField]
	private float turretRotationSpeed;

	[SerializeField]
	private Light2D pointLight;

	[SerializeField]
	private LineRenderer laserRenderer;

	private World _world;

	private void Awake() {
		_world = FindObjectOfType<World>();
	}

	private void FixedUpdate() {
		if (_data.Type.Range > 0) {
			Logic.Data.World.Unit logicTarget = _data.Target ?? _data.ClosestEnemy;
			if (logicTarget != null) RotateTurret(logicTarget);
		}
	}

	/// <summary>
	/// Sets the displayed tower type
	/// </summary>
	public void SetData(Logic.Data.World.Tower data) {
		_data = data;
		var type = (TowerTypeData) data.Type;

		Color color = _data.OwnerColor == Logic.Data.Color.Blue ? type.BlueColor : type.RedColor;
		backgroundSprite.sprite = type.SpriteBackground;
		constantSprite.sprite = type.SpriteConstant;
		coloredSprite.sprite = type.SpriteColored;
		coloredSprite.color = color;
		pointLight.color = color;

		var laserGradient = new Gradient();
		laserGradient.SetKeys(new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
			new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
		laserRenderer.colorGradient = laserGradient;
	}

	/// <summary>
	/// Removes the tower from the scene
	/// </summary>
	public void DestroyTower() {
		Destroy(gameObject);
	}

	private void RotateTurret(Logic.Data.World.Unit logicTarget) {
		Vector3 target = _world.LogicToPresentation(logicTarget).transform.position;
		Vector3 direction = (target - rotatingTurret.position).normalized;
		float angle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
		Quaternion desired = Quaternion.AngleAxis(angle - 90, Vector3.forward);
		rotatingTurret.rotation = Quaternion.RotateTowards(rotatingTurret.rotation, desired,
			Time.fixedDeltaTime * turretRotationSpeed);
	}
}
}
