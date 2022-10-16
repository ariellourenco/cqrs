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
            var entity = new SampleEntity(Guid.NewGuid());
            var @event = new Ping();

            // Act
            entity.AddEvent(@event);

            // Assert
            Assert.NotNull(entity.Events);
            Assert.Contains(@event, entity.Events);
        }

        [Fact]
        public void CanRemovesAllAddedEvents()
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
        public void ComparesSameIdentity_ReturnsTrue()
        {
            // Arrange
            var id = Guid.NewGuid();

            var entityLeft = new SampleEntity(id);
            var entityRight = new SampleEntity(id);

            // Act & Assert
            Assert.False(entityLeft != entityRight);
            Assert.False(entityRight != entityLeft);
            Assert.True(entityLeft == entityRight);
            Assert.True(entityRight == entityLeft);
            Assert.True(entityLeft.Equals(entityRight));
            Assert.True(entityRight.Equals(entityLeft));
        }

        [Fact]
        public void ComparesSameReference_ReturnsTrue()
        {
            // Arrange
            var id = Guid.NewGuid();

            var entityLeft = new SampleEntity(id);
            var entityRight = entityLeft;

            // Act & Assert
            Assert.False(entityLeft != entityRight);
            Assert.False(entityRight != entityLeft);
            Assert.True(entityLeft == entityRight);
            Assert.True(entityRight == entityLeft);
            Assert.True(entityLeft.Equals(entityRight));
            Assert.True(entityRight.Equals(entityLeft));
        }

        [Fact]
        public void ComparesDifferentIdentity_ReturnsFalse()
        {
            // Arrange
            var entityLeft = new SampleEntity(Guid.NewGuid());
            var entityRight = new SampleEntity(Guid.NewGuid());

            // Act & Assert
            Assert.False(entityLeft.Equals(entityRight));
            Assert.False(entityRight.Equals(entityLeft));
            Assert.False(entityLeft == entityRight);
            Assert.False(entityRight == entityLeft);
            Assert.True(entityLeft != entityRight);
            Assert.True(entityRight != entityLeft);
        }

        [Fact]
        public void ComparesDifferentRuntimeType_ReturnsFalse()
        {
            // Arrange
            var id = Guid.NewGuid();

            var entityLeft = new SampleEntity(id);
            var entityRight = new { };

            // Act & Assert
            Assert.False(entityLeft.Equals(entityRight));
            Assert.False(entityRight.Equals(entityLeft));
        }

        [Fact]
        public void ComparesTransientIdentity_ReturnsFalse()
        {
            // Arrange
            var entityLeft = new SampleEntity();
            var entityRight = new SampleEntity();

            // Act & Assert
            Assert.False(entityLeft.Equals(entityRight));
            Assert.False(entityRight.Equals(entityLeft));
            Assert.False(entityLeft == entityRight);
            Assert.False(entityRight == entityLeft);
            Assert.True(entityLeft != entityRight);
            Assert.True(entityRight != entityLeft);
        }

        [Fact]
        public void ComparesNullReference_ReturnsFalse()
        {
            // Arrange
            var id = Guid.NewGuid();

            var entityLeft = new SampleEntity(id);
            var entityRight = (SampleEntity)null;

            // Act & Assert
            Assert.False(entityLeft.Equals(entityRight));
            Assert.False(entityLeft == entityRight);
            Assert.False(entityRight == entityLeft);
            Assert.True(entityLeft != entityRight);
            Assert.True(entityRight != entityLeft);
        }

        [Fact]
        public void ComparesNullOperands_ReturnsTrue()
        {
            // Arrange
            var entityLeft = (SampleEntity)null;
            var entityRight = (SampleEntity)null;

            // Act & Assert
            Assert.False(entityLeft != entityRight);
            Assert.False(entityRight != entityLeft);
            Assert.True(entityLeft == entityRight);
            Assert.True(entityRight == entityLeft);
        }

        [Fact]
        public void EqualIdentity_ReturnsSameHashCode()
        {
            // Arrange
            var id = Guid.NewGuid();

            var entity1 = new SampleEntity(id);
            var entity2 = new SampleEntity(id);

            // Act & Assert
            Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
        }

        [Fact]
        public void DifferentIdentity_ReturnsDifferentHashCode()
        {
            // Arrange
            var entity1 = new SampleEntity(Guid.NewGuid());
            var entity2 = new SampleEntity(Guid.NewGuid());

            // Act & Assert
            Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
        }

        [Fact]
        public void TransientIdentity_ReturnsPredictableHashCode()
        {
            // Arrange
            var entity = new SampleEntity();
            var hashCode = 31;

            // Act & Assert
            Assert.Equal(hashCode, entity.GetHashCode());
        }

        private class SampleEntity : Entity<string>
        {
            public SampleEntity() { }

            public SampleEntity(Guid id) => Id = id.ToString();
        }

        private class Ping : INotification
        {
            public string Message { get; set; }
        }
    }
}