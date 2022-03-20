using System;

namespace Logic.Data.World {
public readonly struct Vector2 {
	public float X { get; }
	public float Y { get; }

	public float Length {
		get { return (float) Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2)); }
	}

	public Vector2(float x, float y) {
		X = x;
		Y = y;
	}

	public TilePosition ToTilePosition() {
		return new TilePosition((int) Math.Floor(X), (int) Math.Floor(Y));
	}

	public Vector2 Multiplied(float multiplier) {
		return new Vector2(X * multiplier, Y * multiplier);
	}

	public Vector2 Added(Vector2 other) {
		return new Vector2(X + other.X, Y + other.Y);
	}

	public override string ToString() {
		return $"({X:F2};{Y:F2})";
	}
}
}
