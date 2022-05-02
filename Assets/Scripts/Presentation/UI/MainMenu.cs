using Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace Presentation.UI {
[RequireComponent(typeof(UIDocument))]
public class MainMenu : MonoBehaviour {
	private const string NewGameButton = "NewGameButton";
	private const string ExitButton = "ExitButton";
	private const string ReturnButton = "ReturnButton";
	private const string TextContent = "TextContent";
	private const string RulesButton = "RulesButton";
	private const string MainContent = "MainContent";
	private const string GameDescription = "GameDescription";

	private const string TitleStyle = "title-style";
	private const string ParagraphStyle = "paragraph-style";

	[SerializeField]
	[TextArea]
	private string gameRules;

	private UIDocument _uiDocument;
	private VisualElement RootElement => _uiDocument.rootVisualElement;

	private void Start() {
		_uiDocument = GetComponent<UIDocument>();

		RootElement.Q<Button>(NewGameButton).clicked += OnNewGameClicked;
		RootElement.Q<Button>(ExitButton).clicked += OnExitClicked;
		RootElement.Q<Button>(RulesButton).clicked += OnRulesClicked;
		RootElement.Q<Button>(ReturnButton).clicked += ShowMainContent;
		ShowMainContent();
	}

	private void ShowMainContent() {
		RootElement.Q(GameDescription).style.display = DisplayStyle.None;
		RootElement.Q(MainContent).style.display = DisplayStyle.Flex;
	}

	private void OnRulesClicked() {
		RootElement.Q(GameDescription).style.display = DisplayStyle.Flex;
		RootElement.Q(MainContent).style.display = DisplayStyle.None;

		var textContent = RootElement.Q(TextContent);
		textContent.Clear();
		foreach (string entry in gameRules.Split('\n')) {
			bool isTitle = entry.StartsWith("#");
			var label = new Label() { text = isTitle ? entry.Substring(1) : entry };
			label.AddToClassList(isTitle ? TitleStyle : ParagraphStyle);
			textContent.Add(label);
		}
	}

	private void OnNewGameClicked() {
		FindObjectOfType<GameManager>().LoadNewGame();
	}

	private void OnExitClicked() {
		FindObjectOfType<GameManager>().ExitGame();
	}
}
}
