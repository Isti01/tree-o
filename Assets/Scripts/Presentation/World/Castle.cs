using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(HealthBarController))]
public class Castle : Structure {
	[SerializeField]
	private CastleData blueCastleData;

	[SerializeField]
	private CastleData redCastleData;

	private Logic.Data.World.Castle _data;

	[SerializeField]
	private SpriteRenderer spriteConstant;

	[SerializeField]
	private SpriteRenderer spriteColored;

	[SerializeField]
	private Light2D pointLight;

	private bool _destroyed;

	private HealthBarController _healthBarController;

	private void Start() {
		_healthBarController = GetComponent<HealthBarController>();
	}

	/// <summary>
	/// Updates the displayed castle type
	/// </summary>
	public void SetData(Logic.Data.World.Castle data) {
		_data = data;
		CastleData type = _data.OwnerColor == Color.Blue ? blueCastleData : redCastleData;

		spriteConstant.sprite = _destroyed ? type.DestroyedSpriteConstant : type.IntactSpriteConstant;
		spriteColored.sprite = _destroyed ? type.DestroyedSpriteColored : type.IntactSpriteColored;
		spriteColored.color = type.Color;
		pointLight.color = type.Color;
		pointLight.enabled = !_destroyed;
	}

	/// <summary>
	/// Updates the displayed health
	/// </summary>
	public void UpdateHealth() {
		_healthBarController.SetHealth(_data.Health / _data.World.Config.CastleStartingHealth);
	}

	/// <summary>
	/// Updates the displayed castle type to it's destroyed variant
	/// </summary>
	public void SetDestroyed() {
		_destroyed = true;
		SetData(_data);
	}
}
}
