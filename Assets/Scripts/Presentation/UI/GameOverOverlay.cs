using System;
using Logic.Data;
using UnityEngine;
using UnityEngine.UIElements;
using Color = Logic.Data.Color;

namespace Presentation.UI {
[RequireComponent(typeof(UIDocument))]
public class GameOverOverlay : MonoBehaviour {
	private const string OkButton = "OkButton";
	private const string Message = "Message";

	[SerializeField]
	private UIDocument ui;

	private VisualElement RootElement => ui.rootVisualElement;

	private void Start() {
		RootElement.Q<Button>(OkButton).clicked += () => OnOkClicked?.Invoke();
	}

	/// <summary>
	///     Displays the winner team's name on the screen
	/// </summary>
	public void UpdateMessage(GameTeam winner) {
		string text = "It's a tie!";
		if (winner != null) {
			string winnerText = winner.TeamColor == Color.Red ? "Red" : "Blue";
			text = $"Player {winnerText} has won the game!";
		}

		RootElement.Q<Label>(Message).text = text;
	}

	/// <summary>
	///     Shows the game over overlay
	/// </summary>
	public void Show() {
		RootElement.style.display = DisplayStyle.Flex;
	}

	/// <summary>
	///     Hides the game over overlay
	/// </summary>
	public void Hide() {
		RootElement.style.display = DisplayStyle.None;
	}

	/// <summary>
	///     Invoked when the ok button is clicked
	/// </summary>
	public event Action OnOkClicked;
}
}
