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
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace Presentation.World {
public class MainMenuSimulationSupervisor : MonoBehaviour {
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
	private List<TowerData> towerTypes;

	[SerializeField]
	private List<UnitData> unitTypes;

	[SerializeField]
	private SimulationManager simulationManager;

	private readonly List<IUnitData> _modifiedUnitTypes = new List<IUnitData>();
	private IEnumerator _simulationCoroutine;

	private void Start() {
		// we don't want to destroy castles
		_modifiedUnitTypes.AddRange(unitTypes.Select(unit => new NoDamageUnitDataProxy(unit)));

		_simulationCoroutine = StartSimulation();
		StartCoroutine(_simulationCoroutine);
	}

	private void OnDestroy() {
		if (_simulationCoroutine != null) StopCoroutine(_simulationCoroutine);
	}

	private IEnumerator StartSimulation() {
		IGameOverview overview = simulationManager.GameOverview;
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

	private class NoDamageUnitDataProxy : IUnitData {
		private readonly IUnitData source;

		public NoDamageUnitDataProxy(IUnitData source) {
			this.source = source;
		}

		public float Damage => 0; //Only this property is changed
		public string Name => source.Name;
		public float Health => source.Health;
		public float Speed => source.Speed;
		public int Cost => source.Cost;
		public Color BlueColor => source.BlueColor;
		public Color RedColor => source.RedColor;
		public Sprite PreviewSprite => source.PreviewSprite;
		public Sprite AliveSpriteConstant => source.AliveSpriteConstant;
		public Sprite AliveSpriteColored => source.AliveSpriteColored;
		public bool Airborne => source.Airborne;
	}
}
}
