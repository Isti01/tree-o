using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Presentation.UI {
[RequireComponent(typeof(UIDocument))]
public class BattleUI : MonoBehaviour {
	private const string Pause = "Pause";
	private const string Exit = "Exit";

	[SerializeField]
	private UIDocument ui;

	private Button _exitButton;
	private Button _pauseButton;

	private VisualElement RootElement => ui.rootVisualElement;

	private void Start() {
		_pauseButton = RootElement.Q<Button>(Pause);
		_pauseButton.clicked += () => OnPauseClicked?.Invoke();

		_exitButton = RootElement.Q<Button>(Exit);
		_exitButton.clicked += () => OnExitClicked?.Invoke();

		ShowPauseButton();
	}

	public void ShowPauseButton() {
		_pauseButton.style.display = DisplayStyle.Flex;
		_exitButton.style.display = DisplayStyle.None;
	}

	public void ShowExitButton() {
		_pauseButton.style.display = DisplayStyle.None;
		_exitButton.style.display = DisplayStyle.Flex;
	}

	public void Show() {
		RootElement.style.display = DisplayStyle.Flex;
	}

	public void Hide() {
		RootElement.style.display = DisplayStyle.None;
	}

	public event Action OnExitClicked;
	public event Action OnPauseClicked;
}
}
