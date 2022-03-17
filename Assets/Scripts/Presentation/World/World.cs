using System;
using System.Collections.Generic;
using Logic.Data.World;
using UnityEngine;
using Vector2 = Logic.Data.World.Vector2;

namespace Presentation.World {
public class World : MonoBehaviour {
	public List<Unit> Units;

	public GameObject Tile;
	public GameObject Barrack;
	public GameObject Obstacle;
	public GameObject Tower;
	public GameObject Castle;

	public float TilePadding = 0.1f;
	private GameObject[,] _map;

	private void Start() {
		var simulation = FindObjectOfType<SimulationManager>();

		GameWorld world = simulation.GameOverview.World;
		_map = new GameObject[world.Width, world.Height];

		for (var i = 0; i < world.Width; i++) {
			for (var j = 0; j < world.Height; j++) InstantiateTile(i, j, world);
		}
	}

	private void Update() {}

	private GameObject InstantiateTile(int col, int row, GameWorld world) {
		float sizeMultiplier = TilePadding + 1.0f;

		float x = col - world.Width / 2.0f + 0.5f;
		float y = row - world.Height / 2.0f + 0.5f;

		Vector3 position = new Vector3(x, y) * sizeMultiplier;

		GameObject tile = Instantiate(Tile, position, Quaternion.identity, transform);
		var tileComponent = tile.GetComponent<Tile>();
		tileComponent.Position = new Vector2(col, row);
		_map[col, row] = tile;

		TileObject data = world[col, row];
		if (data != null) InstantiateStructure(tile, position, data);

		return tile;
	}

	private GameObject InstantiateStructure(GameObject parent, Vector3 position, TileObject tileData) {
		switch (tileData) {
			case Logic.Data.World.Barrack barrack: {
				GameObject structure = Instantiate(Barrack, position, Quaternion.identity, parent.transform);
				var barrackComponent = structure.GetComponent<Barrack>();
				barrackComponent.SetData(barrack);
				return structure;
			}
			case Logic.Data.World.Castle castle: {
				GameObject structure = Instantiate(Castle, position, Quaternion.identity, parent.transform);
				var barrackComponent = structure.GetComponent<Castle>();
				barrackComponent.SetData(castle);
				return structure;
			}
			case Logic.Data.World.Tower tower: {
				GameObject structure = Instantiate(Tower, position, Quaternion.identity, parent.transform);
				var barrackComponent = structure.GetComponent<Tower>();
				barrackComponent.SetData(tower);
				return structure;
			}
			case Logic.Data.World.Obstacle obstacle: {
				GameObject structure = Instantiate(Obstacle, position, Quaternion.identity, parent.transform);
				return structure;
			}
			default:
				throw new Exception("Unhandled TileObject Type");
		}
	}
}
}
