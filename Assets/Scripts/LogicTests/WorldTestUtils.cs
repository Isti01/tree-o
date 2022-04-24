using System;
using System.Text;
using Logic.Data;
using Logic.Data.World;
using Moq;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Utility methods for <see cref="GameWorld"/> related operations.
/// </summary>
public static class WorldTestUtils {
	/// <summary>
	/// Creates a (procedurally) generated world. Its size (dimensions) are also random.
	/// Mocking is used to fulfill the dependencies of the <see cref="GameWorld"/> class.
	/// The generated world is logged to the console for debugging reasons.
	/// </summary>
	/// <returns>a new, randomly generated world</returns>
	public static GameWorld GenerateWorld() {
		Random random = RandomUtils.CreateRandomlySeededRandom();
		int width = random.Next(10, 15);
		int height = random.Next(10, 15);

		Mock<IGameOverview> overview = new Mock<IGameOverview>();
		overview.Setup(o => o.Random).Returns(random);

		Mock<IGameWorldConfig> config = new Mock<IGameWorldConfig>();
		config.Setup(c => c.BarrackSpawnCooldownTime).Returns(1);
		config.Setup(c => c.Width).Returns(width);
		config.Setup(c => c.Height).Returns(height);
		config.Setup(c => c.GenerateObstacles).Returns(true);
		GameWorld world = new GameWorld(overview.Object, config.Object);

		TestContext.Out.WriteLine("Generated world:");
		TestContext.Out.WriteLine(WorldAsMultilineString(world));
		return world;
	}

	/// <summary>
	/// Gets a multiline text that describes how the word looks. Useful for debugging.
	/// </summary>
	/// <param name="world">the world to visualize</param>
	/// <returns>a text that visualizes the world</returns>
	public static string WorldAsMultilineString(GameWorld world) {
		const char boundary = '#';
		StringBuilder result = new StringBuilder();
		result.Append(boundary, world.Width + 2);
		result.Append(Environment.NewLine);

		for (int y = 0; y < world.Height; y++) {
			result.Append(boundary);

			for (int x = 0; x < world.Width; x++) {
				result.Append(world.GetTile(x, y) switch {
					null => ' ',
					Castle c => c.OwnerColor == Color.Red ? 'R' : 'B',
					Barrack b => b.OwnerColor == Color.Red ? 'r' : 'b',
					Obstacle _ => 'X',
					Tower _ => 'T',
					var v => throw new Exception($"Unexpected object: {v}")
				});
			}

			result.Append(boundary);
			result.Append(Environment.NewLine);
		}

		result.Append(boundary, world.Width + 2);
		result.Append(Environment.NewLine);
		return result.ToString();
	}

	/// <summary>
	/// Finds an empty position on the specified world,
	/// throwing an exception if no empty position exists.
	/// </summary>
	/// <param name="world">the world in which to search</param>
	/// <returns>an empty tile's position</returns>
	public static TilePosition GetEmptyPosition(GameWorld world) {
		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {
				if (world[x, y] == null) return new TilePosition(x, y);
			}
		}

		throw new Exception("World has no empty positions");
	}
}

}
