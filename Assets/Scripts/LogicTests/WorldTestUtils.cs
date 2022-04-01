using System;
using System.Text;
using Logic.Data;
using Logic.Data.World;
using Moq;
using NUnit.Framework;

namespace LogicTests {
public static class WorldTestUtils {
	public static GameWorld GenerateWorld() {
		Random random = new Random();
		int width = random.Next(10, 15);
		int height = random.Next(10, 15);

		Mock<IGameOverview> overview = new Mock<IGameOverview>();
		overview.Setup(o => o.Random).Returns(new Random());
		GameWorld world = new GameWorld(overview.Object, new WorldConfig(), width, height);

		TestContext.Out.WriteLine("Generated world:");
		TestContext.Out.WriteLine(WorldAsMultilineString(world));
		return world;
	}

	private static string WorldAsMultilineString(GameWorld world) {
		const char boundary = '#';
		StringBuilder result = new StringBuilder();
		result.Append(boundary, world.Width + 2);
		result.Append(Environment.NewLine);

		for (var y = 0; y < world.Height; y++) {
			result.Append(boundary);

			for (var x = 0; x < world.Width; x++) {
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

	private class WorldConfig : IGameWorldConfig {
		public float BarrackSpawnCooldownTime => 1;
		public float CastleStartingHealth => 1;
	}
}
}
