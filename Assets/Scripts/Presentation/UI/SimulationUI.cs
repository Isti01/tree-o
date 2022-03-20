using System;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;
using Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Tower = Logic.Data.World.Tower;
using Vector2 = Logic.Data.World.Vector2;

namespace Presentation.UI {
public class SimulationUI : MonoBehaviour {
	public const string TopPanel = "TopPanel";
	public const string NextButton = "Next";
	public const string BudgetText = "BudgetText";
	public static readonly string BottomPanel = "BottomPanel";

	public Color teamRedColor;
	public Color teamBlueColor;
	private Logic.Data.Color _activePlayer = Logic.Data.Color.Blue;

	private BattleUI _battleUI;
	private GameOverOverlay _gameOverOverlay;
	private PauseOverlay _pauseOverlay;

	private TowerTypeData _selectedTowerType;
	private SimulationManager _simulationManager;
	private TowerPlacingUI _towerPlacing;
	private UIState _uiState = UIState.UnitDeployment;
	private UnitDeploymentUI _unitDeployment;

	private GameOverview GameOverview => _simulationManager.GameOverview;

	private void Start() {
		_simulationManager = FindObjectOfType<SimulationManager>();
		_simulationManager.OnTileSelected += OnTileSelected;

		_battleUI = GetComponentInChildren<BattleUI>();
		_gameOverOverlay = GetComponentInChildren<GameOverOverlay>();
		_pauseOverlay = GetComponentInChildren<PauseOverlay>();
		_towerPlacing = GetComponentInChildren<TowerPlacingUI>();

		_towerPlacing.SetTeamColors(teamRedColor, teamBlueColor);
		_towerPlacing.Hide();

		_towerPlacing.OnNextClicked += StepTowerPlacing;
		_towerPlacing.OnTowerTypeSelected += OnTowerTypeSelected;

		_unitDeployment = GetComponentInChildren<UnitDeploymentUI>();

		_unitDeployment.SetTeamColors(teamRedColor, teamBlueColor);
		_unitDeployment.Hide();

		_unitDeployment.OnNextClicked += StepUnitDeployment;
		_unitDeployment.OnUnitPurchased += OnUnitPurchased;

		SetupMousePanning();
		UpdateUiState(UIState.TowerPlacing);
	}

	private void OnDestroy() {
		_simulationManager.OnTileSelected -= OnTileSelected;
	}

	private void SetupMousePanning() {
		UIDocument[] uiDocs = GetComponentsInChildren<UIDocument>();

		foreach (UIDocument ui in uiDocs) {
			var gameView = ui.rootVisualElement.Q<VisualElement>("GameView");
			if (gameView == null) continue;

			gameView.RegisterCallback<MouseDownEvent>(e => OnGameViewPanStart?.Invoke(e));
			gameView.RegisterCallback<MouseUpEvent>(e => OnGameViewPanEnd?.Invoke(e));
			gameView.RegisterCallback<MouseMoveEvent>(e => OnGameViewPanUpdate?.Invoke(e));
		}
	}

	private void OnUnitPurchased(UnitTypeData unitType) {
		var command = new PurchaseUnitCommand(GameOverview.GetTeam(_activePlayer), unitType);
		if (GameOverview.Commands.Issue(command).IsSuccess) {
			_unitDeployment.OnUnitBought(unitType);
			_unitDeployment.SetPlayerMoney(GameOverview.GetTeam(_activePlayer).Money);
		} else {
			Debug.Log("Failed to deploy unit"); // TODO maybe show this on the UI
		}
	}

	private void StartTowerPlacing(Logic.Data.Color player) {
		_activePlayer = player;
		GameTeam playerData = GameOverview.GetTeam(_activePlayer);

		_selectedTowerType = null;

		_towerPlacing.Show();
		_towerPlacing.SetActivePlayer(_activePlayer);
		_towerPlacing.SetPlayerMoney(playerData.Money);
	}

	private void StepTowerPlacing() {
		if (_activePlayer == Logic.Data.Color.Blue) {
			StartTowerPlacing(Logic.Data.Color.Red);
		} else {
			_towerPlacing.Hide();
			UpdateUiState(UIState.UnitDeployment);
		}
	}

	private void OnTowerTypeSelected(TowerTypeData towerType) {
		_selectedTowerType = towerType;
	}

	private void OnTileSelected(Vector2 position) {
		GameTeam playerData = GameOverview.GetTeam(_activePlayer);
		if (_uiState == UIState.TowerPlacing && _selectedTowerType != null) {
			string towerName = _selectedTowerType.name; // TODO try to place the tower
			Debug.Log($"[TowerPlacing]: {playerData.TeamColor} team placed {towerName} tower to {position}");
		} else if (_uiState == UIState.TowerPlacing) {
			TileObject tileObject = GameOverview.World[(int) position.X, (int) position.Y];
			if (tileObject is Tower tower
				&& tower.OwnerColor == _activePlayer) // TODO show the selected tower in the UI
				Debug.Log($"[TowerPlacing]: A tower has been selected: {tower} at position {position}");
		}
	}

	private void StepUnitDeployment() {
		if (_activePlayer == Logic.Data.Color.Blue) {
			StartUnitDeployment(Logic.Data.Color.Red);
		} else {
			_unitDeployment.Hide();
			UpdateUiState(UIState.Battle);
		}
	}

	private void StartUnitDeployment(Logic.Data.Color player) {
		_activePlayer = player;
		GameTeam playerData = GameOverview.GetTeam(_activePlayer);

		_unitDeployment.Show();
		_unitDeployment.SetActivePlayer(_activePlayer);
		_unitDeployment.SetPlayerMoney(playerData.Money);
	}

	private void UpdateUiState(UIState uiState) {
		_uiState = uiState;
		switch (_uiState) {
			case UIState.TowerPlacing:
				StartTowerPlacing(Logic.Data.Color.Blue);
				break;
			case UIState.UnitDeployment:
				StartUnitDeployment(Logic.Data.Color.Blue);
				break;
			case UIState.Battle:
				break;
			case UIState.Paused:
				break;
			case UIState.GameOver:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public event Action<MouseDownEvent> OnGameViewPanStart;
	public event Action<MouseUpEvent> OnGameViewPanEnd;
	public event Action<MouseMoveEvent> OnGameViewPanUpdate;
}
}
