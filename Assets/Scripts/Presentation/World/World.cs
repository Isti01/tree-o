using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Data.World;
using Logic.Event.World.Tower;
using Logic.Event.World.Unit;
using UnityEngine;

namespace Presentation.World {
public class World : MonoBehaviour {
	public GameObject Tile;
	public GameObject Barrack;
	public GameObject Obstacle;
	public GameObject Tower;
	public GameObject Castle;
	public GameObject Unit;

	private GameObject[,] _map;
	private Dictionary<Logic.Data.World.Unit, Unit> _units;

	public T LogicToPresentation<T>(TileObject tileObject) where T : Structure
		=> _map[tileObject.Position.X, tileObject.Position.Y].GetComponentInChildren<T>();

	public Unit LogicToPresentation(Logic.Data.World.Unit unit) => _units[unit];

	private void Start() {
		var simulation = FindObjectOfType<SimulationManager>();

		_units = new Dictionary<Logic.Data.World.Unit, Unit>();

		simulation.GameOverview.Events.AddListener<TowerBuiltEvent>(OnTowerBuilt);
		simulation.GameOverview.Events.AddListener<TowerShotEvent>(OnTowerShot);
		simulation.GameOverview.Events.AddListener<TowerCooledDownEvent>(OnTowerCooledDown);

		simulation.GameOverview.Events.AddListener<UnitDeployedEvent>(OnUnitDeployed);
		simulation.GameOverview.Events.AddListener<UnitMovedTileEvent>(OnUnitMovedTile);
		simulation.GameOverview.Events.AddListener<UnitDestroyedEvent>(OnUnitDestroyed);

		GameWorld world = simulation.GameOverview.World;
		transform.position = new Vector3(-world.Width / 2.0f, -world.Width / 2.0f, 0);
		_map = new GameObject[world.Width, world.Height];

		for (var x = 0; x < world.Width; x++) {
			for (var y = 0; y < world.Height; y++) InstantiateTile(x, y, world);
		}
	}

	private void Update() {}

	private void OnUnitDeployed(UnitDeployedEvent e) {
		try {
			Logic.Data.World.Unit unitData = e.Unit;
			GameObject unit = Instantiate(Unit, gameObject.transform);
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

	private void OnUnitDestroyed(UnitDestroyedEvent e) {
		if (_units.TryGetValue(e.Unit, out Unit unit))
			unit.DestroyUnit();
		else
			Debug.LogError($"Failed to retrieve the unit {e.Unit}");
	}

	private void OnTowerBuilt(TowerBuiltEvent e) {
		Logic.Data.World.Tower tower = e.Tower;
		TilePosition tilePosition = tower.Position;

		InstantiateTower(_map[tilePosition.X, tilePosition.Y], tower);
	}

	private void OnTowerShot(TowerShotEvent e) {
		Tower tower = LogicToPresentation<Tower>(e.Tower);
		LineRenderer laserRenderer = tower.GetComponent<LineRenderer>();
		Transform target = LogicToPresentation(e.Target).transform;

		Vector3[] positions = { tower.transform.position, target.position };
		laserRenderer.SetPositions(positions);
		laserRenderer.enabled = true;

		IEnumerator LaserUpdater() {
			var remainingTime = 0.15f;
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
		Tower tower = LogicToPresentation<Tower>(e.Tower);
		//Disable the laser renderer when the tower is ready to shoot again:
		// we want to avoid activating a new laser pulse and then deactivating the old one
		tower.GetComponent<LineRenderer>().enabled = false;
	}

	private GameObject InstantiateTower(GameObject parent, Logic.Data.World.Tower tower) {
		GameObject structure = Instantiate(Tower, parent.transform);
		var barrackComponent = structure.GetComponent<Tower>();
		barrackComponent.SetData(tower);
		return structure;
	}

	private GameObject InstantiateTile(int x, int y, GameWorld world) {
		GameObject tile = Instantiate(Tile, transform);
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
				GameObject structure = Instantiate(Barrack, parent.transform);
				var barrackComponent = structure.GetComponent<Barrack>();
				barrackComponent.SetData(barrack);
				return structure;
			}
			case Logic.Data.World.Castle castle: {
				GameObject structure = Instantiate(Castle, parent.transform);
				var barrackComponent = structure.GetComponent<Castle>();
				barrackComponent.SetData(castle);
				return structure;
			}
			case Logic.Data.World.Tower tower: {
				GameObject structure = InstantiateTower(parent, tower);
				return structure;
			}
			case Logic.Data.World.Obstacle obstacle: {
				GameObject structure = Instantiate(Obstacle, parent.transform);
				return structure;
			}
			default:
				throw new Exception("Unhandled TileObject Type");
		}
	}
}
}
