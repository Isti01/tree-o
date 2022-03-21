using System;
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

	public float TilePadding = 0.1f;

	private GameObject[,] _map;
	private Dictionary<Logic.Data.World.Unit, Unit> _units;

	private void Start() {
		var simulation = FindObjectOfType<SimulationManager>();

		_units = new Dictionary<Logic.Data.World.Unit, Unit>();

		simulation.GameOverview.Events.AddListener<TowerBuiltEvent>(OnTowerBuilt);

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
			unit.transform.localPosition = new Vector3(unitData.Position.X, unitData.Position.Y);
			var unitComponent = unit.GetComponent<Unit>();
			unitComponent.SetData(unitData);

			_units.Add(unitData, unitComponent);
		} catch (Exception ex) {
			Debug.Log($"{ex.Message}\n{ex.StackTrace}");
		}
	}

	private void OnUnitMovedTile(UnitMovedTileEvent e) {
		if (_units.TryGetValue(e.Unit, out Unit unit))
			unit.transform.localPosition = new Vector3(e.Unit.Position.X, e.Unit.Position.Y);
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

	private GameObject InstantiateTower(GameObject parent, Logic.Data.World.Tower tower) {
		GameObject structure = Instantiate(Tower, parent.transform);
		var barrackComponent = structure.GetComponent<Tower>();
		barrackComponent.SetData(tower);
		return structure;
	}

	private GameObject InstantiateTile(int x, int y, GameWorld world) {
		float sizeMultiplier = TilePadding + 1.0f;

		Vector3 position = new Vector3(x, y) * sizeMultiplier;

		GameObject tile = Instantiate(Tile, transform);
		tile.transform.localPosition = position;
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
