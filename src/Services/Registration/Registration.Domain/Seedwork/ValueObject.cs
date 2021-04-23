using System;
using System.Linq;
using System.Reflection;

namespace CQRSJourney.Registration
{
    /// <summary>
    /// An abstract base class that represents objects that do not require an identity and
    /// identity tracking in the system. By default, two objects will be considered equal if they are the
    /// same type and all of their public properties and fields are equal.
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        /// <summary>
        /// Determines whether the specified <see cref="ValueObject"/> is equal to the current one.
        /// </summary>
        /// <param name ="other">The <see cref="ValueObject"/> to compare with the current one.</param>
        /// <returns><see langword="true"/> if the specified <see cref="ValueObject"/> is equal to the current one; otherwise, <see langword="false"/>.</returns>
        public bool Equals(ValueObject other) =>
            Equals(other as object);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            // Check for null and compare run-time types.
            if (obj == null || !this.GetType().Equals(obj.GetType()))
                return false;

            // Compare all public fields and properties.
            var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return properties.All(property => object.Equals(property.GetValue(this, null), property.GetValue(obj, null))) &&
                fields.All(field => object.Equals(field.GetValue(this), field.GetValue(obj)));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = 17;

            // Gets all public fields and properties.
            var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if ((fields != null && fields.Any()) && (properties != null && properties.Any()))
            {
                unchecked
                {
                    foreach (var field in fields)
                    {
                        var name = field.Name;
                        var value = field.GetValue(this);

                        hash += name.GetHashCode() ^ value.GetHashCode();
                    }

                    foreach (var property in properties)
                    {
                        var name = property.Name;
                        var value = property.GetValue(this, null);

                        hash += name.GetHashCode() ^ value.GetHashCode();
                    }
                }
            }

            return hash;
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (object.Equals(left, null))
                return object.Equals(right, null) ? true : false;

            return left.Equals(right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }
    }
}
