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
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void FixedUpdate() {
		transform.localPosition = new Vector3(_data.Position.X - 0.5f, _data.Position.Y - 0.5f);
	}

	public void SetData(Logic.Data.World.Unit data) {
		_data = data;
		UnitData unitData = _data.Owner.TeamColor == Color.Blue ? blueUnitData : redUnitData;
		_spriteRenderer.sprite = unitData.AliveSprite;
		_spriteRenderer.color = unitData.Color;
	}

	public void DestroyUnit() {
		Destroy(gameObject);
	}
}
}
