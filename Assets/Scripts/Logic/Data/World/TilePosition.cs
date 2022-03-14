﻿using System;

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

	public int Distance2(TilePosition from) {
		return (X - from.X) * (X - from.X) + (Y - from.Y) * (Y - from.Y);
	}

	public double Distance(TilePosition from) {
		return Math.Sqrt(Distance2(from));
	}

	public override string ToString() {
		return $"({X};{Y})";
	}
}

}
