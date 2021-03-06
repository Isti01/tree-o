using System;
using NUnit.Framework;

namespace LogicTests {
/// <summary>
/// Utility methods regarding random number generation.
/// </summary>
public static class RandomUtils {
	private static readonly Random SeedSource = new Random();

	/// <summary>
	/// An alternative to calling <c>new Random()</c>.
	/// This method produces <see cref="Random"/> instances with different seeds
	/// even if this method is called multiple times within a few milliseconds
	/// (unlike <c>new Random()</c>, which may return instances with the same seed,
	/// depending on the .NET implementation).
	/// The seed of the returned instance is logged.
	/// </summary>
	/// <returns>a new random instance with a random seed</returns>
	public static Random CreateRandomlySeededRandom() {
		//mutex is necessary: random is not thread safe
		lock (SeedSource) {
			int seed = SeedSource.Next();
			TestContext.Out.WriteLine($"Seed of newly created Random instance: {seed}");
			return new Random(seed);
		}
	}
}
}
