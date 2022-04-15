using System;
using Logic.Data.World;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Presentation.World.Config {
[CreateAssetMenu(fileName = "New World Config", menuName = "World/Config/World Config", order = 1)]
public class WorldConfig : ScriptableObject, IGameWorldConfig {
	[NonSerialized]
	private int width;

	[SerializeField]
	[Min(8)]
	[Tooltip("Inclusive")]
	private int minWidth;

	[SerializeField]
	[Min(8)]
	[Tooltip("Inclusive")]
	private int maxWidth;

	[NonSerialized]
	private int height;

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

	public int Width {
		get {
			if (width == 0) width = Random.Range(minWidth, maxWidth + 1);
			return width;
		}
	}

	public int Height {
		get {
			if (height == 0) return height = Random.Range(minHeight, maxHeight + 1);
			return height;
		}
	}

	public float BarrackSpawnCooldownTime => barrackSpawnCooldownTime;
	public float CastleStartingHealth => castleStartingHealth;
	public int MaxBuildingDistance => maxBuildingDistance;
}
}
