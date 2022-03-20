using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Presentation.UI {
public class PauseOverlay : MonoBehaviour {
	[SerializeField]
	private UIDocument ui;

	private VisualElement RootElement => ui.rootVisualElement;

	private void Start() {
		RootElement.Q<Button>("ResumeButton").clicked += () => OnResumeClicked?.Invoke();
		RootElement.Q<Button>("NewGameButton").clicked += () => OnNewGameClicked?.Invoke();
		RootElement.Q<Button>("ExitButton").clicked += () => OnExitClicked?.Invoke();
	}

	public void Show() {
		RootElement.style.display = DisplayStyle.Flex;
	}

	public void Hide() {
		RootElement.style.display = DisplayStyle.None;
	}

	public event Action OnResumeClicked;
	public event Action OnNewGameClicked;
	public event Action OnExitClicked;
}
}
