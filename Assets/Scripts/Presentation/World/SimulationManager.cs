using System;
using Logic.Data;
using UnityEngine;

namespace Presentation.World {
public class SimulationManager : MonoBehaviour {
	public int WorldWidth = 10;
	public int WorldHeight = 10;
	public int Seed = 1337;

	[SerializeField]
	private Camera mainCamera;

	public GameOverview GameOverview { get; private set; }

	private void Awake() {
		void ExceptionHandler(Exception e) {
			Debug.LogError($"[Logic Exception]: ${e.Message}\n${e.StackTrace}");
		}

		GameOverview = new GameOverview(ExceptionHandler, Seed, WorldWidth, WorldHeight);
	}

	public void Update() {
		Tile tile = GetClickedTile();
		if (tile != null) Debug.Log(tile.Position);
	}

	public Projectile SpawnProjectile(Tower tower) {
		throw new NotImplementedException();
	}

	private Tile GetClickedTile() {
		if (!Input.GetMouseButtonDown(0)) return null;
		Vector3 mousePosition = Input.mousePosition;
		RaycastHit2D result = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero);

		return !result ? null : result.collider.gameObject.GetComponent<Tile>();
	}
}
}
