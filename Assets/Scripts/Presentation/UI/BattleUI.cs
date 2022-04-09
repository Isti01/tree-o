using System;
using System.Linq;
using Logic.Data;
using UnityEngine;
using UnityEngine.UIElements;
using Color = Logic.Data.Color;

namespace Presentation.UI {
[RequireComponent(typeof(UIDocument))]
public class BattleUI : MonoBehaviour {
	private const string Pause = "Pause";
	private const string Exit = "Exit";
	private const string PlayerBlueRoundStats = "PlayerBlueRoundStats";
	private const string PlayerRedRoundStats = "PlayerRedRoundStats";
	private const string TimeLeftText = "TimeLeftText";
	private const string PlayerStatContainer = "PlayerStatContainer";
	private const string PlayerBlueOverallStats = "PlayerBlueOverallStats";
	private const string PlayerRedOverallStats = "PlayerRedOverallStats";
	private const string PlayerMoneyText = "PlayerMoneyText";
	private const string PlayerRemainingUnitsText = "PlayerRemainingUnitsText";
	private const string PlayerCastleHealthText = "PlayerCastleHealthText";
	private const string PlayerDeployedTowersText = "PlayerDeployedTowersText";

	[SerializeField]
	private UIDocument ui;

	private Button _exitButton;
	private Button _pauseButton;

	private VisualElement RootElement => ui.rootVisualElement;
	private VisualElement _playerBlueOverallStats;
	private VisualElement _playerRedOverallStats;
	private VisualElement _playerBlueRoundStats;
	private VisualElement _playerRedRoundStats;

	private bool _active = true;

	private void Awake() {
		_pauseButton = RootElement.Q<Button>(Pause);
		_pauseButton.clicked += () => OnPauseClicked?.Invoke();

		_exitButton = RootElement.Q<Button>(Exit);
		_exitButton.clicked += () => OnExitClicked?.Invoke();

		_playerBlueOverallStats = RootElement.Q(PlayerBlueOverallStats);
		_playerRedOverallStats = RootElement.Q(PlayerRedOverallStats);
		_playerBlueRoundStats = RootElement.Q(PlayerBlueRoundStats);
		_playerRedRoundStats = RootElement.Q(PlayerRedRoundStats);

		ShowPauseButton();
	}

	private VisualElement GetOverallStats(Color player) {
		return player == Color.Red ? _playerRedOverallStats : _playerBlueOverallStats;
	}

	private VisualElement GetRoundStats(Color player) {
		return player == Color.Red ? _playerRedRoundStats : _playerBlueRoundStats;
	}

	public void ShowPauseButton() {
		Debug.Log("Shown pause");
		_pauseButton.style.display = DisplayStyle.Flex;
		_exitButton.style.display = DisplayStyle.None;
	}

	public void ShowExitButton() {
		_pauseButton.style.display = DisplayStyle.None;
		_exitButton.style.display = DisplayStyle.Flex;
	}

	public void Show() {
		_active = true;
		RootElement.style.display = DisplayStyle.Flex;
	}

	public void Hide() {
		_active = false;
		RootElement.style.display = DisplayStyle.None;
	}

	public void SetPlayerMoney(Color color, int money) {
		if (!_active) return;

		var stats = GetOverallStats(color);
		stats.Q<Label>(PlayerMoneyText).text = $"Money: {money}";
	}

	public void SetTeamStatistics(GameTeam team) {
		if (!_active) return;

		SetOverallStats(team);
		SetRoundStats(team);
	}

	private void SetRoundStats(GameTeam team) {
		var stats = GetRoundStats(team.TeamColor).Q(PlayerStatContainer);
		stats.Clear();

		var texts = new[] {
			$"Active towers: {team.PresentTowerCount}", $"Alive Units: {team.AliveUnits}"
		};

		foreach (string text in texts) {
			stats.Add(new Label { text = text });
		}
	}

	private void SetOverallStats(GameTeam team) {
		var stats = GetOverallStats(team.TeamColor);
		stats.Q<Label>(PlayerRemainingUnitsText).text = $"Units purchased: {team.PurchasedUnitCount}";
		stats.Q<Label>(PlayerCastleHealthText).text = $"Castle Health: {team.Castle.Health}";
		stats.Q<Label>(PlayerDeployedTowersText).text = $"Built towers: {team.BuiltTowerCount}";
		SetPlayerMoney(team.TeamColor, team.Money);
	}

	public void UpdateRemainingTime(float timeLeft) {
		if (!_active) return;

		var text = $"{Mathf.Round(timeLeft * 100) / 100 + 0.001f}";
		text = text.Substring(0, text.Length - 1); // A small hack to keep the zeroes after the decimal point
		RootElement.Q<Label>(TimeLeftText).text = $"Time remaining: {text}s";
	}

	public event Action OnExitClicked;

	public event Action OnPauseClicked;
}
}
