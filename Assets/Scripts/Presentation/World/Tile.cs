using Logic.Data.World;
using UnityEngine;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour {
	[SerializeField]
	private TileData tileData;

	private SpriteRenderer _spriteRenderer;

	public TilePosition Position { get; set; }

	private void Start() {
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.color = (Position.X + Position.Y) % 2 == 0
			? tileData.OddColor //position 0,0 is the first tile, which is an "odd" position
			: tileData.EvenColor;
		_spriteRenderer.sprite = tileData.Sprite;
	}
}
}
