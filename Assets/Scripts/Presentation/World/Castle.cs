using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
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

	private void Start() {
		CastleData data = _data.OwnerColor == Color.Blue ? blueCastleData : redCastleData;

		spriteConstant.sprite = data.IntactSpriteConstant;
		spriteColored.sprite = data.IntactSpriteColored;
		spriteColored.color = data.Color;
	}

	public void SetData(Logic.Data.World.Castle data) {
		_data = data;
	}
}
}
