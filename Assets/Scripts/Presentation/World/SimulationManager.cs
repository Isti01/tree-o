using System;
using Logic.Command;
using Logic.Data;
using Logic.Data.World;
using Presentation.UI;
using Presentation.World.Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

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

	public bool IsPaused { get; private set; }

	public IGameOverview GameOverview { get; private set; }

	private void Awake() {
		SceneManager.LoadScene("Scenes/SimulationCore", LoadSceneMode.Additive);

		void ExceptionHandler(Exception e) {
			Debug.LogError($"[Logic Exception]: ${e} {e.InnerException}");
		}

		//Clone the ScriptableObject: it calculates and caches a random value,
		//  which shouldn't be persisted between separate games.
		WorldConfig worldConfigClone = Instantiate(worldConfig);

		GameOverview = new GameOverview(ExceptionHandler, Random.Range(0, 9999),
			overviewConfig, economyConfig, worldConfigClone);
	}

	private void Start() {
		var simulationUI = FindObjectOfType<SimulationUI>();
		if (simulationUI != null) simulationUI.OnGameViewMouseUp += SelectTile;
	}

	private void Update() {
		HandleHover();
	}

	private void FixedUpdate() {
		GameOverview?.Commands?.Issue(new AdvanceTimeCommand(GameOverview, Time.fixedDeltaTime));
	}

	private void HandleHover() {
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		int layerMask = (1 << LayerMask.NameToLayer("Unit")) | (1 << LayerMask.NameToLayer("Castle"));
		RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);
		if (!hit) return;

		var health = hit.transform.parent.GetComponent<HealthBarController>();
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

	/// <summary>
	///     Resumes the simulation by setting the timeScale to 1
	/// </summary>
	public void ResumeGame() {
		Time.timeScale = 1;
		IsPaused = false;
	}

	/// <summary>
	///     Stops the simulation by setting the timeScale to 0
	/// </summary>
	public void PauseGame() {
		Time.timeScale = 0;
		IsPaused = true;
	}

	/// <summary>
	///     Invoked when a tile is selected in the game world
	/// </summary>
	public event Action<TilePosition, MouseButton> OnTileSelected;
}
}
