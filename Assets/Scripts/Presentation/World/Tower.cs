using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class Tower : Structure {
	private Logic.Data.World.Tower _data;

	private SpriteRenderer _spriteRenderer;

	public void SetData(Logic.Data.World.Tower data) {
		_data = data;
		TowerTypeData type = (TowerTypeData) data.Type;
		UnityEngine.Color color = _data.OwnerColor == Color.Blue ? type.BlueColor : type.RedColor;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = type.Sprite;
		_spriteRenderer.color = color;

		LineRenderer laserRenderer = GetComponent<LineRenderer>();
		Gradient laserGradient = new Gradient();
		laserGradient.SetKeys(new[] { new GradientColorKey(color, 0), new GradientColorKey(color, 1) },
			new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
		laserRenderer.colorGradient = laserGradient;
	}

	public void DestroyTower() {
		Destroy(gameObject);
	}
}
}
