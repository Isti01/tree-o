using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Presentation.World {
[RequireComponent(typeof(SpriteRenderer))]
public class Environment : MonoBehaviour {
	[SerializeField]
	private List<Sprite> sprites;

	private void Start() {
		var spriteRenderer = GetComponent<SpriteRenderer>();
		int index = (int) Math.Round(Random.value * sprites.Count);
		spriteRenderer.sprite = sprites[index % sprites.Count];
	}
}
}
