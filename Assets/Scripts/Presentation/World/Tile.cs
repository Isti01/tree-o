using UnityEngine;
using Vector2 = Logic.Data.World.Vector2;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {
	[SerializeField]
	private TileData tileData;

	private SpriteRenderer _spriteRenderer;

	public Vector2 Position { get; set; }

	private void Start() {
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.color = tileData.Color;
		_spriteRenderer.sprite = tileData.Sprite;
	}
}
}
