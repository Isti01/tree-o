using System;
using Logic.Command;
using Logic.Data;
using Logic.Data.World;
using Presentation.UI;
using Presentation.World.Config;
using UnityEngine;
using UnityEngine.UIElements;

namespace Presentation.World {
public class SimulationManager : MonoBehaviour {
	public enum MouseButton {
		Left,
		Right
	}

	[SerializeField]
	private OverviewConfig overviewConfig;

	[SerializeField]
	private EconomyConfig economyConfig;

	[SerializeField]
	private WorldConfig worldConfig;

	[SerializeField]
	private Camera mainCamera;

	private SimulationUI _simulationUI;

	public bool IsPaused { get; private set; }

	public IGameOverview GameOverview { get; private set; }

	private void Awake() {
		void ExceptionHandler(Exception e) {
			Debug.LogError($"[Logic Exception]: ${e} {e.InnerException}");
		}

		GameOverview = new GameOverview(ExceptionHandler, UnityEngine.Random.Range(0, 9999),
			overviewConfig, economyConfig, worldConfig);
	}

	private void Start() {
		_simulationUI = FindObjectOfType<SimulationUI>();
		_simulationUI.OnGameViewMouseUp += SelectTile;
	}

	private void Update() {
		HandleHover();
	}

	private void FixedUpdate() {
		GameOverview?.Commands?.Issue(new AdvanceTimeCommand(GameOverview, Time.fixedDeltaTime));
	}

	private void OnDestroy() {
		_simulationUI.OnGameViewMouseUp -= SelectTile;
	}

	private void HandleHover() {
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		int layerMask = 1 << LayerMask.NameToLayer("Unit");
		RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);
		if (!hit) return;

		HealthbarController health = hit.transform.parent.GetComponent<HealthbarController>();
		health.MakeVisible();
	}

	private void SelectTile(MouseUpEvent e) {
		if (e.button != 0 && e.button != 1) return;
		MouseButton button = e.button == 0 ? MouseButton.Left : MouseButton.Right;

		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		int layerMask = 1 << LayerMask.NameToLayer("Tile");
		RaycastHit2D result = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);
		if (!result) return;

		var tile = result.collider.gameObject.GetComponent<Tile>();
		OnTileSelected?.Invoke(tile.Position, button);
	}

	public void ResumeGame() {
		Time.timeScale = 1;
		IsPaused = false;
	}

	public void PauseGame() {
		Time.timeScale = 0;
		IsPaused = true;
	}

	public event Action<TilePosition, MouseButton> OnTileSelected;
}
}
