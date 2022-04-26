using System;
using Logic.Data.World;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Presentation.World.Config {
/// <summary>
///     Enables the World related settings to be configured in the Unity Editor
/// </summary>
[CreateAssetMenu(fileName = "New World Config", menuName = "World/Config/World Config", order = 1)]
public class WorldConfig : ScriptableObject, IGameWorldConfig {
	[SerializeField]
	[Min(8)]
	[Tooltip("Inclusive")]
	private int minWidth;

	[SerializeField]
	[Min(8)]
	[Tooltip("Inclusive")]
	private int maxWidth;

	[SerializeField]
	[Min(8)]
	[Tooltip("Inclusive")]
	private int minHeight;

	[SerializeField]
	[Min(8)]
	[Tooltip("Inclusive")]
	private int maxHeight;

	[SerializeField]
	[Min(0.01f)]
	private float barrackSpawnCooldownTime;

	[SerializeField]
	[Min(0.01f)]
	private float castleStartingHealth;

	[SerializeField]
	[Min(0)]
	private int maxBuildingDistance;

	[NonSerialized]
	private int _height;

	[NonSerialized]
	private int _width;

	public int Width {
		get {
			if (_width == 0) _width = Random.Range(minWidth, maxWidth + 1);
			return _width;
		}
	}

	public int Height {
		get {
			if (_height == 0) return _height = Random.Range(minHeight, maxHeight + 1);
			return _height;
		}
	}

	public float BarrackSpawnCooldownTime => barrackSpawnCooldownTime;
	public float CastleStartingHealth => castleStartingHealth;
	public int MaxBuildingDistance => maxBuildingDistance;
	public bool GenerateObstacles => true;
}
}
