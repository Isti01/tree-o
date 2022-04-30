using System;
using System.Linq;
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
using EventDispatcher = Logic.Event.EventDispatcher;
using Tower = Logic.Data.World.Tower;

namespace Presentation.UI {
public class SimulationUI : MonoBehaviour {
	private const string GameView = "GameView";

	[SerializeField]
	private Color teamRedColor;

	[SerializeField]
	private Color teamBlueColor;

	private Logic.Data.Color _activePlayer = Logic.Data.Color.Blue;

	private BattleUI _battleUI;
	private GameManager _gameManager;
	private GameOverOverlay _gameOverOverlay;
	private UIState _lastUiState;
	private PauseOverlay _pauseOverlay;
	private Barrack _selectedBarrack;
	private ITowerData _selectedTowerType;

	private SimulationManager _simulationManager;
	private TowerPlacingUI _towerPlacing;
	private UIState _uiState = UIState.UnitDeployment;
	private UnitDeploymentUI _unitDeployment;

	private IGameOverview GameOverview => _simulationManager.GameOverview;

	private void Start() {
		_lastUiState = _uiState;
		_gameManager = FindObjectOfType<GameManager>();

		_simulationManager = FindObjectOfType<SimulationManager>();
		_simulationManager.OnTileSelected += OnTileSelected;

		_battleUI = GetComponentInChildren<BattleUI>();
		_battleUI.OnPauseClicked += PauseGame;
		_battleUI.OnExitClicked += OnExitClicked;

		_gameOverOverlay = GetComponentInChildren<GameOverOverlay>();

		_gameOverOverlay.OnOkClicked += HideGameOverOverlay;

		_pauseOverlay = GetComponentInChildren<PauseOverlay>();

		_pauseOverlay.OnResumeClicked += ResumeGame;
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

		GameOverview.Events.AddListener<PhaseAdvancedEvent>(EventDispatcher.Ordering.Normal, OnPhaseAdvanced);
		GameOverview.Events.AddListener<CastleDestroyedEvent>(EventDispatcher.Ordering.Normal, OnCastleDestroyed);
		GameOverview.Events.AddListener<TeamMoneyUpdatedEvent>(EventDispatcher.Ordering.Normal, OnTeamMoneyUpdated);
		GameOverview.Events.AddListener<TeamStatisticsUpdatedEvent>(EventDispatcher.Ordering.Normal,
			OnTeamStatisticsUpdated);

		HideUIs();
		SetupMousePanning();
		UpdateUiState(UIState.TowerPlacing);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (_simulationManager.IsPaused)
				ResumeGame();
			else
				PauseGame();
		}

		_battleUI.UpdateRemainingTime(GameOverview.TimeLeftFromPhase);
	}

	private void OnTeamStatisticsUpdated(TeamStatisticsUpdatedEvent e) {
		_battleUI.SetTeamStatistics(e.Team);
		_unitDeployment.UpdateDeployedUnitStatistics(e.Team);
	}

	private void OnTeamMoneyUpdated(TeamMoneyUpdatedEvent e) {
		int money = e.Team.Money;
		Logic.Data.Color color = e.Team.TeamColor;
		_towerPlacing.SetPlayerMoney(color, money);
		_unitDeployment.SetPlayerMoney(color, money);
		_battleUI.SetPlayerMoney(color, money);
	}

