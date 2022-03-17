using UnityEngine;
using UnityEngine.SceneManagement;

namespace Presentation.World {
public class GameManager : MonoBehaviour {
	public const string MAIN_MENU_SCENE = "Scenes/MainMenu";
	public const string SIMULATION_SCENE = "Scenes/Simulation";

	public void LoadMenu() {
		SceneManager.LoadScene(MAIN_MENU_SCENE);
	}

	public void LoadNewGame() {
		SceneManager.LoadScene(SIMULATION_SCENE);
	}

	public void ExitGame() {
		Application.Quit();
	}
}
}
