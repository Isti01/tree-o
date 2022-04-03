using System;
using Logic.Command;
using Logic.Command.Tower;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;
using Logic.Event;
using Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;
using Tower = Logic.Data.World.Tower;

namespace Presentation.UI {
public class SimulationUI : MonoBehaviour {
	private const string GameView = "GameView";

	public Color teamRedColor;
	public Color teamBlueColor;
	private Logic.Data.Color _activePlayer = Logic.Data.Color.Blue;

	private BattleUI _battleUI;
	private GameManager _gameManager;
	private GameOverOverlay _gameOverOverlay;
	private PauseOverlay _pauseOverlay;
	private TowerTypeData _selectedTowerType;

	private SimulationManager _simulationManager;
	private TowerPlacingUI _towerPlacing;
	private UIState _uiState = UIState.UnitDeployment;
	private UIState _lastUiState;
	private UnitDeploymentUI _unitDeployment;

	private GameOverview GameOverview => _simulationManager.GameOverview;

	private void Start() {
		_lastUiState = _uiState;
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
		_towerPlacing.OnTowerDestroyed += OnTowerDestroyed;
		_towerPlacing.OnTowerUpgraded += OnTowerUpgraded;

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

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (_simulationManager.IsPaused) {
				ResumeGame();
			} else {
				UpdateUiState(UIState.Paused);
			}
		}
	}

	private void OnTowerUpgraded(Tower tower) {
		Debug.Log(GameOverview.Commands.Issue(new UpgradeTowerCommand(tower)).IsSuccess
			? $"Upgraded: {tower}"
			: $"Failed to upgrade: {tower}");
		_towerPlacing.ShowTowerStats(tower);
	}

	private void OnTowerDestroyed(Tower tower) {
		if (GameOverview.Commands.Issue(new DestroyTowerCommand(tower))) {
			Debug.Log($"Destroyed: {tower}");
			if (_selectedTowerType != null) {
				_towerPlacing.ShowTowerTypeStats(_selectedTowerType);
			} else {
				_towerPlacing.ShowInstructions();
			}
		} else {
			Debug.Log($"Failed to destroy: {tower}");
		}
	}

	private void OnDestroy() {
		_simulationManager.OnTileSelected -= OnTileSelected;

		_battleUI.OnPauseClicked -= OnPauseClicked;

		_towerPlacing.OnNextClicked -= StepTowerPlacing;
		_towerPlacing.OnTowerTypeSelected -= OnTowerTypeSelected;
		_towerPlacing.OnTowerDestroyed -= OnTowerDestroyed;
		_towerPlacing.OnTowerUpgraded -= OnTowerUpgraded;

		_unitDeployment.OnNextClicked -= StepUnitDeployment;
		_unitDeployment.OnUnitPurchased -= OnUnitPurchased;
	}

	private void OnResumeClicked() {
		ResumeGame();
	}

	private void ResumeGame() {
		UpdateUiState(_lastUiState);
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
			var gameView = ui.rootVisualElement.Q<VisualElement>(GameView);
			if (gameView == null) continue;

			gameView.RegisterCallback<MouseDownEvent>(e => OnGameViewPanStart?.Invoke(e));
			gameView.RegisterCallback<MouseUpEvent>(e => OnGameViewPanEnd?.Invoke(e));
			gameView.RegisterCallback<MouseMoveEvent>(e => OnGameViewPanUpdate?.Invoke(e));
			gameView.RegisterCallback<MouseEnterEvent>(e => OnGameViewMouseEnter?.Invoke(e));
			gameView.RegisterCallback<MouseLeaveEvent>(e => OnGameViewMouseLeave?.Invoke(e));
		}
	}

	private void OnUnitPurchased(UnitTypeData unitType) {
		var command = new PurchaseUnitCommand(GameOverview.GetTeam(_activePlayer), unitType);
		if (GameOverview.Commands.Issue(command)) {
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

		_towerPlacing.ResetUI();
		_towerPlacing.Show();
		_towerPlacing.SetActivePlayer(_activePlayer);
		_towerPlacing.SetPlayerMoney(playerData.Money);
	}

	private void StepTowerPlacing() {
		if (_activePlayer == Logic.Data.Color.Blue)
			StartTowerPlacing(Logic.Data.Color.Red);
		else
			UpdateUiState(UIState.UnitDeployment);
	}

	private void OnTowerTypeSelected(TowerTypeData towerType) {
		_selectedTowerType = towerType;
		_towerPlacing.ShowTowerTypeStats(towerType);
	}

	private void OnTileSelected(TilePosition position) {
		if (_uiState == UIState.TowerPlacing) HandleTowerPlacingTileSelection(position);
	}

	private void HandleTowerPlacingTileSelection(TilePosition position) {
		GameTeam playerData = GameOverview.GetTeam(_activePlayer);
		TileObject tileObject = GameOverview.World[position.X, position.Y];
		if (tileObject is Tower tower && tower.OwnerColor == _activePlayer) {
			_towerPlacing.ShowTowerStats(tower);
			Debug.Log($"[TowerPlacing]: A tower has been selected: {tower} at position {position}");
		} else if (_selectedTowerType != null) {
			GameOverview.Commands.Issue(new BuildTowerCommand(playerData, _selectedTowerType, position));
			_towerPlacing.SetPlayerMoney(playerData.Money); // TODO maybe handle the possible outcomes
		}
	}

	private void StepUnitDeployment() {
		if (_activePlayer == Logic.Data.Color.Blue) {
			StartUnitDeployment(Logic.Data.Color.Red);
		} else {
			UpdateUiState(UIState.Battle);
			if (GameOverview.CurrentPhase == GamePhase.Prepare)
				if (!GameOverview.Commands.Issue(new AdvancePhaseCommand(GameOverview)))
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
		Debug.Log($"Updated UI state to: {uiState}");
		if (_uiState != uiState) {
			_lastUiState = _uiState;
		}

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

	public event Action<MouseEnterEvent> OnGameViewMouseEnter;
	public event Action<MouseLeaveEvent> OnGameViewMouseLeave;
	public event Action<MouseDownEvent> OnGameViewPanStart;
	public event Action<MouseUpEvent> OnGameViewPanEnd;
	public event Action<MouseMoveEvent> OnGameViewPanUpdate;
}
}
