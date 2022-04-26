using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Presentation.World {
public class WorldLight : MonoBehaviour {
	[SerializeField]
	private Light2D shapeLight;

	public void SetDimensions(int width, int height) {
		shapeLight.lightType = Light2D.LightType.Freeform;
		transform.localPosition = new Vector3(-.5f, -.5f);
		Vector3[] offsets = {
			new Vector3(0, height), new Vector3(0, 0), new Vector3(width, 0), new Vector3(width, height)
		};

		for (int i = 0; i < offsets.Length; i++) shapeLight.shapePath[i] = offsets[i];
	}
}
}
