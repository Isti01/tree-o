using UnityEngine;

namespace Presentation.World {
[RequireComponent(typeof(LineRenderer))]
public class Tower : Structure {
	private Logic.Data.World.Tower _data;

	[SerializeField]
	private SpriteRenderer backgroundSprite;

	[SerializeField]
	private SpriteRenderer constantSprite;

	[SerializeField]
	private SpriteRenderer coloredSprite;

	public void SetData(Logic.Data.World.Tower data) {
		_data = data;
		var type = (TowerTypeData) data.Type;

		Color color = _data.OwnerColor == Logic.Data.Color.Blue ? type.BlueColor : type.RedColor;
		backgroundSprite.sprite = type.SpriteBackground;
		constantSprite.sprite = type.SpriteConstant;
		coloredSprite.sprite = type.SpriteColored;
		coloredSprite.color = color;

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
