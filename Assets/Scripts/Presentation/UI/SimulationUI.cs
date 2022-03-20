using System;
using Logic.Command;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;
using Logic.Event;
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
	private TowerPlacingUI _towerPlacing;
	private UIState _uiState = UIState.UnitDeployment;
	private UnitDeploymentUI _unitDeployment;

	private SimulationManager _simulationManager;
	private GameManager _gameManager;

	private GameOverview GameOverview => _simulationManager.GameOverview;

	private void Start() {
		_gameManager = FindObjectOfType<GameManager>();

		_simulationManager = FindObjectOfType<SimulationManager>();
		_simulationManager.OnTileSelected += OnTileSelected;

		_battleUI = GetComponentInChildren<BattleUI>();
		_battleUI.OnPauseClicked += OnPauseClicked;

		_gameOverOverlay = GetComponentInChildren<GameOverOverlay>();

		_pauseOverlay = GetComponentInChildren<PauseOverlay>();

		_pauseOverlay.OnResumeClicked += OnResumeClicked;
		_pauseOverlay.OnNewGameClicked += OnNewGameClicked;
		_pauseOverlay.OnExitClicked += OnExitClicked;

		_towerPlacing = GetComponentInChildren<TowerPlacingUI>();
		_towerPlacing.SetTeamColors(teamRedColor, teamBlueColor);

		_towerPlacing.OnNextClicked += StepTowerPlacing;
		_towerPlacing.OnTowerTypeSelected += OnTowerTypeSelected;

		_unitDeployment = GetComponentInChildren<UnitDeploymentUI>();
		_unitDeployment.SetTeamColors(teamRedColor, teamBlueColor);

		_unitDeployment.OnNextClicked += StepUnitDeployment;
		_unitDeployment.OnUnitPurchased += OnUnitPurchased;

		GameOverview.Events.AddListener<PhaseAdvancedEvent>(args => {
			switch (GameOverview.CurrentPhase) {
				case GamePhase.Prepare:
					UpdateUiState(UIState.TowerPlacing);
					break;
				case GamePhase.Fight:
					UpdateUiState(UIState.Battle);
					break;
				case GamePhase.Finished:
					UpdateUiState(UIState.GameOver);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		});

		HideUIs();
		SetupMousePanning();
		UpdateUiState(UIState.TowerPlacing);
	}

	private void OnResumeClicked() {
		_simulationManager.ResumeGame();
		_pauseOverlay.Hide();
	}

	private void OnNewGameClicked() {
		_simulationManager.ResumeGame();
		_gameManager.LoadNewGame();
	}

	private void OnExitClicked() {
		_simulationManager.ResumeGame();
		_gameManager.LoadMenu();
	}

	private void OnDestroy() {
		_simulationManager.OnTileSelected -= OnTileSelected;

		_battleUI.OnPauseClicked -= OnPauseClicked;

		_towerPlacing.OnNextClicked -= StepTowerPlacing;
		_towerPlacing.OnTowerTypeSelected -= OnTowerTypeSelected;

		_unitDeployment.OnNextClicked -= StepUnitDeployment;
		_unitDeployment.OnUnitPurchased -= OnUnitPurchased;
	}

	private void HideUIs() {
		_pauseOverlay.Hide();
		_battleUI.Hide();
		_unitDeployment.Hide();
		_towerPlacing.Hide();
	}

	private void OnPauseClicked() {
		UpdateUiState(UIState.Paused);
		_simulationManager.PauseGame();
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
			UpdateUiState(UIState.Battle);
			if (GameOverview.CurrentPhase == GamePhase.Prepare)
				if (!GameOverview.Commands.Issue(new AdvancePhaseCommand(GameOverview)).IsSuccess)
					Debug.LogError("[GamePhase] Failed to advance the GamePhase");
		}
	}

	private void StartUnitDeployment(Logic.Data.Color player) {
		_activePlayer = player;
		GameTeam playerData = GameOverview.GetTeam(_activePlayer);

		_unitDeployment.Show();
		_unitDeployment.SetActivePlayer(_activePlayer);
		_unitDeployment.SetPlayerMoney(playerData.Money);
	}

	private void StartBattle() {
		_battleUI.Show();
	}

	private void ShowPauseOverlay() {
		_pauseOverlay.Show();
	}

	private void UpdateUiState(UIState uiState) {
		_uiState = uiState;
		switch (_uiState) {
			case UIState.TowerPlacing:
				HideUIs();
				StartTowerPlacing(Logic.Data.Color.Blue);
				break;
			case UIState.UnitDeployment:
				HideUIs();
				StartUnitDeployment(Logic.Data.Color.Blue);
				break;
			case UIState.Battle:
				HideUIs();
				StartBattle();
				break;
			case UIState.Paused:
				ShowPauseOverlay();
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
