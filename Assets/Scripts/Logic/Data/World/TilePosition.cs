using System;

namespace Logic.Data.World {
public readonly struct TilePosition {
	public int X { get; }
	public int Y { get; }

	public TilePosition(int x, int y) {
		X = x;
		Y = y;
	}

	public Vector2 ToVectorLower() {
		return new Vector2(X, Y);
	}

	public Vector2 ToVectorCentered() {
		return new Vector2(X + 0.5f, Y + 0.5f);
	}

	public int FirstNormDistance(TilePosition from) {
		return Math.Abs(X - from.X) + Math.Abs(Y - from.Y);
	}

	public int Distance2(TilePosition from) {
		return (X - from.X) * (X - from.X) + (Y - from.Y) * (Y - from.Y);
	}

	public float Distance(TilePosition from) {
		return (float) Math.Sqrt(Distance2(from));
	}

	public TilePosition Added(TilePosition other) {
		return Added(other.X, other.Y);
	}

	public TilePosition Added(int x, int y) {
		return new TilePosition(X + x, Y + y);
	}

	public TilePosition Subtracted(TilePosition other) {
		return Subtracted(other.X, other.Y);
	}

	public TilePosition Subtracted(int x, int y) {
		return Added(-x, -y);
	}

	public override string ToString() {
		return $"({X};{Y})";
	}
}
}
