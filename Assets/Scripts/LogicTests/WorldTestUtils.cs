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
		GameWorld world = new GameWorld(overview.Object, config.Object);

		TestContext.Out.WriteLine("Generated world:");
		TestContext.Out.WriteLine(WorldAsMultilineString(world));
		return world;
	}

	private static string WorldAsMultilineString(GameWorld world) {
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
}

}
