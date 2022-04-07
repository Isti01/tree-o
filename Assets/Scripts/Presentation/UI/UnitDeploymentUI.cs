using System;
using System.Collections.Generic;
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

	[SerializeField]
	private VisualTreeAsset cardComponent;

	[SerializeField]
	private UIDocument ui;

	[SerializeField]
	private List<UnitTypeData> unitTypes;

	private readonly Dictionary<UnitTypeData, VisualElement> _unitCards = new Dictionary<UnitTypeData, VisualElement>();

	private Color _activePlayer = Color.Red;
	private UnityEngine.Color _teamBlueColor = UnityEngine.Color.magenta;
	private UnityEngine.Color _teamRedColor = UnityEngine.Color.green;

	private VisualElement RootElement => ui.rootVisualElement;

	private void Start() {
		SetupCards();
		RootElement.Q<Button>(NextButton).clicked += () => OnNextClicked?.Invoke();
	}

	public void SetTeamColors(UnityEngine.Color teamRedColor, UnityEngine.Color teamBlueColor) {
		_teamRedColor = teamRedColor;
		_teamBlueColor = teamBlueColor;
	}

	private void SetupCards() {
		_unitCards.Clear();
		var cardList = RootElement.Q<VisualElement>(BottomPanel);
		cardList.Clear();

		foreach (UnitTypeData unitType in unitTypes) {
			UnitTypeData unit = unitType;
			TemplateContainer card = cardComponent.Instantiate();
			var content = card.Q<VisualElement>(CardContent);
			content.style.backgroundImage = new StyleBackground(unitType.AliveSprite);

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

	public void OnUnitBought(UnitTypeData unit) {
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

	public void SetActivePlayer(Color activePlayer) {
		_activePlayer = activePlayer;

		SetupCards(); // TODO maybe do this better (store references to the cards, and query the deployed units by ref)
		bool isBlue = _activePlayer == Color.Blue;
		UnityEngine.Color color = isBlue ? _teamBlueColor : _teamRedColor;

		RootElement.Q(TopPanel).style.backgroundColor = color;
		RootElement.Q(BottomPanel).style.backgroundColor = color;

		string playerName = isBlue ? "Blue" : "Red";
		RootElement.Q<Label>(PlayerNameTurn).text = $"Player {playerName}'s turn";
	}

	public void SetPlayerMoney(int playerMoney) {
		RootElement.Q<Label>(BudgetText).text = $"Budget: {playerMoney}";
	}

	public void Show() {
		RootElement.style.display = DisplayStyle.Flex;
	}

	public void Hide() {
		RootElement.style.display = DisplayStyle.None;
	}

	public event Action<UnitTypeData> OnUnitPurchased;
	public event Action OnNextClicked;
}
}
