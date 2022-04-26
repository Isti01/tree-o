using System;
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

	private bool _active = true;

	private Button _exitButton;
	private Button _pauseButton;
	private VisualElement _playerBlueOverallStats;
	private VisualElement _playerBlueRoundStats;
	private VisualElement _playerRedOverallStats;
	private VisualElement _playerRedRoundStats;

	private VisualElement RootElement => ui.rootVisualElement;

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

	/// <summary>
	///     Sets the visibility of the pause button on the battle screen.
	///     The method hides the "Exit" button and shows the "Pause" button
	/// </summary>
	public void ShowPauseButton() {
		_pauseButton.style.display = DisplayStyle.Flex;
		_exitButton.style.display = DisplayStyle.None;
	}

	/// <summary>
	///     Sets the visibility of the exit button on the battle screen.
	///     The method hides the "Pause" button and shows the "Exit" button
	/// </summary>
	public void ShowExitButton() {
		_pauseButton.style.display = DisplayStyle.None;
		_exitButton.style.display = DisplayStyle.Flex;
	}

	/// <summary>
	///     Shows the battle screen
	/// </summary>
	public void Show() {
		_active = true;
		RootElement.style.display = DisplayStyle.Flex;
	}

	/// <summary>
	///     Hides the battle screen
	/// </summary>
	public void Hide() {
		_active = false;
		RootElement.style.display = DisplayStyle.None;
	}

	/// <summary>
	///     Sets the displayed money amount of the given team
	/// </summary>
	public void SetPlayerMoney(Color color, int money) {
		if (!_active) return;

		VisualElement stats = GetOverallStats(color);
		stats.Q<Label>(PlayerMoneyText).text = $"Money: {money}";
	}

	/// <summary>
	///     Sets both overall and round statistics of the given team
	/// </summary>
	public void SetTeamStatistics(GameTeam team) {
		if (!_active) return;

		SetOverallStats(team);
		SetRoundStats(team);
	}

	/// <summary>
	///     Sets round statistics of the given team
	/// </summary>
	private void SetRoundStats(GameTeam team) {
		VisualElement stats = GetRoundStats(team.TeamColor).Q(PlayerStatContainer);
		stats.Clear();

		string[] texts = { $"Active towers: {team.PresentTowerCount}", $"Alive Units: {team.AliveUnits}" };

		foreach (string text in texts) stats.Add(new Label { text = text });
	}

	/// <summary>
	///     Sets overall statistics of the given team
	/// </summary>
	private void SetOverallStats(GameTeam team) {
		VisualElement stats = GetOverallStats(team.TeamColor);
		stats.Q<Label>(PlayerRemainingUnitsText).text = $"Units purchased: {team.PurchasedUnitCount}";
		stats.Q<Label>(PlayerCastleHealthText).text = $"Castle Health: {team.Castle.Health}";
		stats.Q<Label>(PlayerDeployedTowersText).text = $"Built towers: {team.BuiltTowerCount}";
		SetPlayerMoney(team.TeamColor, team.Money);
	}

	/// <summary>
	///     Sets the remaining time
	/// </summary>
	public void UpdateRemainingTime(float timeLeft) {
		if (!_active) return;

		string text = $"{Mathf.Round(timeLeft * 100) / 100 + 0.001f}";
		text = text.Substring(0, text.Length - 1); // A small hack to keep the zeroes after the decimal point
		RootElement.Q<Label>(TimeLeftText).text = $"Time remaining: {text}s";
	}

	/// <summary>
	///     Invoked when the exit button is clicked
	/// </summary>
	public event Action OnExitClicked;

	/// <summary>
	///     Invoked when the pause button is clicked
	/// </summary>
	public event Action OnPauseClicked;
}
}
