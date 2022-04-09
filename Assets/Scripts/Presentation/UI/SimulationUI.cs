using System;
using Logic.Command;
using Logic.Command.Barrack;
using Logic.Command.Tower;
using Logic.Command.Unit;
using Logic.Data;
using Logic.Data.World;
using Logic.Event;
using Logic.Event.Team;
using Logic.Event.World.Castle;
using Presentation.World;
using UnityEngine;
using UnityEngine.UIElements;
using Barrack = Logic.Data.World.Barrack;
using Color = UnityEngine.Color;
using Tower = Logic.Data.World.Tower;
using Ordering = Logic.Event.EventDispatcher.Ordering;

namespace Presentation.UI {
public class SimulationUI : MonoBehaviour {
	private const string GameView = "GameView";

	public Color teamRedColor;
	public Color teamBlueColor;
	private Logic.Data.Color _activePlayer = Logic.Data.Color.Blue;

	private BattleUI _battleUI;
	private GameManager _gameManager;
	private GameOverOverlay _gameOverOverlay;
	private UIState _lastUiState;
	private PauseOverlay _pauseOverlay;
	private Barrack _selectedBarrack;
	private TowerTypeData _selectedTowerType;

	private SimulationManager _simulationManager;
	private TowerPlacingUI _towerPlacing;
	private UIState _uiState = UIState.UnitDeployment;
	private UnitDeploymentUI _unitDeployment;

	private GameOverview GameOverview => _simulationManager.GameOverview;

	private void Start() {
		_lastUiState = _uiState;
		_gameManager = FindObjectOfType<GameManager>();

		_simulationManager = FindObjectOfType<SimulationManager>();
		_simulationManager.OnTileSelected += OnTileSelected;

		_battleUI = GetComponentInChildren<BattleUI>();
		_battleUI.OnPauseClicked += OnPauseClicked;
		_battleUI.OnExitClicked += OnExitClicked;

		_gameOverOverlay = GetComponentInChildren<GameOverOverlay>();

		_gameOverOverlay.OnOkClicked += HideGameOverOverlay;

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

		GameOverview.Events.AddListener<PhaseAdvancedEvent>(Ordering.Normal, OnPhaseAdvanced);
		GameOverview.Events.AddListener<CastleDestroyedEvent>(Ordering.Normal, OnCastleDestroyed);
		GameOverview.Events.AddListener<TeamMoneyUpdatedEvent>(Ordering.Normal, OnTeamMoneyUpdated);
		GameOverview.Events.AddListener<TeamStatisticsUpdatedEvent>(Ordering.Normal, OnTeamStatisticsUpdated);

		HideUIs();
		SetupMousePanning();
		UpdateUiState(UIState.TowerPlacing);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (_simulationManager.IsPaused)
				ResumeGame();
			else
				UpdateUiState(UIState.Paused);
		}

		_battleUI.UpdateRemainingTime(GameOverview.TimeLeftFromPhase);
	}

	private void OnDestroy() {
		_simulationManager.OnTileSelected -= OnTileSelected;

		_gameOverOverlay.OnOkClicked -= HideGameOverOverlay;

		_battleUI.OnPauseClicked -= OnPauseClicked;
		_battleUI.OnExitClicked -= OnExitClicked;

		_towerPlacing.OnNextClicked -= StepTowerPlacing;
		_towerPlacing.OnTowerTypeSelected -= OnTowerTypeSelected;
		_towerPlacing.OnTowerDestroyed -= OnTowerDestroyed;
		_towerPlacing.OnTowerUpgraded -= OnTowerUpgraded;

		_pauseOverlay.OnResumeClicked -= OnResumeClicked;
		_pauseOverlay.OnNewGameClicked -= OnNewGameClicked;
		_pauseOverlay.OnExitClicked -= OnExitClicked;

		_unitDeployment.OnNextClicked -= StepUnitDeployment;
		_unitDeployment.OnUnitPurchased -= OnUnitPurchased;

		GameOverview.Events.RemoveListener<PhaseAdvancedEvent>(Ordering.Normal, OnPhaseAdvanced);
		GameOverview.Events.RemoveListener<CastleDestroyedEvent>(Ordering.Normal, OnCastleDestroyed);
		GameOverview.Events.RemoveListener<TeamMoneyUpdatedEvent>(Ordering.Normal, OnTeamMoneyUpdated);
		GameOverview.Events.RemoveListener<TeamStatisticsUpdatedEvent>(Ordering.Normal, OnTeamStatisticsUpdated);
	}

	private void OnTeamStatisticsUpdated(TeamStatisticsUpdatedEvent e) {
		_battleUI.SetTeamStatistics(e.Team);
	}

	private void OnTeamMoneyUpdated(TeamMoneyUpdatedEvent e) {
		int money = e.Team.Money;
		Logic.Data.Color color = e.Team.TeamColor;
		_towerPlacing.SetPlayerMoney(color, money);
		_unitDeployment.SetPlayerMoney(color, money);
		_battleUI.SetPlayerMoney(color, money);
	}

	private void OnCastleDestroyed(CastleDestroyedEvent e) {
		Logic.Data.Color winner =
			e.Castle.OwnerColor == Logic.Data.Color.Red ? Logic.Data.Color.Blue : Logic.Data.Color.Red;
		_gameOverOverlay.UpdateMessage(winner);
	}

	private void OnPhaseAdvanced(PhaseAdvancedEvent e) {
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
	}

