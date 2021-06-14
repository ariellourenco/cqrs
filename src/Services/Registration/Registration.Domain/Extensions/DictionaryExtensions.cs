using System;
using System.Collections.Generic;

namespace CQRSJourney.Registration.Extensions
{
    /// <summary>
    /// Extension methods to <see cref="IDictionary"/>.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds a key/value pair to the <see cref="Dictionary{TKey,TValue}"/> if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary,
        /// or the new value for the key if the key was not in the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is a null reference.</exception>
        /// <exception cref="OverflowException">The dictionary contains too many elements.</exception>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
            where TValue : new()
            {
                if (!source.TryGetValue(key, out var value))
                {
                    value = new TValue();
                    source[key] = value;
                }

                return value;
            }
    }
}
