using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Presentation.UI {
[RequireComponent(typeof(UIDocument))]
public class PauseOverlay : MonoBehaviour {
	private const string ResumeButton = "ResumeButton";
	private const string NewGameButton = "NewGameButton";
	private const string ExitButton = "ExitButton";

	[SerializeField]
	private UIDocument ui;

	private VisualElement RootElement => ui.rootVisualElement;

	private void Start() {
		RootElement.Q<Button>(ResumeButton).clicked += () => OnResumeClicked?.Invoke();
		RootElement.Q<Button>(NewGameButton).clicked += () => OnNewGameClicked?.Invoke();
		RootElement.Q<Button>(ExitButton).clicked += () => OnExitClicked?.Invoke();
	}

	/// <summary>
	/// Shows the pause overlay
	/// </summary>
	public void Show() {
		RootElement.style.display = DisplayStyle.Flex;
	}

	/// <summary>
	/// Hides the pause overlay
	/// </summary>
	public void Hide() {
		RootElement.style.display = DisplayStyle.None;
	}

	/// <summary>
	/// Invoked when the resume button is clicked
	/// </summary>
	public event Action OnResumeClicked;

	/// <summary>
	/// Invoked when the new game button is clicked
	/// </summary>
	public event Action OnNewGameClicked;

	/// <summary>
	/// Invoked when the exit button is clicked
	/// </summary>
	public event Action OnExitClicked;
}
}
