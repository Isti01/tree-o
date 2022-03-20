using System;
using System.Collections.Generic;
using Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;
using Color = Logic.Data.Color;

namespace Presentation.UI {
public class TowerPlacingUI : MonoBehaviour {
	[SerializeField]
	private UIDocument ui;

	[SerializeField]
	private VisualTreeAsset cardComponent;

	[SerializeField]
	private List<TowerTypeData> towersToPlace;

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

	public void SetActivePlayer(Color activePlayer) {
		_activePlayer = activePlayer;

		bool isBlue = _activePlayer == Color.Blue;
		UnityEngine.Color color = isBlue ? _teamBlueColor : _teamRedColor;

		RootElement.Q(SimulationUI.TopPanel).style.backgroundColor = color;
		RootElement.Q(SimulationUI.BottomPanel).style.backgroundColor = color;

		string playerName = isBlue ? "Blue" : "Red";
		RootElement.Q<Label>("PlayerNameTurn").text = $"Player {playerName}'s turn";
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

	private void SetupCards() {
		var cardList = RootElement.Q<VisualElement>(SimulationUI.BottomPanel);
		cardList.Clear();

		foreach (TowerTypeData towerType in towersToPlace) {
			TemplateContainer card = cardComponent.Instantiate();

			var content = card.Q<VisualElement>("CardContent");
			content.style.backgroundImage = new StyleBackground(towerType.sprite);
			card.Q("TopChip").style.display = DisplayStyle.None;

			var bottomLabel = card.Q<Label>("BottomChipText");
			bottomLabel.text = $"Cost: {towerType.buildingCost}";

			card.RegisterCallback<ClickEvent>(e => {
				if (e.button == 0) OnTowerTypeSelected?.Invoke(towerType);
			});

			cardList.Add(card);
		}
	}

	public event Action OnNextClicked;
	public event Action<TowerTypeData> OnTowerTypeSelected;
}
}
