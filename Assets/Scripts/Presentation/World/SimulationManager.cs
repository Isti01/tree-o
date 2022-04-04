using System;
using Logic.Command;
using Logic.Data;
using Logic.Data.World;
using Presentation.UI;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;

namespace Presentation.World {
public class SimulationManager : MonoBehaviour {
	public int worldWidth = 10;
	public int worldHeight = 10;
	public int seed = 1337;

	[SerializeField]
	private OverviewConfig overviewConfig;

	[SerializeField]
	private EconomyConfig economyConfig;

	[SerializeField]
	private WorldConfig worldConfig;

	[SerializeField]
	private Camera mainCamera;

	public bool IsPaused { get; private set; }

	public GameOverview GameOverview { get; private set; }
	private SimulationUI _simulationUI;

	private void Awake() {
		void ExceptionHandler(Exception e) {
			Debug.LogError($"[Logic Exception]: ${e}");
		}

		GameOverview = new GameOverview(ExceptionHandler, seed, worldWidth, worldHeight,
			overviewConfig, economyConfig, worldConfig);
	}

	private void Start() {
		_simulationUI = FindObjectOfType<SimulationUI>();
		_simulationUI.OnGameViewMouseUp += SelectTile;
	}

	private void OnDestroy() {
		_simulationUI.OnGameViewMouseUp -= SelectTile;
	}

	private void FixedUpdate() {
		GameOverview?.Commands?.Issue(new AdvanceTimeCommand(GameOverview, Time.fixedDeltaTime));
	}

	private void SelectTile(MouseUpEvent e) {
		if (e.button != 0) return;

		Vector2 rayOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition); // It's Input.mouse position on purpose
		RaycastHit2D result = Physics2D.Raycast(rayOrigin, Vector2.zero);
		if (!result) return;

		var tile = result.collider.gameObject.GetComponent<Tile>();
		OnTileSelected?.Invoke(tile.Position);
	}

	public void ResumeGame() {
		Time.timeScale = 1;
		IsPaused = false;
	}

	public void PauseGame() {
		Time.timeScale = 0;
		IsPaused = true;
	}

	public event Action<TilePosition> OnTileSelected;
}
}
