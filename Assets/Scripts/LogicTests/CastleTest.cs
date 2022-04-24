using System;
using System.Linq;
using Logic.Data;
using Logic.Data.World;
using Logic.Event.World.Castle;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Container of unit tests for the <see cref="Castle"/> class.
/// </summary>
public class CastleTest {
	[Test]
	public void TestDamage() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Castle castle = overview.Teams.First().Castle;

		GameTeam unitTeam = overview.GetEnemyTeam(castle.Owner);
		overview.World.DeployUnit(unitTeam.Barracks.First(), new GameTestUtils.UnitTypeData());
		Unit unit = overview.World.Units.First();

		int eventsCalled = 0;
		overview.Events.AddListener<CastleDamagedEvent>(_ => eventsCalled++);

		float oldHealth = castle.Health;
		const float delta = 0.5f;
		castle.Damage(unit, delta);
		Assert.AreEqual(oldHealth - delta, castle.Health);
		Assert.AreEqual(1, eventsCalled);

		castle.Damage(unit, castle.Health + 1);
		Assert.AreEqual(2, eventsCalled);

		Assert.Throws<InvalidOperationException>(() => castle.Damage(unit, 1));
	}

	[Test]
	public void TestIsDestroyed() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Castle castle = overview.Teams.First().Castle;
		Assert.IsFalse(castle.IsDestroyed);

		GameTeam unitTeam = overview.GetEnemyTeam(castle.Owner);
		overview.World.DeployUnit(unitTeam.Barracks.First(), new GameTestUtils.UnitTypeData());
		Unit unit = overview.World.Units.First();

		int eventsCalled = 0;
		overview.Events.AddListener<CastleDamagedEvent>(_ => eventsCalled++);
		overview.Events.AddListener<CastleDestroyedEvent>(_ => eventsCalled++);

		castle.Damage(unit, castle.Health + 1);
		Assert.AreEqual(2, eventsCalled);
		Assert.IsTrue(castle.IsDestroyed);
	}
}

}
