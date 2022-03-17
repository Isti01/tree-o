using UnityEngine;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Obstacle : Structure {
	[SerializeField]
	private ObstacleData obstacleData;

	private SpriteRenderer _spriteRenderer;

	private void Start() {
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = obstacleData.Sprite;
		_spriteRenderer.color = obstacleData.Color;
	}
}
}
