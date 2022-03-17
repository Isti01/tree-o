using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Unit : MonoBehaviour {
	[SerializeField]
	private UnitData redUnitData;

	[SerializeField]
	private UnitData blueUnitData;

	private Logic.Data.World.Unit _data;

	private SpriteRenderer _spriteRenderer;

	private void Awake() {
		UnitData data = _data.Owner.TeamColor == Color.Blue ? blueUnitData : redUnitData;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = data.AliveSprite;
		_spriteRenderer.color = data.Color;
	}

	public void SetData(Logic.Data.World.Unit data) {
		_data = data;
	}
}
}
