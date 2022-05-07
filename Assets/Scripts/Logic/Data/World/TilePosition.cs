using System;

namespace Logic.Data.World {
/// <summary>
/// Represents a tile's position on the grid.
/// </summary>
public readonly struct TilePosition {
	/// <summary>
	/// First coordinate of the position.
	/// </summary>
	public int X { get; }

	/// <summary>
	/// Second coordinate of the position.
	/// </summary>
	public int Y { get; }

	public TilePosition(int x, int y) {
		X = x;
		Y = y;
	}

	/// <returns>A <see cref="Vector2"/>pointing at the bottom-left edge of the tile.</returns>
	public Vector2 ToVectorLower() {
		return new Vector2(X, Y);
	}

	/// <returns>A <see cref="Vector2"/>pointing at the middle of the tile.</returns>
	public Vector2 ToVectorCentered() {
		return new Vector2(X + 0.5f, Y + 0.5f);
	}

	/// <summary>
	/// Calculates the manhattan-distance between two TilePositions.
	/// </summary>
	/// <param name="from">The other TilePosition.</param>
	/// <returns>The manhattan distance between the two TilePositions.</returns>
	public int FirstNormDistance(TilePosition from) {
		return Math.Abs(X - from.X) + Math.Abs(Y - from.Y);
	}

	/// <summary>
	/// Calculates the squared "real" distance between two TilePositions.
	/// </summary>
	/// <param name="from">The other TilePosition.</param>
	/// <returns>The squared "real" distance between the two TilePositions.</returns>
	public int Distance2(TilePosition from) {
		return (X - from.X) * (X - from.X) + (Y - from.Y) * (Y - from.Y);
	}

	/// <summary>
	/// Calculates the "real" distance between two TilePositions.
	/// </summary>
	/// <param name="from">The other TilePosition.</param>
	/// <returns>The "real" distance between the two TilePositions.</returns>
	public float Distance(TilePosition from) {
		return (float) Math.Sqrt(Distance2(from));
	}

	/// <summary>
	/// Makes a new TilePosition by adding two positions.
	/// </summary>
	/// <param name="other">The other position.</param>
	/// <returns>The new TilePosition.</returns>
	public TilePosition Added(TilePosition other) {
		return Added(other.X, other.Y);
	}

	/// <summary>
	/// Makes a new TilePosition by adding two positions.
	/// </summary>
	/// <param name="x">The other position's first coordinate.</param>
	/// <param name="y">The other position's second coordinate.</param>
	/// <returns>The new TilePosition.</returns>
	public TilePosition Added(int x, int y) {
		return new TilePosition(X + x, Y + y);
	}

	/// <summary>
	/// Makes a new TilePosition by subtracting two positions.
	/// </summary>
	/// <param name="other">The other position.</param>
	/// <returns>The new TilePosition.</returns>
	public TilePosition Subtracted(TilePosition other) {
		return Subtracted(other.X, other.Y);
	}

	/// <summary>
	/// Makes a new TilePosition by subtracting two positions.
	/// </summary>
	/// <param name="x">The other position's first coordinate.</param>
	/// <param name="y">The other position's second coordinate.</param>
	/// <returns>The new TilePosition.</returns>
	public TilePosition Subtracted(int x, int y) {
		return Added(-x, -y);
	}

	public override string ToString() {
		return $"({X};{Y})";
	}
}
}
