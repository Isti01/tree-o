using UnityEngine;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {
	[SerializeField]
	private TileData tileData;

	private SpriteRenderer _spriteRenderer;

	private void Start() {
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.color = tileData.Color;
		_spriteRenderer.sprite = tileData.Sprite;
	}
}
}
