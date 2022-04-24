using System.Linq;
using Logic.Data;
using Logic.Data.World;
using Logic.Event.World.Unit;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Container of unit tests for the <see cref="Unit"/> class.
/// </summary>
public class UnitClassTest {
	[Test]
	public void TestInflictDamage() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Barrack barrack = overview.Teams.First().Barracks.First();

		barrack.QueueUnit(new GameTestUtils.UnitTypeData());
		overview.AdvancePhase();
		barrack.Spawn();

		bool eventCalled = false;
		overview.Events.AddListener<UnitDamagedEvent>(_ => eventCalled = true);

		Unit unit = overview.World.Units.First();
		float oldHealth = unit.CurrentHealth;
		float delta = oldHealth / 2 + 1;
		unit.InflictDamage(null, delta);

		Assert.IsTrue(eventCalled);
		Assert.AreEqual(oldHealth - delta, unit.CurrentHealth, 0.1f);
	}

	[Test]
	public void TestDestroyWithoutDamage() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Barrack barrack = overview.Teams.First().Barracks.First();

		barrack.QueueUnit(new GameTestUtils.UnitTypeData());
		overview.AdvancePhase();
		barrack.Spawn();

		bool eventCalled = false;
		overview.Events.AddListener<UnitDamagedEvent>(_ => eventCalled = true);

		Unit unit = overview.World.Units.First();
		unit.DestroyWithoutDamage();

		Assert.IsFalse(eventCalled);
		Assert.IsFalse(unit.IsAlive);
	}

	[Test]
	public void TestMove() {
		GameOverview overview = GameTestUtils.CreateOverview();
		Barrack barrack = overview.Teams.First().Barracks.First();

		barrack.QueueUnit(new GameTestUtils.UnitTypeData { Speed = 1 });
		overview.AdvancePhase();
		barrack.Spawn();

		bool eventCalled = false;
		overview.Events.AddListener<UnitMovedTileEvent>(_ => eventCalled = true);

		Unit unit = overview.World.Units.First();
		Vector2 oldPos = unit.Position;
		unit.Move(1);
		Assert.IsTrue(oldPos.Distance(unit.Position) < 1.1f);

		unit.Move(1);
		unit.Move(1);
		Assert.IsTrue(eventCalled);
	}
}

}
