using System;
using Logic.Event;
using NUnit.Framework;

namespace LogicTests {

/// <summary>
/// Tests the full functionality of the <see cref="EventDispatcher"/> class.
/// </summary>
public class EventDispatcherTest {
	[Test]
	public void TestAddingListener() {
		EventDispatcher dispatcher = NewErrorRethrowingDispatcher();
		EventDispatcher.Listener<BaseEvent> listener = _ => {};
		dispatcher.AddListener<BicycleEvent>(listener);
		dispatcher.AddListener<BicycleEvent>(listener);
		dispatcher.AddListener<AstraEvent>(_ => {});
	}

	[Test]
	public void TestRemovingListener() {
		EventDispatcher dispatcher = NewErrorRethrowingDispatcher();
		EventDispatcher.Listener<BaseEvent> listener = _ => {};
		Assert.Throws<EventDispatcher.IllegalListenerStateException>(() =>
			dispatcher.RemoveListener<BicycleEvent>(EventDispatcher.Ordering.Normal, listener));
		dispatcher.AddListener<BicycleEvent>(listener);
		dispatcher.AddListener<BicycleEvent>(listener);
		dispatcher.AddListener<BicycleEvent>(EventDispatcher.Ordering.First, listener);
		dispatcher.RemoveListener<BicycleEvent>(EventDispatcher.Ordering.Normal, listener);
		dispatcher.RemoveListener<BicycleEvent>(EventDispatcher.Ordering.Normal, listener);
		Assert.Throws<EventDispatcher.IllegalListenerStateException>(() =>
			dispatcher.RemoveListener<BicycleEvent>(EventDispatcher.Ordering.Normal, listener));
		dispatcher.RemoveListener<BicycleEvent>(EventDispatcher.Ordering.First, listener);
		Assert.Throws<EventDispatcher.IllegalListenerStateException>(() =>
			dispatcher.RemoveListener<BicycleEvent>(EventDispatcher.Ordering.First, listener));
	}

	[Test]
	public void TestInvocationNoListeners() {
		EventDispatcher dispatcher = NewErrorRethrowingDispatcher();
		dispatcher.Raise(new BicycleEvent());
		dispatcher.Raise(new AstraEvent());
		dispatcher.Raise(new PriusEvent());
	}

	[Test]
	public void TestInvocationExactListener() {
		EventDispatcher dispatcher = NewErrorRethrowingDispatcher();
		var called = false;
		dispatcher.AddListener<BicycleEvent>(_ => called = true);
		dispatcher.Raise(new BicycleEvent());
		Assert.IsTrue(called);
	}

	[Test]
	public void TestInvocationMultipleSameListeners() {
		EventDispatcher dispatcher = NewErrorRethrowingDispatcher();
		var counter = 0;
		EventDispatcher.Listener<BicycleEvent> listener = _ => counter++;
		dispatcher.AddListener(listener);
		dispatcher.AddListener(listener);
		dispatcher.Raise(new BicycleEvent());
		Assert.AreEqual(2, counter);
	}

	[Test]
	public void TestInvocationPolymorphListeners() {
		EventDispatcher dispatcher = NewErrorRethrowingDispatcher();
		var counter = 0;
		// ReSharper disable once AccessToModifiedClosure
		dispatcher.AddListener<BaseEvent>(_ => counter++);
		dispatcher.AddListener<BicycleEvent>(_ => counter++);
		dispatcher.AddListener<CarAbstractEvent>(_ => Assert.Fail());
		dispatcher.AddListener<AstraEvent>(_ => Assert.Fail());
		dispatcher.AddListener<PriusEvent>(_ => Assert.Fail());
		dispatcher.Raise(new BicycleEvent());
		Assert.AreEqual(2, counter);

		dispatcher = NewErrorRethrowingDispatcher();
		counter = 0;
		dispatcher.AddListener<BaseEvent>(_ => counter++);
		dispatcher.AddListener<BicycleEvent>(_ => Assert.Fail());
		dispatcher.AddListener<CarAbstractEvent>(_ => counter++);
		dispatcher.AddListener<AstraEvent>(_ => counter++);
		dispatcher.AddListener<PriusEvent>(_ => Assert.Fail());
		dispatcher.Raise(new AstraEvent());
		Assert.AreEqual(3, counter);

		dispatcher = NewErrorRethrowingDispatcher();
		counter = 0;
		dispatcher.AddListener<BaseEvent>(_ => counter++);
		dispatcher.AddListener<BicycleEvent>(_ => Assert.Fail());
		dispatcher.AddListener<CarAbstractEvent>(_ => counter++);
		dispatcher.AddListener<AstraEvent>(_ => Assert.Fail());
		dispatcher.AddListener<PriusEvent>(_ => counter++);
		dispatcher.Raise(new PriusEvent());
		Assert.AreEqual(3, counter);
	}

	[Test]
	public void TestErrorHandlerSingleError() {
		var called = false;
		EventDispatcher dispatcher = new EventDispatcher(errors => {
			Assert.AreEqual(1, errors.Count);
			called = true;
		});
		dispatcher.AddListener<BicycleEvent>(_ => throw new Exception("Testing"));
		dispatcher.Raise(new BicycleEvent());
		Assert.IsTrue(called);
	}

	[Test]
	public void TestErrorHandlerMultipleErrors() {
		var called = false;
		EventDispatcher dispatcher = new EventDispatcher(errors => {
			Assert.AreEqual(3, errors.Count);
			called = true;
		});
		dispatcher.AddListener<BicycleEvent>(_ => throw new Exception("Testing"));
		dispatcher.AddListener<BaseEvent>(_ => throw new Exception("Testing"));
		dispatcher.AddListener<BicycleEvent>(_ => throw new Exception("Testing"));
		dispatcher.Raise(new BicycleEvent());
		Assert.IsTrue(called);
	}

	[Test]
	public void TestInvocationOrder() {
		EventDispatcher dispatcher = NewErrorRethrowingDispatcher();
		var concat = "-";
		// ReSharper disable once AccessToModifiedClosure
		dispatcher.AddListener<BicycleEvent>(_ => concat += "3");
		dispatcher.AddListener<BaseEvent>(EventDispatcher.Ordering.Normal, _ => concat += "3");
		dispatcher.AddListener<BaseEvent>(EventDispatcher.Ordering.Sooner, _ => concat += "2");
		dispatcher.AddListener<BicycleEvent>(EventDispatcher.Ordering.Last, _ => concat += "5");
		dispatcher.AddListener<BicycleEvent>(EventDispatcher.Ordering.First, _ => concat += "1");
		dispatcher.AddListener<BicycleEvent>(EventDispatcher.Ordering.Later, _ => concat += "4");
		dispatcher.Raise(new BicycleEvent());
		Assert.AreEqual("-123345", concat);
	}

	private static EventDispatcher NewErrorRethrowingDispatcher() {
		return new EventDispatcher(errors => throw new AggregateException(errors));
	}

	private class BicycleEvent : BaseEvent {}

	private abstract class CarAbstractEvent : BaseEvent {}

	private class AstraEvent : CarAbstractEvent {}

	private class PriusEvent : CarAbstractEvent {}
}

}
