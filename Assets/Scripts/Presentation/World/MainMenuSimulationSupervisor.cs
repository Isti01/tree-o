using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		_simulationCoroutine = null;
		// we don't want to destroy castles
		foreach (UnitTypeData unitType in unitTypes) {
			UnitTypeData modifiedUnitType = Instantiate(unitType);
			FieldInfo prop = modifiedUnitType.GetType().GetField("damage",
				BindingFlags.NonPublic | BindingFlags.Instance);
			Debug.Assert(prop != null);
			if (prop == null) continue;
			prop.SetValue(modifiedUnitType, 0.0f);
			_modifiedUnitTypes.Add(modifiedUnitType);
		}

		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.LoadScene(SimulationScenePath, LoadSceneMode.Additive);
	}

	private void OnDestroy() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
		if (_simulationCoroutine != null) StopCoroutine(_simulationCoroutine);
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

		var manager = FindObjectInRootObjects<SimulationManager>(scene);
		yield return new WaitUntil(() => manager.GameOverview != null);

		IGameOverview overview = manager.GameOverview;
		if (overview.CurrentPhase == GamePhase.Prepare) yield return PreparePhase(overview);

		overview.Events.AddListener<PhaseAdvancedEvent>(OnPhaseAdvanced);
	}

	private void OnPhaseAdvanced(PhaseAdvancedEvent e) {
		IGameOverview overview = e.Overview;
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
		foreach (GameTeam team in overview.Teams) {
			yield return DestroyTowers(team);
			yield return PlaceTowers(team);
			yield return UpgradeTowers(team);
			TrainUnits(team);
		}

		overview.Commands.Issue(new AdvancePhaseCommand(overview));
	}

	private void TrainUnits(GameTeam team) {
		int unitDeploymentAttempts = Random.Range(minUnitDeploymentAttempts, maxUnitDeploymentAttempts);
		for (int i = 0; i < unitDeploymentAttempts; i++) {
			int index = Random.Range(0, _modifiedUnitTypes.Count);
			team.Overview.Commands.Issue(new PurchaseUnitCommand(team, _modifiedUnitTypes[index]));
		}
	}

	private IEnumerator UpgradeTowers(GameTeam team) {
		int towerUpgradeAttempts = Random.Range(minTowerUpgradeAttempts, maxTowerUpgradeAttempts);
		for (int i = 0; i < towerUpgradeAttempts; i++) {
			Logic.Data.World.Tower tower = team.Towers.OrderBy(e => Random.value - .5f)
				.FirstOrDefault(_ => Random.value > towerUpgradeProbability);
			if (tower == null) continue;
			if (team.Overview.Commands.Issue(new UpgradeTowerCommand(tower)))
				yield return new WaitForSeconds(towerPlacingDelay);
		}
	}

	private IEnumerator PlaceTowers(GameTeam team) {
		int width = team.Overview.World.Width;
		int height = team.Overview.World.Height;

		int towerPlacingAttempts = Random.Range(minTowerPlacingAttempts, maxTowerPlacingAttempts);
		for (int i = 0; i < towerPlacingAttempts; i++) {
			int x = Random.Range(0, width);
			int y = Random.Range(0, height);
			int index = Random.Range(0, towerTypes.Count);
			var position = new TilePosition(x, y);
			if (team.Overview.Commands.Issue(new BuildTowerCommand(team, towerTypes[index], position)))
				yield return new WaitForSeconds(towerPlacingDelay);
		}
	}

	private IEnumerator DestroyTowers(GameTeam team) {
		int towerDestroyAttempts = Random.Range(minTowerDestroyAttempts, maxTowerDestroyAttempts);
		for (int i = 0; i < towerDestroyAttempts; i++) {
			Logic.Data.World.Tower tower = team.Towers.OrderBy(e => Random.value - .5f)
				.FirstOrDefault(_ => Random.value > towerDestroyProbability);
			if (tower == null) continue;
			team.Overview.Commands.Issue(new DestroyTowerCommand(tower));
			yield return new WaitForSeconds(towerPlacingDelay);
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (scene.name != SimulationSceneName || mode != LoadSceneMode.Additive) return;
		if (_simulationCoroutine != null) Debug.LogError("The simulation is already running");
		_simulationCoroutine = StartSimulation(scene);
		StartCoroutine(_simulationCoroutine);
	}
}
}
