using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Barrack : Structure {
	[SerializeField]
	private BarrackData blueBarrackData;

	[SerializeField]
	private BarrackData redBarrackData;

	private Logic.Data.World.Barrack _data;

	private SpriteRenderer _spriteRenderer;

	private void Start() {
		BarrackData data = _data.OwnerColor == Color.Blue ? blueBarrackData : redBarrackData;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = data.Sprite;
		_spriteRenderer.color = data.Color;
	}

	public void SetData(Logic.Data.World.Barrack data) {
		_data = data;
	}
}
}
