using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Castle : Structure {
	[SerializeField]
	private CastleData blueCastleData;

	[SerializeField]
	private CastleData redCastleData;

	private Logic.Data.World.Castle _data;

	private SpriteRenderer _spriteRenderer;

	private void Start() {
		CastleData data = _data.OwnerColor == Color.Blue ? blueCastleData : redCastleData;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = data.IntactSprite;
		_spriteRenderer.color = data.Color;
	}

	public void SetData(Logic.Data.World.Castle data) {
		_data = data;
	}
}
}
