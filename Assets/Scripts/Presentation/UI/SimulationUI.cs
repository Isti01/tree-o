using System;
using Logic.Command.Unit;
using Logic.Data;
using Presentation.World;
using UnityEngine;
using Color = UnityEngine.Color;
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
		_unitDeployment = GetComponentInChildren<UnitDeploymentUI>();

		_unitDeployment.SetTeamColors(teamRedColor, teamBlueColor);
		_unitDeployment.Hide();

		_unitDeployment.OnNextClicked += StepUnitDeployment;
		_unitDeployment.OnUnitPurchased += OnUnitPurchased;

		UpdateUiState(UIState.UnitDeployment); // TODO start with tower placing
	}

	private void OnDestroy() {
		_simulationManager.OnTileSelected -= OnTileSelected;
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

	private void OnTileSelected(Vector2 position) {
		// TODO deploy or inspect tower
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
}
}
