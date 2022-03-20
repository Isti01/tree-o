using System;
using Logic.Command;
using Logic.Data;
using UnityEngine;
using Vector2 = Logic.Data.World.Vector2;

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
		if (tile != null) OnTileSelected?.Invoke(tile.Position);
	}

	private void FixedUpdate() {
		GameOverview?.Commands?.Issue(new AdvanceTimeCommand(GameOverview, Time.fixedDeltaTime));
	}

	public Projectile SpawnProjectile(Tower tower) {
		throw new NotImplementedException();
	}

	private Tile GetClickedTile() {
		if (!Input.GetMouseButtonDown(0)) return null;
		Vector3 mousePosition = Input.mousePosition;
		RaycastHit2D result = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(mousePosition), UnityEngine.Vector2.zero);

		return !result ? null : result.collider.gameObject.GetComponent<Tile>();
	}

	public event Action<Vector2> OnTileSelected;
}
}
