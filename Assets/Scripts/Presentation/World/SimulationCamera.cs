using Presentation.UI;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Presentation.World {
[RequireComponent(typeof(Camera))]
public class SimulationCamera : MonoBehaviour {
	[SerializeField]
	private float minZoomLevel = 5;

	[SerializeField]
	private float maxZoomLevel = 35;

	[SerializeField]
	private float zoomSensitivity = 1;

	[SerializeField]
	private float zoomSpeed = 30;

	[SerializeField]
	private Vector2 panSensitivity = new Vector2(0.02f, 0.02f);

	[SerializeField]
	private float panSpeed = 100;

	private Camera _cam;
	private bool _isRightButtonDown;
	private Vector3 _panTarget;
	private float _zoomTarget;
	private bool _shouldZoom = true;

	private void Start() {
		_cam = GetComponent<Camera>();
		_zoomTarget = _cam.orthographicSize;
		_panTarget = transform.position;

		SetupCallbacks();
	}

	private void Update() {
		if (_shouldZoom) {
			_zoomTarget -= Input.mouseScrollDelta.y * zoomSensitivity;
			_zoomTarget = Mathf.Clamp(_zoomTarget, minZoomLevel, maxZoomLevel);
		}

		_cam.orthographicSize = Mathf.MoveTowards(_cam.orthographicSize, _zoomTarget, zoomSpeed * Time.deltaTime);
		transform.position = Vector3.MoveTowards(transform.position, _panTarget, panSpeed * Time.deltaTime);
	}

	private void SetupCallbacks() {
		var simulationUI = FindObjectOfType<SimulationUI>();

		Vector2 mousePosition = Vector2.zero;
		simulationUI.OnGameViewPanStart += e => {
			mousePosition = e.mousePosition;
			_isRightButtonDown |= e.button == 1;
		};

		simulationUI.OnGameViewPanEnd += e => { _isRightButtonDown &= e.button != 1; };

		simulationUI.OnGameViewPanUpdate += e => {
			if (!_isRightButtonDown) return;
			var mouseDelta = e.mousePosition - mousePosition;
			_panTarget -= new Vector3(mouseDelta.x * panSensitivity.x, -mouseDelta.y * panSensitivity.y, 0);
			mousePosition = e.mousePosition;
		};

		simulationUI.OnGameViewMouseEnter += e => { _shouldZoom = true; };

		simulationUI.OnGameViewMouseLeave += e => {
			_isRightButtonDown = false;
			_shouldZoom = false;
		};
	}
}
}
