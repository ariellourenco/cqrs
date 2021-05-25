using Xunit;

namespace CQRSJourney.Registration
{
    public sealed class ValueObjectTests
    {
        [Fact]
        public void Equals_ComparesAllPropertiesAndFieldsNull_ReturnsTrue()
        {
            // Arrange
            var value1 = new TestValue();
            var value2 = new TestValue();

            // Act & Assert
            Assert.True(value1.Equals(value2));
            Assert.True(value2.Equals(value1));
            Assert.True(value1 == value2);
            Assert.True(value2 == value1);
        }

        [Fact]
        public void Equals_ComparesAllPropertiesAndFields_ReturnsTrue()
        {
            // Arrange
            var value1 = new TestValue { Property1 = "test", Property2 = 10, Field = 3 };
            var value2 = new TestValue { Property1 = "test", Property2 = 10, Field = 3 };

            // Act & Assert
            Assert.True(value1.Equals(value2));
            Assert.True(value2.Equals(value1));
            Assert.True(value1 == value2);
            Assert.True(value2 == value1);
        }

        [Fact]
        public void  Equals_ComparesAllPropertiesAndFieldsForDifferentProperty_ReturnsFalse()
        {
            // Arrange
            var value1 = new TestValue { Property1 = "test", Property2 = 10, Field = 3 };
            var value2 = new TestValue { Property1 = "test2", Property2 = 10, Field = 3 };

            // Act & Assert
            Assert.False(value1.Equals(value2));
            Assert.False(value2.Equals(value1));
            Assert.False(value1 == value2);
            Assert.False(value2 == value1);
        }

        [Fact]
        public void  Equals_ComparesAllPropertiesAndFieldsForDifferentFields_ReturnsFalse()
        {
            // Arrange
            var value1 = new TestValue { Property1 = "test", Property2 = 10, Field = 3 };
            var value2 = new TestValue { Property1 = "test", Property2 = 10, Field = 5 };

            // Act & Assert
            Assert.False(value1.Equals(value2));
            Assert.False(value2.Equals(value1));
            Assert.False(value1 == value2);
            Assert.False(value2 == value1);
        }

        [Fact]
        public void Equals_ComparisionIgnoresPrivatePropertiesAndFields()
        {
            // Arrange
            var value1 = new TestValue(5);
            var value2 = new TestValue(8);

            // Act & Assert
            Assert.True(value1.Equals(value2));
            Assert.True(value2.Equals(value1));
            Assert.True(value1 == value2);
            Assert.True(value2 == value1);
        }

        [Fact]
        public void Equals_ComparesSameReference_ReturnsTrue()
        {
            // Arrange
            var value1 = new TestValue { Property1 = "test", Property2 = 10, Field = 3 };
            var value2 = value1;

            // Act & Assert
            Assert.True(value1.Equals(value2));
            Assert.True(value2.Equals(value1));
            Assert.True(value1 == value2);
            Assert.True(value2 == value1);
        }

        [Fact]
        public void Equals_ComparesDifferentRuntimeType_ReturnsFalse()
        {
            // Arrange
            var value1 = new TestValue { Property1 = "test", Property2 = 10, Field = 3 };
            var value2 = new {};

            // Act & Assert
            Assert.False(value1.Equals(value2));
            Assert.False(value2.Equals(value1));
        }

        [Fact]
        public void Equals_ComparesNullReference_ReturnsFalse()
        {
            // Arrange
            var value1 = new TestValue { Property1 = "test", Property2 = 10, Field = 3 };
            var value2 = (ValueObject)null;

            // Act & Assert
            Assert.False(value1.Equals(value2));
            Assert.False(value1 == value2);
            Assert.False(value2 == value1);
        }

        [Fact]
        public void Equals_ComparesWhenBothOperandsAreNull_ReturnsTrue()
        {
            // Arrange
            var value1 = (ValueObject)null;
            var value2 = (ValueObject)null;

            // Act & Assert
            Assert.True(value1 == value2);
            Assert.True(value2 == value1);
        }

        [Fact]
        public void DifferentOperator_ComparesPropertiesAndFields_ReturnsFalse()
        {
            // Arrange
            var value1 = new TestValue { Property1 = "test", Property2 = 10, Field = 3 };
            var value2 = new TestValue { Property1 = "test", Property2 = 10, Field = 3 };

            // Act & Assert
            Assert.False(value1 != value2);
            Assert.False(value2 != value1);
        }

        [Fact]
        public void DifferentOperator_ComparesDifferentPropertiesAndFields_ReturnsTrue()
        {
            // Arrange
            var value1 = new TestValue { Property1 = "test", Property2 = 10, Field = 3 };
            var value2 = new TestValue { Property1 = "test2", Property2 = 10, Field = 3 };

            // Act & Assert
            Assert.True(value1 != value2);
            Assert.True(value2 != value1);
        }

        [Fact]
        public void GetHashCode_AlwaysEqualForEqualObjects()
        {
            // Arrange
            var value1 = new TestValue { Property1 = "string", Property2 = 4 };
            var value2 = new TestValue { Property1 = "string", Property2 = 4 };

            // Act & Assert
             Assert.Equal(value1.GetHashCode(), value2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_NotEqualForDistinctObjects()
        {
            // Arrange
            var value1 = new TestValue { Property1 = "string", Property2 = 4 };
            var value2 = new TestValue { Property1 = "String", Property2 = 5 };

            // Act & Assert
             Assert.NotEqual(value1.GetHashCode(), value2.GetHashCode());
        }

        private class TestValue : ValueObject
        {
            private int privateField;

            protected int protectedField;

            public int Field;

            private int PrivateProperty { get; set; }

            protected int ProtectedProperty { get; set; }

            public string Property1 { get; set; }

            public int Property2 { get; set; }

            public TestValue()
            {
            }

            public TestValue(int nonPublicValue)
            {
                ProtectedProperty = nonPublicValue;
                PrivateProperty = nonPublicValue;
                privateField = nonPublicValue;
                protectedField = nonPublicValue;
            }
        }
    }
}
