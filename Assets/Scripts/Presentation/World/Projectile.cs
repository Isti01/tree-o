using UnityEngine;
using Color = Logic.Data.Color;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour {
	[SerializeField]
	private ProjectileData blueProjectileData;

	[SerializeField]
	private ProjectileData redProjectileData;

	private SpriteRenderer _spriteRenderer;

	private Color _teamColor = Color.Blue;

	private void Start() {
		ProjectileData data = _teamColor == Color.Blue ? blueProjectileData : redProjectileData;

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = data.Sprite;
		_spriteRenderer.color = data.Color;
	}

	public void SetTeamColor(Color teamColor) {
		_teamColor = teamColor;
	}
}
}
