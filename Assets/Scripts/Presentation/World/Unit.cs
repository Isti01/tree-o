using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Unit : MonoBehaviour {
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
		UnitTypeData unitData = (UnitTypeData) _data.Type;
		_spriteRenderer.sprite = unitData.AliveSprite;
		_spriteRenderer.color = _data.Owner.TeamColor == Color.Blue ? unitData.BlueColor : unitData.RedColor;
	}

	public void DestroyUnit() {
		Destroy(gameObject);
	}
}
}
