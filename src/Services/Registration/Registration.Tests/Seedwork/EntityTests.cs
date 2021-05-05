using System;
using MediatR;
using Xunit;

namespace CQRSJourney.Registration
{
    public sealed class EntityTests
    {
        [Fact]
        public void CanAddEvents()
        {
            // Arrange
            var entity = new SampleEntity();
            var @event = new Ping();

            // Act
            entity.AddEvent(@event);

            // Assert
            Assert.NotNull(entity.Events);
            Assert.Contains(@event, entity.Events);
        }

        [Fact]
        public void ClearEvents_DoesNothing_IfNoEventsAreAdded()
        {
            // Arrange
            var entity = new SampleEntity();

            // Act
            entity.ClearEvents();

            // Assert
            Assert.Null(entity.Events);
        }

        [Fact]
        public void ClearEvents_RemovesAllEventsAdded()
        {
            // Arrange
            var entity = new SampleEntity();
            var @event = new Ping();

            // Act
            entity.AddEvent(@event);
            entity.ClearEvents();

            // Assert
            Assert.Empty(entity.Events);
        }

        [Fact]
        public void Equals_ComparesSameIdentity_ReturnsTrue()
        {
            // Arrange
            var id = Guid.NewGuid();

            var entityLeft = new SampleEntity(id);
            var entityRight = new SampleEntity(id);

            // Act & Assert
            Assert.True(entityLeft.Equals(entityRight));
            Assert.True(entityRight.Equals(entityLeft));
            Assert.True(entityLeft == entityRight);
            Assert.True(entityRight == entityLeft);
        }

        [Fact]
        public void Equals_ComparesSameReference_ReturnsTrue()
        {
            // Arrange
            var entityLeft = new SampleEntity();
            var entityRight = entityLeft;

            // Act & Assert
            Assert.True(entityLeft.Equals(entityRight));
            Assert.True(entityRight.Equals(entityLeft));
            Assert.True(entityLeft == entityRight);
            Assert.True(entityRight == entityLeft);
        }

        [Fact]
        public void Equals_ComparesDifferentIdentity_ReturnsFalse()
        {
            // Arrange
            var entityLeft = new SampleEntity();
            var entityRight = new SampleEntity();

            // Act & Assert
            Assert.False(entityLeft.Equals(entityRight));
            Assert.False(entityRight.Equals(entityLeft));
            Assert.False(entityLeft == entityRight);
            Assert.False(entityRight == entityLeft);
        }

        [Fact]
        public void Equals_ComparesDifferentRuntimeType_ReturnsFalse()
        {
            // Arrange
            var entityLeft = new SampleEntity();
            var entityRight = new {};

            // Act & Assert
            Assert.False(entityLeft.Equals(entityRight));
            Assert.False(entityRight.Equals(entityLeft));
        }

        [Fact]
        public void Equals_ComparesNullReference_ReturnsFalse()
        {
            // Arrange
            var entityLeft = new SampleEntity();
            var entityRight = (SampleEntity)null;

            // Act & Assert
            Assert.False(entityLeft.Equals(entityRight));
            Assert.False(entityLeft == entityRight);
            Assert.False(entityRight == entityLeft);
        }

        [Fact]
        public void Equals_ComparesWhenBothOperandsAreNull_ReturnsTrue()
        {
            // Arrange
            var entityLeft = (SampleEntity)null;
            var entityRight = (SampleEntity)null;

            // Act & Assert
            Assert.True(entityLeft == entityRight);
            Assert.True(entityRight == entityLeft);
        }

        [Fact]
        public void DifferentOperator_ComparesSameIdentity_ReturnsFalse()
        {
            // Arrange
            var id = Guid.NewGuid();

            var entityLeft = new SampleEntity(id);
            var entityRight = new SampleEntity(id);

            // Act & Assert
            Assert.False(entityLeft != entityRight);
            Assert.False(entityRight != entityLeft);
        }

        [Fact]
        public void DifferentOperator_ComparesDifferentIdentity_ReturnsTrue()
        {
            // Arrange
            var entityLeft = new SampleEntity();
            var entityRight = new SampleEntity();

            // Act & Assert
            Assert.True(entityLeft != entityRight);
            Assert.True(entityRight != entityLeft);
        }

        [Fact]
        public void GetHashCode_ForEqualIdentity_ReturnsSameHashCode()
        {
            // Arrange
            var id = Guid.NewGuid();

            var entity1 = new SampleEntity(id);
            var entity2 = new SampleEntity(id);

            // Act & Assert
            Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_ForDifferentIdentity_ReturnsDifferentHashCode()
        {
            // Arrange
            var entity1 = new SampleEntity();
            var entity2 = new SampleEntity();

            // Act & Assert
            Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
        }

        private class SampleEntity : Entity
        {
            public SampleEntity() { }

            public SampleEntity(Guid id) => Id = id;
        }

        private class Ping : INotification
        {
            public string Message { get; set; }
        }
    }
}
