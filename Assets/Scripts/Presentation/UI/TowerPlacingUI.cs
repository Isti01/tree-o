using System;
using System.Collections.Generic;
using Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;
using Color = Logic.Data.Color;
using Tower = Logic.Data.World.Tower;

namespace Presentation.UI {
public class TowerPlacingUI : MonoBehaviour {
	private const string TopPanel = "TopPanel";
	private const string NextButton = "Next";
	private const string BudgetText = "BudgetText";
	private const string BottomPanel = "BottomPanel";
	private const string CardContent = "CardContent";
	private const string TopChip = "TopChip";
	private const string BottomChipText = "BottomChipText";
	private const string PlayerNameTurn = "PlayerNameTurn";
	private const string InstructionsText = "InstructionsText";
	private const string TowerTypeStats = "TowerTypeStats";
	private const string DeployedTowerStats = "DeployedTowerStats";
	private const string TowerName = "TowerName";
	private const string TowerStats = "TowerStats";
	private const string StatsContainer = "StatsContainer";
	private const string StatsButton = "StatsButton";
	private const string ManageButton = "ManageButton";
	private const string ManageContainer = "ManageContainer";
	private const string AfterUpgrade = "AfterUpgrade";
	private const string DestroyButton = "DestroyButton";
	private const string MoneyRecoveredText = "MoneyRecoveredText";
	private const string UpgradeButton = "UpgradeButton";
	private const string UpgradeCost = "UpgradeCost";
	private const string UpgradeContainer = "UpgradeContainer";

	private const string SelectedButtonClass = "selected-button";

	[SerializeField]
	private UIDocument ui;

	[SerializeField]
	private VisualTreeAsset cardComponent;

	[SerializeField]
	private List<TowerTypeData> towersToPlace;

	private readonly List<VisualElement> _tabs = new List<VisualElement>();

	private Color _activePlayer = Color.Red;
	private UnityEngine.Color _teamBlueColor = UnityEngine.Color.magenta;
	private UnityEngine.Color _teamRedColor = UnityEngine.Color.green;

	private VisualElement RootElement => ui.rootVisualElement;

	private bool _showDeployedStats = true;
	private Tower _selectedDeployedTower;

	private void Start() {
		SetupCards();
		RootElement.Q<Button>(NextButton).clicked += () => OnNextClicked?.Invoke();

		foreach (string tabSelector in new[] { InstructionsText, TowerTypeStats, DeployedTowerStats }) {
			_tabs.Add(RootElement.Q(tabSelector));
		}

		ResetUI();
		SetupTabs();
	}

	private void HideTabs() {
		foreach (VisualElement tab in _tabs) tab.style.display = DisplayStyle.None;
	}

	public void ShowInstructions() {
		HideTabs();
		RootElement.Q(InstructionsText).style.display = DisplayStyle.Flex;
	}

	public void ResetUI() {
		HideTabs();
		ShowInstructions();
	}

	public void SetTeamColors(UnityEngine.Color teamRedColor, UnityEngine.Color teamBlueColor) {
		_teamRedColor = teamRedColor;
		_teamBlueColor = teamBlueColor;
	}

