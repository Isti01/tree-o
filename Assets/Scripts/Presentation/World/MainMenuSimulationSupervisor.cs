using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic.Command;
using Logic.Command.Tower;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;
using Logic.Event;
using Presentation.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Presentation.World {
public class MainMenuSimulationSupervisor : MonoBehaviour {
	private const string SimulationScenePath = "Scenes/Simulation";
	private const string SimulationSceneName = "Simulation";

	[SerializeField]
	private int minTowerPlacingAttempts = 5;

	[SerializeField]
	private int maxTowerPlacingAttempts = 10;

	[SerializeField]
	private int minTowerDestroyAttempts = 2;

	[SerializeField]
	private int maxTowerDestroyAttempts = 5;

	[SerializeField]
	private float towerDestroyProbability = .4f;

	[SerializeField]
	private int minTowerUpgradeAttempts = 2;

	[SerializeField]
	private int maxTowerUpgradeAttempts = 5;

	[SerializeField]
	private float towerUpgradeProbability = .4f;

	[SerializeField]
	private int minUnitDeploymentAttempts = 3;

	[SerializeField]
	private int maxUnitDeploymentAttempts = 10;

	[SerializeField]
	private float towerPlacingDelay = .125f;

	[SerializeField]
	private List<TowerTypeData> towerTypes;

	[SerializeField]
	private List<UnitTypeData> unitTypes;

	private readonly List<UnitTypeData> _modifiedUnitTypes = new List<UnitTypeData>();
	private IEnumerator _simulationCoroutine;

	private void Start() {
		// we don't want to destroy castles
		foreach (UnitTypeData unitType in unitTypes) {
			var modifiedUnitType = Instantiate(unitType);
			var prop = modifiedUnitType.GetType().GetField("damage",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			Debug.Assert(prop != null);
			if (prop == null) continue;
			prop.SetValue(modifiedUnitType, 0.0f);
			_modifiedUnitTypes.Add(modifiedUnitType);
		}

		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.LoadScene(SimulationScenePath, LoadSceneMode.Additive);
	}

	private static T FindObjectInRootObjects<T>(Scene scene) where T : MonoBehaviour {
		return scene.GetRootGameObjects()
			.Select(rootGameObject => rootGameObject.GetComponent<T>())
			.FirstOrDefault(component => component != null);
	}

	private static void RemoveSimulationUI(Scene scene) {
		var simulationUI = FindObjectInRootObjects<SimulationUI>(scene);
		simulationUI.gameObject.SetActive(false);
	}

	private static void MoveWorld(Scene scene) {
		var simulationCamera = FindObjectInRootObjects<SimulationCamera>(scene);

		simulationCamera.transform.position = new Vector3(-10, -2, -10);
		simulationCamera.GetComponent<Camera>().orthographicSize = 15;
	}

	private IEnumerator StartSimulation(Scene scene) {
		RemoveSimulationUI(scene);
		MoveWorld(scene);

		SimulationManager manager = FindObjectInRootObjects<SimulationManager>(scene);
		yield return new WaitUntil(() => manager.GameOverview != null);

		IGameOverview overview = manager.GameOverview;
		if (overview.CurrentPhase == GamePhase.Prepare) {
			yield return PreparePhase(overview);
		}

		overview.Events.AddListener<PhaseAdvancedEvent>(OnPhaseAdvanced);
	}

	private void OnPhaseAdvanced(PhaseAdvancedEvent e) {
		var overview = e.Overview;
		switch (overview.CurrentPhase) {
			case GamePhase.Prepare:
				StartCoroutine(PreparePhase(overview));
				break;
			case GamePhase.Finished:
				overview.Commands.Issue(new AdvancePhaseCommand(overview));
				break;
			case GamePhase.Fight:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private IEnumerator PreparePhase(IGameOverview overview) {
		CommandDispatcher commands = overview.Commands;
		var world = overview.World;
		int width = world.Width;
		int height = world.Height;

		foreach (GameTeam team in overview.Teams) {
			int towerDestroyAttempts = Random.Range(minTowerDestroyAttempts, maxTowerDestroyAttempts);
			for (int i = 0; i < towerDestroyAttempts; i++) {
				var tower = team.Towers.OrderBy(e => Random.value - .5f)
					.FirstOrDefault(_ => Random.value > towerDestroyProbability);
				if (tower == null) continue;
				commands.Issue(new DestroyTowerCommand(tower));
				yield return new WaitForSeconds(towerPlacingDelay);
			}

			int towerPlacingAttempts = Random.Range(minTowerPlacingAttempts, maxTowerPlacingAttempts);
			for (int i = 0; i < towerPlacingAttempts; i++) {
				int x = Random.Range(0, width);
				int y = Random.Range(0, height);
				int index = Random.Range(0, towerTypes.Count);
				if (commands.Issue(new BuildTowerCommand(team, towerTypes[index], new TilePosition(x, y)))) {
					yield return new WaitForSeconds(towerPlacingDelay);
				}
			}

			int towerUpgradeAttempts = Random.Range(minTowerUpgradeAttempts, maxTowerUpgradeAttempts);
			for (int i = 0; i < towerUpgradeAttempts; i++) {
				var tower = team.Towers.OrderBy(e => Random.value - .5f)
					.FirstOrDefault(_ => Random.value > towerUpgradeProbability);
				if (tower == null) continue;
				commands.Issue(new UpgradeTowerCommand(tower));
				yield return new WaitForSeconds(towerPlacingDelay);
			}

			int unitDeploymentAttempts = Random.Range(minUnitDeploymentAttempts, maxUnitDeploymentAttempts);
			for (int i = 0; i < unitDeploymentAttempts; i++) {
				int index = Random.Range(0, _modifiedUnitTypes.Count);
				commands.Issue(new PurchaseUnitCommand(team, _modifiedUnitTypes[index]));
			}
		}

		commands.Issue(new AdvancePhaseCommand(overview));
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (scene.name != SimulationSceneName || mode != LoadSceneMode.Additive) return;
		if (_simulationCoroutine != null) Debug.LogError("The simulation is already running");
		_simulationCoroutine = StartSimulation(scene);
		StartCoroutine(_simulationCoroutine);
	}
}
}
