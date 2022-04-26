using UnityEngine;
using UnityEngine.SceneManagement;

namespace Presentation.World {
public class GameManager : MonoBehaviour {
	private const string MainMenuScene = "Scenes/MainMenu";
	private const string SimulationScene = "Scenes/Simulation";

	/// <summary>
	///     Loads the "MainMenu" scene
	/// </summary>
	public void LoadMenu() {
		SceneManager.LoadScene(MainMenuScene);
	}

	/// <summary>
	///     Loads the "Simulation" scene
	/// </summary>
	public void LoadNewGame() {
		SceneManager.LoadScene(SimulationScene);
	}

	/// <summary>
	///     Closes the game
	/// </summary>
	public void ExitGame() {
		Application.Quit();
	}
}
}
