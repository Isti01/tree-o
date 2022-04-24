using System;
using System.Linq;
using Logic.Data.World;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Container of unit tests for the <see cref="Barrack"/> class.
/// Most features are tested in the <see cref="BarrackUnitIntegrationTest "/>
/// and the <see cref="BarrackCheckpointTest"/> class.
/// </summary>
public class BarrackTest {
	[Test]
	public void TestQueuingAndSpawning() {
		Barrack barrack = GameTestUtils.CreateOverview().Teams.First().Barracks.First();
		IUnitTypeData unitType = new GameTestUtils.UnitTypeData();

		barrack.QueueUnit(unitType);
		Assert.AreEqual(1, barrack.QueuedUnits.Count);
		Assert.AreEqual(unitType, barrack.QueuedUnits.First());
		barrack.QueueUnit(new GameTestUtils.UnitTypeData());

		barrack.Spawn();
		Assert.IsTrue(barrack.IsOnCooldown);
		Assert.AreEqual(1, barrack.World.Units.Count);
		Assert.AreEqual(unitType, barrack.World.Units.First().Type);
		Assert.AreEqual(1, barrack.QueuedUnits.Count);

		barrack.ResetCooldown();
		barrack.Spawn();
		Assert.AreEqual(2, barrack.World.Units.Count);
		Assert.AreEqual(0, barrack.QueuedUnits.Count);

		Assert.Throws<InvalidOperationException>(barrack.Spawn);
	}

	[Test]
	public void TestPushNewCheckpoint() {
		Barrack barrack = GameTestUtils.CreateOverview().Teams.First().Barracks.First();
		TilePosition position = new TilePosition(4, 2);
		barrack.PushCheckPoint(position);
		Assert.AreEqual(1, barrack.CheckPoints.Count);
		Assert.AreEqual(position, barrack.CheckPoints.First());
	}

	[Test]
	public void TestPushExistingCheckpoint() {
		Barrack barrack = GameTestUtils.CreateOverview().Teams.First().Barracks.First();
		TilePosition position = new TilePosition(4, 2);
		barrack.PushCheckPoint(position);
		Assert.Throws<ArgumentException>(() => barrack.PushCheckPoint(position));
		Assert.AreEqual(1, barrack.CheckPoints.Count);
	}

	[Test]
	public void TestDeleteExistingCheckpoint() {
		Barrack barrack = GameTestUtils.CreateOverview().Teams.First().Barracks.First();
		TilePosition position = new TilePosition(4, 2);
		barrack.PushCheckPoint(position);
		barrack.PushCheckPoint(position.Added(1, 0));
		barrack.DeleteCheckPoint(position.Added(1, 0));
		Assert.AreEqual(1, barrack.CheckPoints.Count);
		Assert.AreEqual(position, barrack.CheckPoints.First());
	}

	[Test]
	public void TestDeleteNotACheckpoint() {
		Barrack barrack = GameTestUtils.CreateOverview().Teams.First().Barracks.First();
		TilePosition position = new TilePosition(4, 2);
		barrack.PushCheckPoint(position);
		Assert.Throws<ArgumentException>(() => barrack.DeleteCheckPoint(position.Added(1, 0)));
		Assert.AreEqual(position, barrack.CheckPoints.First());
	}
}

}
