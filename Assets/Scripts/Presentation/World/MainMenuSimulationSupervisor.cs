using System.Collections;
using System.Linq;
using Presentation.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Presentation.World {
public class MainMenuSimulationSupervisor : MonoBehaviour {
	private const string SimulationScenePath = "Scenes/Simulation";
	private const string SimulationSceneName = "Simulation";

	private IEnumerator _simulationCoroutine;

	private void Start() {
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.LoadScene(SimulationScenePath, LoadSceneMode.Additive);
	}

	private T FindObjectInRootObjects<T>(Scene scene) where T : MonoBehaviour {
		return scene.GetRootGameObjects()
			.Select(rootGameObject => rootGameObject.GetComponent<T>())
			.FirstOrDefault(component => component != null);
	}

	private void RemoveSimulationUI(Scene scene) {
		var simulationUI = FindObjectInRootObjects<SimulationUI>(scene);
		simulationUI.gameObject.SetActive(false);
	}

	private void MoveWorld(Scene scene) {
		var simulationCamera = FindObjectInRootObjects<SimulationCamera>(scene);

		simulationCamera.transform.position = new Vector3(-10, -2, -10);
		simulationCamera.GetComponent<Camera>().orthographicSize = 15;
	}

	private IEnumerator StartSimulation(Scene scene) {
		RemoveSimulationUI(scene);
		MoveWorld(scene);
		yield return new WaitForEndOfFrame();
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (scene.name != SimulationSceneName || mode != LoadSceneMode.Additive) return;
		if (_simulationCoroutine != null) Debug.LogError("The simulation is already running");
		_simulationCoroutine = StartSimulation(scene);
		StartCoroutine(_simulationCoroutine);
	}

	private void OnDestroy() {
		StopCoroutine(_simulationCoroutine);
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
}
}
