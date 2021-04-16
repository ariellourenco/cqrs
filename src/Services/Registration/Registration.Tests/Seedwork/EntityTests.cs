using System;
using Xunit;

namespace Registration
{
    public sealed class EntityTests
    {
        public class EqualsMethod
        {
            [Fact]
            public void ReturnsTrueForSameIdentity()
            {
                // Arrange
                var id = Guid.NewGuid();

                var entityLeft = new SampleEntity(id);
                var entityRight = new SampleEntity(id);

                // Act & Assert
                Assert.True(entityLeft.Equals(entityRight));
                Assert.True(entityRight.Equals(entityLeft));
            }

            [Fact]
            public void ReturnsTrueForSameReference()
            {
                // Arrange
                var entityLeft = new SampleEntity();
                var entityRight = entityLeft;

                // Act & Assert
                Assert.True(entityLeft.Equals(entityRight));
                Assert.True(entityRight.Equals(entityLeft));
            }

            [Fact]
            public void ReturnsFalseForDifferentIdentity()
            {
                // Arrange
                var entityLeft = new SampleEntity();
                var entityRight = new SampleEntity();

                // Act & Assert
                Assert.False(entityLeft.Equals(entityRight));
                Assert.False(entityRight.Equals(entityLeft));
            }

            [Fact]
            public void ReturnsFalseForDifferentRuntimeType()
            {
                // Arrange
                var entityLeft = new SampleEntity();
                var entityRight = new {};

                // Act & Assert
                Assert.False(entityLeft.Equals(entityRight));
                Assert.False(entityRight.Equals(entityLeft));
            }

            [Fact]
            public void ReturnsFalseForNullReference()
            {
                // Arrange
                var entity = new SampleEntity();

                // Act & Assert
                Assert.False(entity.Equals((Entity)null));
            }
        }

        public class EqualsOperator
        {
            [Fact]
            public void ReturnsTrueForSameIdentity()
            {
                // Arrange
                var id = Guid.NewGuid();

                var entityLeft = new SampleEntity(id);
                var entityRight = new SampleEntity(id);

                // Act & Assert
                Assert.True(entityLeft == entityRight);
                Assert.True(entityRight == entityLeft);
            }

            [Fact]
            public void ReturnsFalseForDifferentIdentity()
            {
                // Arrange
                var entityLeft = new SampleEntity();
                var entityRight = new SampleEntity();

                // Act & Assert
                Assert.False(entityLeft == entityRight);
                Assert.False(entityRight == entityLeft);
            }

            [Fact]
            public void ReturnsFalseForEitherOperandNull()
            {
                // Arrange
                var entityLeft = (Entity)null;
                var entityRight = new SampleEntity();

                // Act & Assert
                Assert.False(entityLeft == entityRight);
                Assert.False(entityRight == entityLeft);
            }

            [Fact]
            public void ReturnsTrueForBothOperandNull()
            {
                // Arrange
                var entityLeft = (Entity)null;
                var entityRight = (Entity)null;

                // Act & Assert
                Assert.True(entityLeft == entityRight);
                Assert.True(entityRight == entityLeft);
            }
        }

        public class DifferentOperator
        {
            [Fact]
            public void ReturnsFalseForSameIdentity()
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
            public void ReturnsTrueForDifferentIdentity()
            {
                // Arrange
                var entityLeft = new SampleEntity();
                var entityRight = new SampleEntity();

                // Act & Assert
                Assert.True(entityLeft != entityRight);
                Assert.True(entityRight != entityLeft);
            }

            [Fact]
            public void ReturnsTrueForEitherOperandNull()
            {
                // Arrange
                var entityLeft = (Entity)null;
                var entityRight = new SampleEntity();

                // Act & Assert
                Assert.True(entityLeft != entityRight);
                Assert.True(entityRight != entityLeft);
            }

            [Fact]
            public void ReturnsFalseForBothOperandNull()
            {
                // Arrange
                var entityLeft = (Entity)null;
                var entityRight = (Entity)null;

                // Act & Assert
                Assert.False(entityLeft != entityRight);
                Assert.False(entityRight != entityLeft);
            }
        }

        public class GetHashCodeMethod
        {
            [Fact]
            public void ReturnsSameHashCodeForEqualIdentity()
            {
                // Arrange
                var id = Guid.NewGuid();

                var entity1 = new SampleEntity(id);
                var entity2 = new SampleEntity(id);

                // Act & Assert
                Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
            }

            [Fact]
            public void ReturnsDifferentHashCodeForNonEqualIdentity()
            {
                // Arrange
                var entity1 = new SampleEntity();
                var entity2 = new SampleEntity();

                // Act & Assert
                Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
            }
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
