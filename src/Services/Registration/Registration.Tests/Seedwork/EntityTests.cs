using System;
using Xunit;

namespace Registration
{
    public sealed class EntityTests
    {
        [Fact]
        public void CanAddDomainEvents()
        {
            // Arrange
            var entity = new SampleEntity();
            var @event = new {};

            // Act
            entity.AddEvent(@event);

            // Assert
            Assert.Contains(@event, entity.DomainEvents);
        }

        [Fact]
        public void CanClearDomainEvents()
        {
            // Arrange
            var entity = new SampleEntity();
            var @event = new {};

            entity.AddEvent(@event);

            // Act
            entity.ClearEvents();

            // Assert
            Assert.Empty(entity.DomainEvents);
        }

        [Fact]
        public void Equals_CompareSameIdentity_ReturnTrue()
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
        public void Equals_CompareSameReference_ReturnTrue()
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
        public void Equals_CompareDifferentIdentity_ReturnFalse()
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
        public void Equals_CompareDifferentRuntimeType_ReturnFalse()
        {
            // Arrange
            var entityLeft = new SampleEntity();
            var entityRight = new {};

            // Act & Assert
            Assert.False(entityLeft.Equals(entityRight));
            Assert.False(entityRight.Equals(entityLeft));
        }

        [Fact]
        public void Equals_CompareNullReference_ReturnFalse()
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
        public void Equals_CompareWhenBothOperandsAreNull_ReturnTrue()
        {
            // Arrange
            var entityLeft = (SampleEntity)null;
            var entityRight = (SampleEntity)null;

            // Act & Assert
            Assert.True(entityLeft == entityRight);
            Assert.True(entityRight == entityLeft);
        }

        [Fact]
        public void DifferentOperator_CompareSameIdentity_ReturnFalse()
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
        public void DifferentOperator_CompareDifferentIdentity_ReturnTrue()
        {
            // Arrange
            var entityLeft = new SampleEntity();
            var entityRight = new SampleEntity();

            // Act & Assert
            Assert.True(entityLeft != entityRight);
            Assert.True(entityRight != entityLeft);
        }

        [Fact]
        public void GetHashCode_ForEqualIdentity_ReturnSameHashCode()
        {
            // Arrange
            var id = Guid.NewGuid();

            var entity1 = new SampleEntity(id);
            var entity2 = new SampleEntity(id);

            // Act & Assert
            Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_ForDifferentIdentity_ReturnDifferentHashCode()
        {
            // Arrange
            var entity1 = new SampleEntity();
            var entity2 = new SampleEntity();

            // Act & Assert
            Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
        }

        private class SampleEntity : Entity
        {

            public SampleEntity()
            {
            }

            public SampleEntity(Guid id) => Id = id;
        }
    }
}
