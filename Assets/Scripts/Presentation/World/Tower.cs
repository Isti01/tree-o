using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Tower : Structure {
	private Logic.Data.World.Tower _data;

	private SpriteRenderer _spriteRenderer;

	private void Start() {
		TowerTypeData data = (TowerTypeData) _data.Type;
		UnityEngine.Color color = _data.OwnerColor == Color.Blue ? data.BlueColor : data.RedColor;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = data.Sprite;
		_spriteRenderer.color = color;
	}

	public void SetData(Logic.Data.World.Tower data) {
		_data = data;
	}
}
}
