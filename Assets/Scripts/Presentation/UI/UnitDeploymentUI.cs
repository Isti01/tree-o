using System;
using System.Collections.Generic;
using Logic.Data;
using Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;
using Color = Logic.Data.Color;

namespace Presentation.UI {
[RequireComponent(typeof(UIDocument))]
public class UnitDeploymentUI : MonoBehaviour {
	private const string PlayerNameTurn = "PlayerNameTurn";
	private const string NextButton = "Next";
	private const string TopPanel = "TopPanel";
	private const string BottomPanel = "BottomPanel";
	private const string BudgetText = "BudgetText";
	private const string TopChipText = "TopChipText";
	private const string CardContent = "CardContent";
	private const string BottomChipText = "BottomChipText";
	private const string DeployedUnitsContainer = "DeployedUnitsContainer";

	[SerializeField]
	private VisualTreeAsset cardComponent;

	[SerializeField]
	private UIDocument ui;

	[SerializeField]
	private List<UnitData> unitTypes;

	private readonly Dictionary<IUnitData, VisualElement> _unitCards = new Dictionary<IUnitData, VisualElement>();

	private Color _activePlayer = Color.Red;
	private UnityEngine.Color _teamBlueColor = UnityEngine.Color.magenta;
	private UnityEngine.Color _teamRedColor = UnityEngine.Color.green;

	private VisualElement RootElement => ui.rootVisualElement;

	private void Start() {
		SetupCards();
		RootElement.Q<Button>(NextButton).clicked += () => OnNextClicked?.Invoke();
	}

	/// <summary>
	///     Sets the colors that should be displayed on each team's turn
	/// </summary>
	public void SetTeamColors(UnityEngine.Color teamRedColor, UnityEngine.Color teamBlueColor) {
		_teamRedColor = teamRedColor;
		_teamBlueColor = teamBlueColor;
	}

	private void SetupCards() {
		_unitCards.Clear();
		var cardList = RootElement.Q<VisualElement>(BottomPanel);
		cardList.Clear();

		foreach (IUnitData unitType in unitTypes) {
			IUnitData unit = unitType;
			TemplateContainer card = cardComponent.Instantiate();
			var content = card.Q<VisualElement>(CardContent);
			content.style.backgroundImage = new StyleBackground(unitType.PreviewSprite);

			card.userData = 0;
			UpdateCardUnitCount(card, 0);

			var bottomLabel = card.Q<Label>(BottomChipText);
			bottomLabel.text = $"Cost: {unitType.Cost}";

			card.RegisterCallback<ClickEvent>(e => {
				if (e.button == 0) OnUnitPurchased?.Invoke(unit);
			});

			_unitCards.Add(unitType, card);
			cardList.Add(card);
		}
	}

	/// <summary>
	///     Increments the bought unit count of a given type
	/// </summary>
	public void UpdateBoughtUnitCount(IUnitData unit) {
		if (_unitCards.TryGetValue(unit, out VisualElement card))
			UpdateCardUnitCount(card, 1);
		else
			Debug.LogError($"Cannot retrieve unit type card {unit}");
	}

	private static void UpdateCardUnitCount(VisualElement card, int newUnits) {
		newUnits += (int) card.userData;
		card.userData = newUnits;

		var topLabel = card.Q<Label>(TopChipText);
		topLabel.text = $"Amount: {newUnits}";
	}

	/// <summary>
	///     Sets the UI color and team name, resets the bought unit counters
	/// </summary>
	public void SetActivePlayer(Color activePlayer) {
		_activePlayer = activePlayer;

		SetupCards();
		bool isBlue = _activePlayer == Color.Blue;
		UnityEngine.Color color = isBlue ? _teamBlueColor : _teamRedColor;

		RootElement.Q(TopPanel).style.backgroundColor = color;
		RootElement.Q(BottomPanel).style.backgroundColor = color;

		string playerName = isBlue ? "Blue" : "Red";
		RootElement.Q<Label>(PlayerNameTurn).text = $"Player {playerName}'s turn";
	}

	/// <summary>
	///     Sets the displayed money amount of the given team
	/// </summary>
	public void SetPlayerMoney(Color teamColor, int playerMoney) {
		if (teamColor == _activePlayer) RootElement.Q<Label>(BudgetText).text = $"Budget: {playerMoney}";
	}

	/// <summary>
	///     Shows the unit deployment UI
	/// </summary>
	public void Show() {
		RootElement.style.display = DisplayStyle.Flex;
	}

	/// <summary>
	///     Hides the unit deployment UI
	/// </summary>
	public void Hide() {
		RootElement.style.display = DisplayStyle.None;
	}

	/// <summary>
	///     Updates overall unit deployment statistics of the given team
	/// </summary>
	public void UpdateDeployedUnitStatistics(GameTeam team) {
		if (team.TeamColor != _activePlayer) return;

		VisualElement container = RootElement.Q(DeployedUnitsContainer);
		container.Clear();
		foreach (IUnitData type in unitTypes)
			container.Add(new Label { text = $"{type.Name}: {team.GetDeployedUnitTypeCount(type)}" });
	}

	/// <summary>
	///     Invoked when a unit purchase card is clicked
	/// </summary>
	public event Action<IUnitData> OnUnitPurchased;

	/// <summary>
	///     Invoked when the next button is clicked
	/// </summary>
	public event Action OnNextClicked;
}
}
