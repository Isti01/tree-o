using System.Numerics;
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

	private void Start() {
		_cam = GetComponent<Camera>();
		_zoomTarget = _cam.orthographicSize;
		_panTarget = transform.position;

		SetupCallbacks();
	}

	private void Update() {
		_zoomTarget -= Input.mouseScrollDelta.y * zoomSensitivity;
		_zoomTarget = Mathf.Clamp(_zoomTarget, minZoomLevel, maxZoomLevel);

		_cam.orthographicSize = Mathf.MoveTowards(_cam.orthographicSize, _zoomTarget, zoomSpeed * Time.deltaTime);
		transform.position = Vector3.MoveTowards(transform.position, _panTarget, panSpeed * Time.deltaTime);
	}

	private void SetupCallbacks() {
		var simulationUI = FindObjectOfType<SimulationUI>();

		Vector2 mousePosition = Vector2.zero;
		simulationUI.OnGameViewPanStart += evt => {
			mousePosition = evt.mousePosition;
			_isRightButtonDown |= evt.button == 1;
		};

		simulationUI.OnGameViewPanEnd += evt => { _isRightButtonDown &= evt.button != 1; };

		simulationUI.OnGameViewPanUpdate += evt => {
			if (!_isRightButtonDown) return;
			var mouseDelta = evt.mousePosition - mousePosition;
			_panTarget -= new Vector3(mouseDelta.x * panSensitivity.x, -mouseDelta.y * panSensitivity.y, 0);
			mousePosition = evt.mousePosition;
		};
	}
}
}
