using System;
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

	public void UpdateMessage(Color winner) {
		string winnerText = winner == Color.Red ? "Red" : "Blue";
		RootElement.Q<Label>(Message).text = $"Player {winnerText} has won the game!";
	}

	public void Show() {
		RootElement.style.display = DisplayStyle.Flex;
	}

	public void Hide() {
		RootElement.style.display = DisplayStyle.None;
	}

	public event Action OnOkClicked;
}
}
