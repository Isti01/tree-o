using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Data;
using Logic.Data.World;
using Logic.Event;
using Logic.Event.World.Barrack;
using Logic.Event.World.Castle;
using Logic.Event.World.Tower;
using Logic.Event.World.Unit;
using Presentation.UI;
using UnityEngine;

namespace Presentation.World {
public class World : MonoBehaviour {
	[SerializeField]
	private GameObject tilePrefab;

	[SerializeField]
	private GameObject barrackPrefab;

	[SerializeField]
	private GameObject obstaclePrefab;

	[SerializeField]
	private GameObject towerPrefab;

	[SerializeField]
	private GameObject castlePrefab;

	[SerializeField]
	private GameObject unitPrefab;

	[SerializeField]
	private GameObject tileHighlightPrefab;

	[SerializeField]
	private GameObject towerRadiusHighlightPrefab;

	private GameObject[,] _map;
	private GameTeam _buildingPossibleTeam;
	private Logic.Data.World.Barrack _selectedBarrack;
	private Logic.Data.World.Tower _selectedTower;
	private GameObject _selectedTowerRadiusHighlight;
	private SimulationManager _simulationManager;

	private Dictionary<TilePosition, GameObject> _tileHighlights;
	private Dictionary<Logic.Data.World.Unit, Unit> _units;

	private IGameOverview GameOverview => _simulationManager.GameOverview;