	private void HideGameOverOverlay() {
		_simulationManager.ResumeGame();
		_gameOverOverlay.Hide();
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
			if (_selectedTowerType != null)
				_towerPlacing.ShowTowerTypeStats(_selectedTowerType);
			else
				_towerPlacing.ShowInstructions();
		} else {
			Debug.Log($"Failed to destroy: {tower}");
		}
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
		_gameOverOverlay.Hide();
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

			gameView.RegisterCallback<MouseDownEvent>(e => OnGameViewMouseDown?.Invoke(e));
			gameView.RegisterCallback<MouseUpEvent>(e => OnGameViewMouseUp?.Invoke(e));
			gameView.RegisterCallback<MouseMoveEvent>(e => OnGameViewMouseMove?.Invoke(e));
			gameView.RegisterCallback<MouseEnterEvent>(e => OnGameViewMouseEnter?.Invoke(e));
			gameView.RegisterCallback<MouseLeaveEvent>(e => OnGameViewMouseLeave?.Invoke(e));
		}
	}

	private void OnUnitPurchased(UnitTypeData unitType) {
		var command = new PurchaseUnitCommand(GameOverview.GetTeam(_activePlayer), unitType);
		if (GameOverview.Commands.Issue(command))
			_unitDeployment.OnUnitBought(unitType);
		else
			Debug.Log("Failed to deploy unit"); // TODO maybe show this on the UI
	}

	private void StartTowerPlacing(Logic.Data.Color player) {
		_activePlayer = player;
		GameTeam playerData = GameOverview.GetTeam(_activePlayer);

		_selectedTowerType = null;

		_towerPlacing.ResetUI();
		_towerPlacing.Show();
		_towerPlacing.SetActivePlayer(_activePlayer);
		_towerPlacing.SetPlayerMoney(playerData.TeamColor, playerData.Money);
	}

	private void StepTowerPlacing() {
		if (_activePlayer == Logic.Data.Color.Blue) {
			OnBuildingPossibleChanges?.Invoke(null);
			StartTowerPlacing(Logic.Data.Color.Red);
		} else {
			OnBuildingPossibleChanges?.Invoke(null);
			UpdateUiState(UIState.UnitDeployment);
		}
	}

	private void OnTowerTypeSelected(TowerTypeData towerType) {
		_selectedTowerType = towerType;
		_towerPlacing.ShowTowerTypeStats(towerType);
		OnBuildingPossibleChanges?.Invoke(GameOverview.GetTeam(_activePlayer));
	}

	private void OnTileSelected(TilePosition position, SimulationManager.MouseButton button) {
		switch (_uiState) {
			case UIState.TowerPlacing:
				if (button == SimulationManager.MouseButton.Left) HandleTowerPlacingTileSelection(position);

				break;
			case UIState.UnitDeployment:
				HandleUnitDeploymentTileSelection(position, button);
				break;
		}
	}

	private void HandleUnitDeploymentTileSelection(TilePosition position, SimulationManager.MouseButton mouseButton) {
		if (mouseButton == SimulationManager.MouseButton.Left) {
			if (GameOverview.World[position] is Barrack barrack && barrack.OwnerColor == _activePlayer) {
				_selectedBarrack = barrack;
				OnBarrackSelected?.Invoke(barrack);
			} else if (_selectedBarrack != null) {
				GameOverview.Commands.Issue(new AddBarrackCheckpointCommand(_selectedBarrack, position));
			}
		} else if (_selectedBarrack != null) {
			GameOverview.Commands.Issue(new RemoveBarrackCheckpointCommand(_selectedBarrack, position));
		}
	}

	private void HandleTowerPlacingTileSelection(TilePosition position) {
		GameTeam playerData = GameOverview.GetTeam(_activePlayer);
		TileObject tileObject = GameOverview.World[position];
		if (tileObject is Tower tower && tower.OwnerColor == _activePlayer) {
			_towerPlacing.ShowTowerStats(tower);
			Debug.Log($"[TowerPlacing]: A tower has been selected: {tower} at position {position}");
		} else if (_selectedTowerType != null) {
			GameOverview.Commands.Issue(new BuildTowerCommand(playerData, _selectedTowerType, position));
		}
	}

	private void StepUnitDeployment() {
		OnBarrackSelected?.Invoke(null);
		_selectedBarrack = null;
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
		_unitDeployment.SetPlayerMoney(player, playerData.Money);
	}

	private void StartBattle() {
		_battleUI.Show();
		_battleUI.ShowPauseButton();
		foreach (var team in GameOverview.Teams) {
			_battleUI.SetTeamStatistics(team);
		}
	}

	private void ShowPauseOverlay() {
		_pauseOverlay.Show();
	}

	private void ShowGameOverUI() {
		_battleUI.Show();
		_battleUI.ShowExitButton();
		_gameOverOverlay.Show();
		_simulationManager.PauseGame();
	}

	private void UpdateUiState(UIState uiState) {
		Debug.Log($"Updated UI state to: {uiState}");
		if (_uiState != uiState) _lastUiState = _uiState;

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
				HideUIs();
				ShowGameOverUI();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public event Action<GameTeam> OnBuildingPossibleChanges;
	public event Action<Barrack> OnBarrackSelected;
	public event Action<MouseEnterEvent> OnGameViewMouseEnter;
	public event Action<MouseLeaveEvent> OnGameViewMouseLeave;
	public event Action<MouseDownEvent> OnGameViewMouseDown;
	public event Action<MouseUpEvent> OnGameViewMouseUp;
	public event Action<MouseMoveEvent> OnGameViewMouseMove;
}
}