	private void OnCastleDestroyed(CastleDestroyedEvent e) {
		GameTeam winner = e.Castle.World.Overview.Teams.FirstOrDefault(team => !team.Castle.IsDestroyed);
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

	private void PauseGame() {
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

	private void OnUnitPurchased(IUnitData unitType) {
		var command = new PurchaseUnitCommand(GameOverview.GetTeam(_activePlayer), unitType);
		if (GameOverview.Commands.Issue(command)) _unitDeployment.UpdateBoughtUnitCount(unitType);
	}

	private void StartTowerPlacing(Logic.Data.Color player, bool resetUI) {
		if (resetUI) {
			_activePlayer = player;
			GameTeam playerData = GameOverview.GetTeam(_activePlayer);

			_selectedTowerType = null;

			_towerPlacing.ResetUI();
			_towerPlacing.SetActivePlayer(_activePlayer);
			_towerPlacing.SetPlayerMoney(playerData.TeamColor, playerData.Money);
		}

		_towerPlacing.Show();
	}

	private void StepTowerPlacing() {
		OnBuildingPossibleChanges?.Invoke(null);
		OnTowerSelected?.Invoke(null);

		if (_activePlayer == Logic.Data.Color.Blue)
			StartTowerPlacing(Logic.Data.Color.Red, true);
		else
			UpdateUiState(UIState.UnitDeployment);
	}

	private void OnTowerTypeSelected(ITowerData towerType) {
		_selectedTowerType = towerType;
		_towerPlacing.ShowTowerTypeStats(towerType);
		OnTowerSelected?.Invoke(null);
		OnBuildingPossibleChanges?.Invoke(GameOverview.GetTeam(_activePlayer));
	}

	private void OnTileSelected(TilePosition position, SimulationManager.MouseButton button) {
		if (_uiState == UIState.TowerPlacing) {
			if (button == SimulationManager.MouseButton.Left) HandleTowerPlacingTileSelection(position);
		} else if (_uiState == UIState.UnitDeployment) {
			HandleUnitDeploymentTileSelection(position, button);
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
			_selectedTowerType = null;
			OnBuildingPossibleChanges?.Invoke(null);
			OnTowerSelected?.Invoke(tower);
			Debug.Log($"[TowerPlacing]: A tower has been selected: {tower} at position {position}");
		} else if (_selectedTowerType != null) {
			GameOverview.Commands.Issue(new BuildTowerCommand(playerData, _selectedTowerType, position));
		}
	}

	private void StepUnitDeployment() {
		OnBarrackSelected?.Invoke(null);
		_selectedBarrack = null;
		if (_activePlayer == Logic.Data.Color.Blue) {
			StartUnitDeployment(Logic.Data.Color.Red, true);
		} else {
			UpdateUiState(UIState.Battle);
			var command = new AdvancePhaseCommand(GameOverview);
			if (GameOverview.CurrentPhase == GamePhase.Prepare && !GameOverview.Commands.Issue(command))
				Debug.LogError("[GamePhase] Failed to advance the GamePhase");
		}
	}

	private void StartUnitDeployment(Logic.Data.Color player, bool resetUI) {
		if (resetUI) {
			_activePlayer = player;
			GameTeam playerData = GameOverview.GetTeam(_activePlayer);

			_unitDeployment.SetActivePlayer(_activePlayer);
			_unitDeployment.SetPlayerMoney(player, playerData.Money);
			_unitDeployment.UpdateDeployedUnitStatistics(GameOverview.GetTeam(_activePlayer));
		}

		_unitDeployment.Show();
	}

	private void StartBattle() {
		_battleUI.Show();
		_battleUI.ShowPauseButton();
		foreach (GameTeam team in GameOverview.Teams) _battleUI.SetTeamStatistics(team);
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
		bool returnedFromPause = _uiState == UIState.Paused;
		if (_uiState != uiState) _lastUiState = _uiState;

		_uiState = uiState;
		switch (_uiState) {
			case UIState.TowerPlacing:
				HideUIs();
				StartTowerPlacing(Logic.Data.Color.Blue, !returnedFromPause);
				break;
			case UIState.UnitDeployment:
				HideUIs();
				StartUnitDeployment(Logic.Data.Color.Blue, !returnedFromPause);
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

	/// <summary>
	///     Invoked when a tower is selected, the event argument is null when a tower is deselected
	/// </summary>
	public event Action<Tower> OnTowerSelected;

	/// <summary>
	///     Invoked when the building possible area visualization should be updated
	/// </summary>
	public event Action<GameTeam> OnBuildingPossibleChanges;

	/// <summary>
	///     Invoked when a barrack is selected, the event argument is null when a barrack is deselected
	/// </summary>
	public event Action<Barrack> OnBarrackSelected;

	/// <summary>
	///     Invoked when the mouse enters the game area
	/// </summary>
	public event Action<MouseEnterEvent> OnGameViewMouseEnter;

	/// <summary>
	///     Invoked when the mouse leaves the game area
	/// </summary>
	public event Action<MouseLeaveEvent> OnGameViewMouseLeave;

	/// <summary>
	///     Invoked when a mouse button is pressed down on the game area
	/// </summary>
	public event Action<MouseDownEvent> OnGameViewMouseDown;

	/// <summary>
	///     Invoked when a mouse button is released on the game area
	/// </summary>
	public event Action<MouseUpEvent> OnGameViewMouseUp;

	/// <summary>
	///     Invoked when the mouse cursor moved on the game area
	/// </summary>
	public event Action<MouseMoveEvent> OnGameViewMouseMove;
}
}
