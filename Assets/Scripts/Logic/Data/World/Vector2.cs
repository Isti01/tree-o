using System;

namespace Logic.Data.World {

/// <summary>
/// Represents an indiscreet position in the world.
/// </summary>
public readonly struct Vector2 {
	/// <summary>
	/// First coordinate of the vector.
	/// </summary>
	public float X { get; }

	/// <summary>
	/// Second coordinate of the vector.
	/// </summary>
	public float Y { get; }

	/// <summary>
	/// Squared length of the vector.
	/// </summary>
	public float Length2 => X * X + Y * Y;

	/// <summary>
	/// Length of the vector;
	/// </summary>
	public float Length => (float) Math.Sqrt(Length2);

	/// <summary>
	/// Creates a new Vector.
	/// </summary>
	/// <param name="x">First coordinate of the vector.</param>
	/// <param name="y">Second coordinate of the vector.</param>
	public Vector2(float x, float y) {
		X = x;
		Y = y;
	}

	/// <returns>The TilePosition on which the Vector2 is.</returns>
	public TilePosition ToTilePosition() {
		return new TilePosition((int) Math.Floor(X), (int) Math.Floor(Y));
	}

	/// <summary>
	/// Calculates the squared distance of two Vector2s.
	/// </summary>
	/// <param name="other">The other Vector2.</param>
	/// <returns>The squared distance of the two Vector2s.</returns>
	public float Distance2(Vector2 other) {
		return (X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y);
	}

	/// <summary>
	/// Calculates the distance of two Vector2s.
	/// </summary>
	/// <param name="other">The other Vector2.</param>
	/// <returns>The distance of the two Vector2s.</returns>
	public float Distance(Vector2 other) {
		return (float) Math.Sqrt(Distance2(other));
	}

	/// <summary>
	/// Creates a new Vector2 by multiplying one with a scalar.
	/// </summary>
	/// <param name="multiplier">The scalar to multiply with.</param>
	/// <returns>the new multiplied Vector2.</returns>
	public Vector2 Multiplied(float multiplier) {
		return new Vector2(X * multiplier, Y * multiplier);
	}

	/// <summary>
	/// Makes a new Vector2 by adding two Vector2s.
	/// </summary>
	/// <param name="other">The other Vector2.</param>
	/// <returns>The new Vector2.</returns>
	public Vector2 Added(Vector2 other) {
		return Added(other.X, other.Y);
	}

	/// <summary>
	/// Makes a new Vector2 by adding coordinates to a Vector2.
	/// </summary>
	/// <param name="x">The first coordinate.</param>
	/// <param name="y">The second coordinate.</param>
	/// <returns>The new Vector2.</returns>
	public Vector2 Added(float x, float y) {
		return new Vector2(X + x, Y + y);
	}

	/// <summary>
	/// Makes a new Vector2 by subtracting two Vector2s.
	/// </summary>
	/// <param name="other">The other Vector2.</param>
	/// <returns>The new Vector2.</returns>
	public Vector2 Subtracted(Vector2 other) {
		return Subtracted(other.X, other.Y);
	}

	/// <summary>
	/// Makes a new Vector2 by subtracting coordinates from a Vector2.
	/// </summary>
	/// <param name="x">The first coordinate.</param>
	/// <param name="y">The second coordinate.</param>
	/// <returns>The new Vector2.</returns>
	public Vector2 Subtracted(float x, float y) {
		return Added(-x, -y);
	}

	/// <summary>
	/// Creates the perpendicular of the Vector2.
	/// </summary>
	/// <returns>The perpendicular of the Vector2.</returns>
	public Vector2 Perpendicular() {
		return new Vector2(-Y, X);
	}

	/// <summary>
	/// Normalizes the Vector2.
	/// </summary>
	/// <returns>The normalized Vector2.</returns>
	public Vector2 Normalized() {
		return Multiplied(1 / Length);
	}

	/// <summary>
	/// Formats the Vector2 for printing on screen.
	/// </summary>
	/// <returns>Printable format of the Vector2.</returns>
	public override string ToString() {
		return $"({X:F2};{Y:F2})";
	}

	/// <summary>
	/// Checks whether two Vector2s are the same.
	/// </summary>
	/// <param name="other">The other Vector2.</param>
	/// <returns>True if they are at the same position.$</returns>
	public bool EqualsWithThreshold(Vector2 other) {
		return Subtracted(other).Length < 0.001;
	}
}
}