	private void Start() {
		_simulationManager = FindObjectOfType<SimulationManager>();

		_tileHighlights = new Dictionary<TilePosition, GameObject>();
		_units = new Dictionary<Logic.Data.World.Unit, Unit>();

		EventDispatcher events = _simulationManager.GameOverview.Events;
		events.AddListener<TowerBuiltEvent>(OnTowerBuilt);
		events.AddListener<TowerShotEvent>(OnTowerShot);
		events.AddListener<TowerCooledDownEvent>(OnTowerCooledDown);
		events.AddListener<TowerDestroyedEvent>(OnTowerDestroyed);
		events.AddListener<TowerUpgradedEvent>(OnTowerUpgraded);

		events.AddListener<UnitDeployedEvent>(OnUnitDeployed);
		events.AddListener<UnitMovedTileEvent>(OnUnitMovedTile);
		events.AddListener<UnitDamagedEvent>(OnUnitDamaged);
		events.AddListener<UnitDestroyedEvent>(OnUnitDestroyed);

		events.AddListener<BarrackCheckpointCreatedEvent>(OnBarrackCheckpointCreated);
		events.AddListener<BarrackCheckpointRemovedEvent>(OnBarrackCheckpointRemoved);

		events.AddListener<CastleDestroyedEvent>(OnCastleDestroyed);

		var simulationUI = FindObjectOfType<SimulationUI>();
		simulationUI.OnBuildingPossibleChanges += OnBuildingPossibleChanges;
		simulationUI.OnBarrackSelected += OnBarrackSelected;
		simulationUI.OnTowerSelected += OnTowerSelected;

		GameWorld world = GameOverview.World;
		transform.position = new Vector3(-world.Width / 2.0f, -world.Width / 2.0f, 0);
		_map = new GameObject[world.Width, world.Height];

		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) InstantiateTile(x, y, world);
		}
	}

	private void OnCastleDestroyed(CastleDestroyedEvent e) {
		var castle = LogicToPresentation<Castle>(e.Castle);
		castle.SetDestroyed();
	}

	private void OnTowerSelected(Logic.Data.World.Tower tower) {
		if (_selectedTower == tower) return;
		_selectedTower = tower;

		HighlightTowerRadius(tower);
	}

	private void HighlightTowerRadius(Logic.Data.World.Tower tower) {
		if (_selectedTowerRadiusHighlight != null) {
			Destroy(_selectedTowerRadiusHighlight);
		}

		if (tower == null) return;

		var position = tower.Position;
		var parentTile = _map[position.X, position.Y];
		_selectedTowerRadiusHighlight = Instantiate(towerRadiusHighlightPrefab, parentTile.transform);
		_selectedTowerRadiusHighlight.GetComponent<TileHighlight>().SetRadius(tower.Type.Range);
	}

	private void OnBarrackCheckpointRemoved(BarrackCheckpointRemovedEvent e) {
		if (e.Barrack == _selectedBarrack) VisualizeBarrackCheckpoints();
	}

	private void OnBarrackCheckpointCreated(BarrackCheckpointCreatedEvent e) {
		if (e.Barrack == _selectedBarrack) VisualizeBarrackCheckpoints();
	}

	private void OnBarrackSelected(Logic.Data.World.Barrack barrack) {
		if (_selectedBarrack == barrack) return;

		_selectedBarrack = barrack;
		VisualizeBarrackCheckpoints();
	}

	private void VisualizeBarrackCheckpoints() {
		RemoveHighlights();
		if (_selectedBarrack == null) return;

		HighlightTile(_selectedBarrack.Position);

		float checkpointCount = _selectedBarrack.CheckPoints.Count;
		int current = 1;
		foreach (TilePosition position in _selectedBarrack.CheckPoints) {
			var highlight = HighlightTile(position);
			highlight.ScaleIntensity(current / checkpointCount);
			current++;
		}
	}

	private void OnBuildingPossibleChanges(GameTeam team) {
		if (_buildingPossibleTeam == team) return;
		_buildingPossibleTeam = team;
		VisualizeValidTowerPositions();
	}

	private void VisualizeValidTowerPositions() {
		RemoveHighlights();
		if (_buildingPossibleTeam == null) return;

		foreach (TilePosition position in _buildingPossibleTeam.AvailableTowerPositions) {
			var tile = HighlightTile(position);
			tile.SetDimmed();
		}
	}

	/// <returns>Returns the <see cref="Structure"/> on the given coordinates</returns>
	public T LogicToPresentation<T>(TileObject tileObject) where T : Structure {
		return _map[tileObject.Position.X, tileObject.Position.Y].GetComponentInChildren<T>();
	}

	/// <returns>Returns the <see cref="Unit"/> on the given coordinates</returns>
	public Unit LogicToPresentation(Logic.Data.World.Unit unit) {
		return _units[unit];
	}

	private void OnTowerUpgraded(TowerUpgradedEvent e) {
		if (_selectedTower == e.Tower) {
			HighlightTowerRadius(e.Tower);
		}

		var tower = LogicToPresentation<Tower>(e.Tower);
		tower.SetData(e.Tower);
		Debug.Log($"Tower Upgraded {e.Tower}");
	}

	private void OnTowerDestroyed(TowerDestroyedEvent e) {
		if (_selectedTower == e.Tower) {
			HighlightTowerRadius(null);
		}

		var tower = LogicToPresentation<Tower>(e.Tower);
		tower.DestroyTower();
		VisualizeValidTowerPositions();
		Debug.Log($"Removed Tower: {e.Tower}");
	}

	private TileHighlight HighlightTile(TilePosition pos) {
		GameObject highlight = Instantiate(tileHighlightPrefab, _map[pos.X, pos.Y].transform);
		_tileHighlights.Add(pos, highlight);
		return highlight.GetComponent<TileHighlight>();
	}

	private void RemoveHighlights() {
		foreach (GameObject obj in _tileHighlights.Values) Destroy(obj);

		_tileHighlights.Clear();
	}

	private void OnUnitDeployed(UnitDeployedEvent e) {
		try {
			Logic.Data.World.Unit unitData = e.Unit;
			GameObject unit = Instantiate(unitPrefab, gameObject.transform);
			unit.transform.localPosition = new Vector3(unitData.Position.X - 0.5f, unitData.Position.Y - 0.5f);
			var unitComponent = unit.GetComponent<Unit>();
			unitComponent.SetData(unitData);

			_units.Add(unitData, unitComponent);
		} catch (Exception ex) {
			Debug.Log($"{ex.Message}\n{ex.StackTrace}");
		}
	}

	private void OnUnitMovedTile(UnitMovedTileEvent e) {
		if (_units.TryGetValue(e.Unit, out Unit unit))
			unit.transform.localPosition = new Vector3(e.Unit.Position.X - 0.5f, e.Unit.Position.Y - 0.5f);
		else
			Debug.LogError($"Failed to retrieve the unit {e.Unit}");
	}

	private void OnUnitDamaged(UnitDamagedEvent e) {
		if (_units.TryGetValue(e.Unit, out Unit unit))
			unit.UpdateHealth();
		else
			Debug.LogError($"Failed to retrieve the unit {e.Unit}");
	}

	private void OnUnitDestroyed(UnitDestroyedEvent e) {
		if (_units.TryGetValue(e.Unit, out Unit unit)) {
			unit.DestroyUnit();
			_units.Remove(e.Unit);
		} else {
			Debug.LogError($"Failed to retrieve the unit {e.Unit}");
		}
	}

	private void OnTowerBuilt(TowerBuiltEvent e) {
		Logic.Data.World.Tower tower = e.Tower;
		TilePosition tilePosition = tower.Position;

		InstantiateTower(_map[tilePosition.X, tilePosition.Y], tower);
		VisualizeValidTowerPositions();
	}

	private void OnTowerShot(TowerShotEvent e) {
		var tower = LogicToPresentation<Tower>(e.Tower);
		var laserRenderer = tower.GetComponent<LineRenderer>();
		Transform target = LogicToPresentation(e.Target).transform;

		Vector3[] positions = { tower.transform.position, target.position };
		laserRenderer.SetPositions(positions);
		laserRenderer.enabled = true;

		IEnumerator LaserUpdater() {
			float remainingTime = 0.15f;
			while (remainingTime > 0) {
				yield return new WaitForFixedUpdate();
				remainingTime -= Time.fixedDeltaTime;

				if (e.Target.IsAlive) {
					positions[1] = target.position;
					laserRenderer.SetPositions(positions);
				}
			}

			laserRenderer.enabled = false;
		}

		StartCoroutine(LaserUpdater());
	}

	private void OnTowerCooledDown(TowerCooledDownEvent e) {
		var tower = LogicToPresentation<Tower>(e.Tower);
		// Disable the laser renderer when the tower is ready to shoot again:
		// we want to avoid activating a new laser pulse and then deactivating the old one
		tower.GetComponent<LineRenderer>().enabled = false;
	}

	private GameObject InstantiateTower(GameObject parent, Logic.Data.World.Tower tower) {
		GameObject structure = Instantiate(towerPrefab, parent.transform);
		var towerComponent = structure.GetComponent<Tower>();
		towerComponent.SetData(tower);
		return structure;
	}

	private GameObject InstantiateTile(int x, int y, GameWorld world) {
		GameObject tile = Instantiate(tilePrefab, transform);
		tile.transform.localPosition = new Vector3(x, y);
		var tileComponent = tile.GetComponent<Tile>();
		tileComponent.Position = new TilePosition(x, y);
		_map[x, y] = tile;

		TileObject data = world[x, y];
		if (data != null) InstantiateStructure(tile, data);

		return tile;
	}

	private GameObject InstantiateStructure(GameObject parent, TileObject tileData) {
		switch (tileData) {
			case Logic.Data.World.Barrack barrack: {
				GameObject structure = Instantiate(barrackPrefab, parent.transform);
				var barrackComponent = structure.GetComponent<Barrack>();
				barrackComponent.SetData(barrack);
				return structure;
			}
			case Logic.Data.World.Castle castle: {
				GameObject structure = Instantiate(castlePrefab, parent.transform);
				var barrackComponent = structure.GetComponent<Castle>();
				barrackComponent.SetData(castle);
				return structure;
			}
			case Logic.Data.World.Tower tower: {
				GameObject structure = InstantiateTower(parent, tower);
				return structure;
			}
			case Logic.Data.World.Obstacle _: {
				GameObject structure = Instantiate(obstaclePrefab, parent.transform);
				return structure;
			}
			default:
				throw new Exception("Unhandled TileObject Type");
		}
	}
}
}
