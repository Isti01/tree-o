using Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace Presentation.UI {
public class MainMenu : MonoBehaviour {
	private void Start() {
		var document = GetComponent<UIDocument>();
		var root = document.rootVisualElement;

		var newGameButton = root.Q<Button>("NewGameButton");
		var exitButton = root.Q<Button>("ExitButton");

		newGameButton.clicked += OnNewGameClicked;
		exitButton.clicked += OnExitClicked;
	}

	private void OnNewGameClicked() {
		FindObjectOfType<GameManager>().LoadNewGame();
	}

	private void OnExitClicked() {
		FindObjectOfType<GameManager>().ExitGame();
	}
}
}
