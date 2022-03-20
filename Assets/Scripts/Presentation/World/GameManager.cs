using UnityEngine;
using UnityEngine.SceneManagement;

namespace Presentation.World {
public class GameManager : MonoBehaviour {
	public const string MainMenuScene = "Scenes/MainMenu";
	public const string SimulationScene = "Scenes/Simulation";

	public void LoadMenu() {
		SceneManager.LoadScene(MainMenuScene);
	}

	public void LoadNewGame() {
		SceneManager.LoadScene(SimulationScene);
	}

	public void ExitGame() {
		Application.Quit();
	}
}
}
