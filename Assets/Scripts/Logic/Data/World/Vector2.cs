using System;

namespace Logic.Data.World {
public readonly struct Vector2 {
	public float X { get; }
	public float Y { get; }

	public float Length2 => X * X + Y * Y;
	public float Length => (float) Math.Sqrt(Length2);

	public Vector2(float x, float y) {
		X = x;
		Y = y;
	}

	public TilePosition ToTilePosition() {
		return new TilePosition((int) Math.Floor(X), (int) Math.Floor(Y));
	}

	public float Distance2(Vector2 other) {
		return (X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y);
	}

	public float Distance(Vector2 other) {
		return (float) Math.Sqrt(Distance2(other));
	}

	public Vector2 Multiplied(float multiplier) {
		return new Vector2(X * multiplier, Y * multiplier);
	}

	public Vector2 Added(Vector2 other) {
		return Added(other.X, other.Y);
	}

	public Vector2 Added(float x, float y) {
		return new Vector2(X + x, Y + y);
	}

	public Vector2 Subtracted(Vector2 other) {
		return Subtracted(other.X, other.Y);
	}

	public Vector2 Subtracted(float x, float y) {
		return Added(-x, -y);
	}

	public Vector2 Perpendicular() {
		return new Vector2(-Y, X);
	}

	public Vector2 Normalized() {
		return Multiplied(1 / Length);
	}

	public override string ToString() {
		return $"({X:F2};{Y:F2})";
	}

	public bool EqualsWithThreshold(Vector2 other) {
		return Subtracted(other).Length < 0.001;
	}
}
}
