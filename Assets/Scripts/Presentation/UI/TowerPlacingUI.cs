using System;
using System.Collections.Generic;
using Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;
using Color = Logic.Data.Color;
using Tower = Logic.Data.World.Tower;

namespace Presentation.UI {
[RequireComponent(typeof(UIDocument))]
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
	private List<TowerData> towersToPlace;

	private readonly List<VisualElement> _tabs = new List<VisualElement>();

	private Color _activePlayer = Color.Red;
	private Tower _selectedDeployedTower;

	private bool _showDeployedStats = true;
	private UnityEngine.Color _teamBlueColor = UnityEngine.Color.magenta;
	private UnityEngine.Color _teamRedColor = UnityEngine.Color.green;

	private VisualElement RootElement => ui.rootVisualElement;

	private void Start() {
		SetupCards();
		RootElement.Q<Button>(NextButton).clicked += () => OnNextClicked?.Invoke();

		foreach (string tabSelector in new[] { InstructionsText, TowerTypeStats, DeployedTowerStats })
			_tabs.Add(RootElement.Q(tabSelector));

		ResetUI();
		SetupTabs();
	}

	private void HideTabs() {
		foreach (VisualElement tab in _tabs) tab.style.display = DisplayStyle.None;
	}

	/// <summary>
	///     Shows the tower placing instructions
	/// </summary>
	public void ShowInstructions() {
		HideTabs();
		RootElement.Q(InstructionsText).style.display = DisplayStyle.Flex;
	}

	/// <summary>
	///     Resets the tower placing UI
	///     The method hides the visible tabs and shows the tower placing instructions
	/// </summary>
	public void ResetUI() {
		HideTabs();
		ShowInstructions();
	}

	/// <summary>
	///     Sets the colors that should be displayed on each team's turn
	/// </summary>
	public void SetTeamColors(UnityEngine.Color teamRedColor, UnityEngine.Color teamBlueColor) {
		_teamRedColor = teamRedColor;
		_teamBlueColor = teamBlueColor;
	}

	/// <summary>
	///     Sets the UI color and team name
	/// </summary>
	public void SetActivePlayer(Color activePlayer) {
		_activePlayer = activePlayer;

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
	///     Shows the tower placing UI
	/// </summary>
	public void Show() {
		RootElement.style.display = DisplayStyle.Flex;
	}

	/// <summary>
	///     Hides the tower placing UI
	/// </summary>
	public void Hide() {
		RootElement.style.display = DisplayStyle.None;
	}

	private void SetupCards() {
		var cardList = RootElement.Q<VisualElement>(BottomPanel);
		cardList.Clear();

		foreach (ITowerData towerType in towersToPlace) {
			TemplateContainer card = cardComponent.Instantiate();

			var content = card.Q<VisualElement>(CardContent);
			content.style.backgroundImage = new StyleBackground(towerType.PreviewSprite);
			card.Q(TopChip).style.display = DisplayStyle.None;

			var bottomLabel = card.Q<Label>(BottomChipText);
			bottomLabel.text = $"Cost: {towerType.BuildingCost}";

			card.RegisterCallback<ClickEvent>(e => {
				if (e.button == 0) OnTowerTypeSelected?.Invoke(towerType);
			});

			cardList.Add(card);
		}
	}

	/// <summary>
	///     Displays the stats of a tower type
	/// </summary>
	public void ShowTowerTypeStats(ITowerData towerType) {
		HideTabs();
		VisualElement tab = RootElement.Q(TowerTypeStats);
		tab.style.display = DisplayStyle.Flex;
		tab.Q<Label>(TowerName).text = towerType.Name;
		ShowTowerTypeStats(towerType, tab.Q<VisualElement>(TowerStats));
	}

	private void ShowTowerTypeStats(ITowerData towerType, VisualElement statsContainer) {
		statsContainer.Clear();
		if (towerType != null) {
			string[] stats = {
				$"Damage: {towerType.Damage}", $"Range: {towerType.Range}", $"Building Cost: {towerType.BuildingCost}",
				$"Destroy Refund: {towerType.DestroyRefund}", $"Cooldown Time: {towerType.CooldownTime}",
				$"UpgradeCost: {towerType.UpgradeCost}"
			};
			foreach (string stat in stats)
				statsContainer.Add(new Label(stat) { style = { marginTop = 2, marginBottom = 0 } });
		} else {
			Debug.Log("The tower type was null");
		}
	}

	private void SetupTabs() {
		VisualElement element = RootElement.Q(DeployedTowerStats);
		var statsButton = element.Q<Button>(StatsButton);
		var manageButton = element.Q<Button>(ManageButton);

		statsButton.clicked += () => {
			manageButton.RemoveFromClassList(SelectedButtonClass);
			statsButton.AddToClassList(SelectedButtonClass);
			_showDeployedStats = true;
			ShowTowerStats(_selectedDeployedTower);
		};

		manageButton.clicked += () => {
			statsButton.RemoveFromClassList(SelectedButtonClass);
			manageButton.AddToClassList(SelectedButtonClass);
			_showDeployedStats = false;
			ShowTowerStats(_selectedDeployedTower);
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

	/// <summary>
	///     Displays the stats of a built tower on the map
	/// </summary>
	public void ShowTowerStats(Tower tower) {
		HideTabs();
		_selectedDeployedTower = tower;
		VisualElement tab = RootElement.Q(DeployedTowerStats);
		tab.style.display = DisplayStyle.Flex;
		tab.Q<Label>(TowerName).text = tower.Type.Name;

		VisualElement statsContainer = tab.Q(StatsContainer);
		VisualElement manageContainer = tab.Q(ManageContainer);
		if (_showDeployedStats) {
			manageContainer.style.display = DisplayStyle.None;
			statsContainer.style.display = DisplayStyle.Flex;
			ShowTowerTypeStats(tower.Type as ITowerData, statsContainer);
		} else {
			manageContainer.style.display = DisplayStyle.Flex;
			statsContainer.style.display = DisplayStyle.None;
			var afterUpgrade = tower.Type.AfterUpgradeType as ITowerData;
			VisualElement container = manageContainer.Q(UpgradeContainer);

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

	/// <summary>
	///     Invoked when the destroy tower button is clicked
	/// </summary>
	public event Action<Tower> OnTowerDestroyed;

	/// <summary>
	///     Invoked when the update tower button is clicked
	/// </summary>
	public event Action<Tower> OnTowerUpgraded;

	/// <summary>
	///     Invoked when the next button is clicked
	/// </summary>
	public event Action OnNextClicked;

	/// <summary>
	///     Invoked when a tower type is selected
	/// </summary>
	public event Action<ITowerData> OnTowerTypeSelected;
}
}
