using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
public class Barrack : Structure {
	[SerializeField]
	private BarrackData blueBarrackData;

	[SerializeField]
	private BarrackData redBarrackData;

	private Logic.Data.World.Barrack _data;

	[SerializeField]
	private SpriteRenderer spriteConstant;

	[SerializeField]
	private SpriteRenderer spriteColored;

	public void SetData(Logic.Data.World.Barrack data) {
		_data = data;
		BarrackData type = _data.OwnerColor == Color.Blue ? blueBarrackData : redBarrackData;

		spriteConstant.sprite = type.SpriteConstant;
		spriteColored.sprite = type.SpriteColored;
		spriteColored.color = type.Color;
	}
}
}
