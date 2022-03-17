using System;
using Logic.Data;
using UnityEngine;

namespace Presentation.World {
public class SimulationManager : MonoBehaviour {
	public int WorldWidth = 10;
	public int WorldHeight = 10;
	public int Seed = 1337;
	public GameOverview GameOverview { get; private set; }

	private void Awake() {
		void ExceptionHandler(Exception e) {
			Debug.LogError($"[Logic Exception]: ${e.Message}\n${e.StackTrace}");
		}

		GameOverview = new GameOverview(ExceptionHandler, Seed, WorldWidth, WorldHeight);
	}

	public Projectile SpawnProjectile(Tower tower) {
		throw new NotImplementedException();
	}
}
}
