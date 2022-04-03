using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Presentation.UI {
public class BattleUI : MonoBehaviour {
	private const string Pause = "Pause";

	[SerializeField]
	private UIDocument ui;

	private VisualElement RootElement => ui.rootVisualElement;

	private void Start() {
		RootElement.Q<Button>(Pause).clicked += () => OnPauseClicked?.Invoke();
	}

	public void Show() {
		RootElement.style.display = DisplayStyle.Flex;
	}

	public void Hide() {
		RootElement.style.display = DisplayStyle.None;
	}

	public event Action OnPauseClicked;
}
}
