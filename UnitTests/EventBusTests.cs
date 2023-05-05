using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicWallpaper.Tests
{
    [TestClass]
    public class EventBusTests
    {
        [TestMethod]
        public void Register_AddsNewEventHandlerList_WhenEventNameDoesNotExist()
        {
            // Arrange
            string eventName = "TestEvent";

            // Act
            EventBus.Register(eventName);

            // Assert
            Assert.IsTrue(EventBus.ContainsEvent(eventName));
        }

        [TestMethod]
        public void Subscribe_AddsEventHandlerToList_WhenEventNameExists()
        {
            // Arrange
            string eventName = "TestEvent";
            bool eventHandled = false;
            Action<CustomEventArgs> handler = (args) => { eventHandled = true; };

            EventBus.Register(eventName);

            // Act
            EventBus.Subscribe(eventName, handler);
            EventBus.Publish(eventName, new CustomEventArgs(null));

            // Assert
            Assert.IsTrue(eventHandled);
        }

        [TestMethod]
        public void Subscribe_ThrowsArgumentException_WhenEventNameDoesNotExist()
        {
            // Arrange
            string eventName = "NonExistentEvent";
            Action<CustomEventArgs> handler = (args) => { };

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => EventBus.Subscribe(eventName, handler));
        }

        [TestMethod]
        public void Publish_ThrowsArgumentException_WhenEventNameDoesNotExist()
        {
            // Arrange
            string eventName = "NonExistentEvent";
            CustomEventArgs eventArgs = new CustomEventArgs(null);

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => EventBus.Publish(eventName, eventArgs));
        }
    }
}