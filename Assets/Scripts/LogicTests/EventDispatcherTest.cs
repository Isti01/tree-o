using System;
using Logic.Event;
using NUnit.Framework;

namespace LogicTests {

public class EventDispatcherTest {
	[Test]
	public void TestAddingListener() {
		EventDispatcher dispatcher = NewErrorRethrowingDispatcher();
		EventDispatcher.Listener<EventBase> listener = _ => {};
		dispatcher.AddListener<BicycleEvent>(listener);
		dispatcher.AddListener<BicycleEvent>(listener);
		dispatcher.AddListener<AstraEvent>(_ => {});
	}

	[Test]
	public void TestRemovingListener() {
		EventDispatcher dispatcher = NewErrorRethrowingDispatcher();
		EventDispatcher.Listener<EventBase> listener = _ => {};
		Assert.Throws<EventDispatcher.IllegalListenerStateException>(() =>
			dispatcher.RemoveListener<BicycleEvent>(listener));
		dispatcher.AddListener<BicycleEvent>(listener);
		dispatcher.AddListener<BicycleEvent>(listener);
		dispatcher.RemoveListener<BicycleEvent>(listener);
		dispatcher.RemoveListener<BicycleEvent>(listener);
		Assert.Throws<EventDispatcher.IllegalListenerStateException>(() =>
			dispatcher.RemoveListener<BicycleEvent>(listener));
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
		dispatcher.AddListener<EventBase>(_ => counter++);
		dispatcher.AddListener<BicycleEvent>(_ => counter++);
		dispatcher.AddListener<CarAbstractEvent>(_ => Assert.Fail());
		dispatcher.AddListener<AstraEvent>(_ => Assert.Fail());
		dispatcher.AddListener<PriusEvent>(_ => Assert.Fail());
		dispatcher.Raise(new BicycleEvent());
		Assert.AreEqual(2, counter);

		dispatcher = NewErrorRethrowingDispatcher();
		counter = 0;
		dispatcher.AddListener<EventBase>(_ => counter++);
		dispatcher.AddListener<BicycleEvent>(_ => Assert.Fail());
		dispatcher.AddListener<CarAbstractEvent>(_ => counter++);
		dispatcher.AddListener<AstraEvent>(_ => counter++);
		dispatcher.AddListener<PriusEvent>(_ => Assert.Fail());
		dispatcher.Raise(new AstraEvent());
		Assert.AreEqual(3, counter);

		dispatcher = NewErrorRethrowingDispatcher();
		counter = 0;
		dispatcher.AddListener<EventBase>(_ => counter++);
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
		dispatcher.AddListener<EventBase>(_ => throw new Exception("Testing"));
		dispatcher.AddListener<BicycleEvent>(_ => throw new Exception("Testing"));
		dispatcher.Raise(new BicycleEvent());
		Assert.IsTrue(called);
	}

	private static EventDispatcher NewErrorRethrowingDispatcher() {
		return new EventDispatcher(errors => throw new AggregateException(errors));
	}

	private class BicycleEvent : EventBase {}

	private abstract class CarAbstractEvent : EventBase {}

	private class AstraEvent : CarAbstractEvent {}

	private class PriusEvent : CarAbstractEvent {}
}

}