	public void SetActivePlayer(Color activePlayer) {
		_activePlayer = activePlayer;

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

	private void SetupCards() {
		var cardList = RootElement.Q<VisualElement>(BottomPanel);
		cardList.Clear();

		foreach (TowerTypeData towerType in towersToPlace) {
			TemplateContainer card = cardComponent.Instantiate();

			var content = card.Q<VisualElement>(CardContent);
			content.style.backgroundImage = new StyleBackground(towerType.Sprite);
			card.Q(TopChip).style.display = DisplayStyle.None;

			var bottomLabel = card.Q<Label>(BottomChipText);
			bottomLabel.text = $"Cost: {towerType.BuildingCost}";

			card.RegisterCallback<ClickEvent>(e => {
				if (e.button == 0) OnTowerTypeSelected?.Invoke(towerType);
			});

			cardList.Add(card);
		}
	}

	public void ShowTowerTypeStats(TowerTypeData towerType) {
		HideTabs();
		var tab = RootElement.Q(TowerTypeStats);
		tab.style.display = DisplayStyle.Flex;
		tab.Q<Label>(TowerName).text = towerType.Name;
		ShowTowerTypeStats(towerType, tab.Q<VisualElement>(TowerStats));
	}

	private void ShowTowerTypeStats(TowerTypeData towerType, VisualElement statsContainer) {
		statsContainer.Clear();
		if (towerType != null) {
			var stats = new[] {
				$"Damage: {towerType.Damage}", $"Range: {towerType.Range}", $"Building Cost: {towerType.BuildingCost}",
				$"Destroy Refund: {towerType.DestroyRefund}", $"Cooldown Time: {towerType.CooldownTime}",
				$"UpgradeCost: {towerType.UpgradeCost}",
			};
			foreach (string stat in stats) {
				statsContainer.Add(new Label(stat) { style = { marginTop = 2, marginBottom = 0 } });
			}
		} else {
			Debug.Log("The tower type was null");
		}
	}

	private void SetupTabs() {
		var element = RootElement.Q(DeployedTowerStats);
		var statsButton = element.Q<Button>(StatsButton);
		var manageButton = element.Q<Button>(ManageButton);

		statsButton.clicked += () => {
			manageButton.RemoveFromClassList(SelectedButtonClass);
			statsButton.AddToClassList(SelectedButtonClass);
			_showDeployedStats = true;
			ShowTowerStats(_selectedDeployedTower);
			Debug.Log("Clicked stats");
		};

		manageButton.clicked += () => {
			statsButton.RemoveFromClassList(SelectedButtonClass);
			manageButton.AddToClassList(SelectedButtonClass);
			_showDeployedStats = false;
			ShowTowerStats(_selectedDeployedTower);
			Debug.Log("Clicked manage");
		};

		element.Q<Button>(DestroyButton).clicked += () => {
			if (_selectedDeployedTower == null) Debug.LogError("The selected tower is null");
			OnTowerDestroyed?.Invoke(_selectedDeployedTower);
		};
		element.Q<Button>(UpgradeButton).clicked += () => {
			if (_selectedDeployedTower == null) Debug.LogError("The selected tower is null");
			OnTowerUpgraded?.Invoke(_selectedDeployedTower);
		};
	}

	public void ShowTowerStats(Tower tower) {
		HideTabs();
		_selectedDeployedTower = tower;
		var tab = RootElement.Q(DeployedTowerStats);
		tab.style.display = DisplayStyle.Flex;
		tab.Q<Label>(TowerName).text = tower.Type.Name;

		var statsContainer = tab.Q(StatsContainer);
		var manageContainer = tab.Q(ManageContainer);
		if (_showDeployedStats) {
			manageContainer.style.display = DisplayStyle.None;
			statsContainer.style.display = DisplayStyle.Flex;
			ShowTowerTypeStats(tower.Type as TowerTypeData, statsContainer);
		} else {
			manageContainer.style.display = DisplayStyle.Flex;
			statsContainer.style.display = DisplayStyle.None;
			var afterUpgrade = tower.Type.AfterUpgradeType as TowerTypeData;
			var container = manageContainer.Q(UpgradeContainer);

			if (afterUpgrade != null) {
				container.style.display = DisplayStyle.Flex;
				ShowTowerTypeStats(afterUpgrade, manageContainer.Q(AfterUpgrade));
				manageContainer.Q<Label>(UpgradeCost).text = $"Upgrade Cost: {tower.Type.UpgradeCost}";
			} else {
				container.style.display = DisplayStyle.None;
			}

			manageContainer.Q<Label>(MoneyRecoveredText).text =
				$"Money Recovered after destroyed: {tower.Type.DestroyRefund}";
		}
	}

	public event Action<Tower> OnTowerDestroyed;
	public event Action<Tower> OnTowerUpgraded;
	public event Action OnNextClicked;
	public event Action<TowerTypeData> OnTowerTypeSelected;
}
}
