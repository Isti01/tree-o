using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Tower : Structure {
	[SerializeField]
	private TowerData blueTowerData;

	[SerializeField]
	private TowerData redTowerData;

	private Logic.Data.World.Tower _data;

	private SpriteRenderer _spriteRenderer;

	private void Start() {
		TowerData data = _data.OwnerColor == Color.Blue ? blueTowerData : redTowerData;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = data.Sprite;
		_spriteRenderer.color = data.Color;
	}

	public void SetData(Logic.Data.World.Tower data) {
		_data = data;
	}
}
}
