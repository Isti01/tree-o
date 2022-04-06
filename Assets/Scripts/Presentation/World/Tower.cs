using UnityEngine;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class Tower : Structure {
	private Logic.Data.World.Tower _data;

	private SpriteRenderer _spriteRenderer;

	public void SetData(Logic.Data.World.Tower data) {
		_data = data;
		var type = (TowerTypeData) data.Type;
		Color color = _data.OwnerColor == Logic.Data.Color.Blue ? type.BlueColor : type.RedColor;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = type.Sprite;
		_spriteRenderer.color = color;

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
