using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(HealthbarController))]
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

	private bool _destroyed = false;

	private HealthbarController _healthbarController;

	private void Start() {
		_healthbarController = GetComponent<HealthbarController>();
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
	}

	/// <summary>
	/// Updates the displayed health
	/// </summary>
	public void UpdateHealth() {
		_healthbarController.SetHealth(_data.Health / _data.World.Config.CastleStartingHealth);
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
