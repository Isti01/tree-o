using System;

namespace Logic.Data.World {

public readonly struct Vector2 {
	public float X { get; }
	public float Y { get; }

	public Vector2(float x, float y) {
		X = x;
		Y = y;
	}

	public TilePosition ToTilePosition() {
		return new TilePosition((int) Math.Floor(X), (int) Math.Floor(Y));
	}

	public override string ToString() {
		return $"({X:F2};{Y:F2})";
	}
}

}
