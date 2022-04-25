using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Presentation.World {
public class MainMenuSimulationSupervisor : MonoBehaviour {
	private const string SimulationScenePath = "Scenes/Simulation";
	private const string SimulationSceneName = "Simulation";

	private void Start() {
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.LoadScene(SimulationScenePath, LoadSceneMode.Additive);
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (scene.name == SimulationSceneName && mode == LoadSceneMode.Additive) {
			Debug.Log("Simulation scene is loaded");
		}
	}

	private void OnDestroy() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
}
}
