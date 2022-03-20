using System;
using System.Collections.Generic;
using Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;
using Color = Logic.Data.Color;

namespace Presentation.UI {
public class UnitDeploymentUI : MonoBehaviour {
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
		RootElement.Q<Button>(SimulationUI.NextButton).clicked += () => OnNextClicked?.Invoke();
	}

	public void SetTeamColors(UnityEngine.Color teamRedColor, UnityEngine.Color teamBlueColor) {
		_teamRedColor = teamRedColor;
		_teamBlueColor = teamBlueColor;
	}

	private void SetupCards() {
		_unitCards.Clear();
		var cardList = RootElement.Q<VisualElement>(SimulationUI.BottomPanel);
		cardList.Clear();

		foreach (UnitTypeData unitType in unitTypes) {
			UnitTypeData unit = unitType;
			TemplateContainer card = cardComponent.Instantiate();
			var content = card.Q<VisualElement>("CardContent");
			content.style.backgroundImage = new StyleBackground(unitType.sprite);

			card.userData = 0;
			UpdateCardUnitCount(card, 0);

			var bottomLabel = card.Q<Label>("BottomChipText");
			bottomLabel.text = $"Cost: {unitType.cost}";

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

		var topLabel = card.Q<Label>("TopChipText");
		topLabel.text = $"Amount: {newUnits}";
	}

	public void SetActivePlayer(Color activePlayer) {
		_activePlayer = activePlayer;

		SetupCards();
		UnityEngine.Color color = _activePlayer == Color.Blue ? _teamBlueColor : _teamRedColor;

		RootElement.Q(SimulationUI.TopPanel).style.backgroundColor = color;
		RootElement.Q(SimulationUI.BottomPanel).style.backgroundColor = color;
	}

	public void SetPlayerMoney(int playerMoney) {
		RootElement.Q<Label>(SimulationUI.BudgetText).text = $"Budget: {playerMoney}";
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
