using System;
using UnityEngine;

namespace Presentation.World {

public class HealthbarController : MonoBehaviour {
	[SerializeField]
	private float fullyVisibleDuration = 1f;

	[SerializeField]
	private float fadingDuration = 0.5f;

	[SerializeField]
	private Transform healthContainer;

	[SerializeField]
	private SpriteRenderer backgroundSprite;

	[SerializeField]
	private SpriteRenderer healthSprite;

	private float lastActionTime = Mathf.NegativeInfinity;

	private void Update() {
		SetTransparency();
	}

	public void MakeVisible() {
		lastActionTime = Time.time;
		SetTransparency();
	}

	public void SetHealth(float ratio) {
		if (Math.Abs(healthContainer.localScale.x - ratio) < 0.001) return;
		healthContainer.localScale = new Vector3(ratio, 1, 1);
		MakeVisible();
	}

	private void SetTransparency() {
		float newAlpha;

		float delta = Time.time - lastActionTime;
		if (delta < fullyVisibleDuration) {
			newAlpha = 1;
		} else if (delta - fullyVisibleDuration < fadingDuration) {
			newAlpha = Mathf.Lerp(1, 0, (delta - fullyVisibleDuration) / fadingDuration);
		} else {
			newAlpha = 0;
		}

		Color backgroundColor = backgroundSprite.color;
		backgroundColor.a = newAlpha;
		backgroundSprite.color = backgroundColor;

		Color healthColor = healthSprite.color;
		healthColor.a = newAlpha;
		healthSprite.color = healthColor;
	}
}

}
