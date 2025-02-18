﻿// <copyright file="TestOnlyExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides extension methods to ease the writing of unit tests.
    /// </summary>
    public static class TestOnlyExtensions
    {
        /// <summary>
        /// Converts the given items of type <see cref="List{T}"/> to a dictionary of
        /// type <see cref="Dictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="items">The items to convert.</param>
        /// <typeparam name="T">The type of items in the <see cref="List{T}"/>.</typeparam>
        /// <returns>The list of items as the type <see cref="Dictionary{TKey,TValue}"/>.</returns>
        public static Dictionary<uint, T> ToDictionary<T>(this List<T> items)
        {
            var result = new Dictionary<uint, T>();

            for (var i = 0u; i < items.Count; i++)
            {
                result.Add(i, items[(int)i]);
            }

            return result;
        }

        /// <summary>
        /// Converts the items of type <see cref="IEnumerable{T}"/> to type <see cref="ReadOnlyCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the <see cref="IEnumerable{T}"/> list.</typeparam>
        /// <param name="items">The items to convert.</param>
        /// <returns>The items as a read only collection.</returns>
        public static ReadOnlyDictionary<uint, T> ToReadOnlyDictionary<T>(this List<T> items)
            => new (items.ToDictionary());

        /// <summary>
        /// Converts the given list of <paramref name="items"/> to a read only dictionary where
        /// the key is the <paramref name="items"/> array item index.
        /// </summary>
        /// <param name="items">The list of items to convert.</param>
        /// <typeparam name="T">The type of values in the lists.</typeparam>
        /// <returns>A read only dictionary of the given <paramref name="items"/>.</returns>
        public static ReadOnlyDictionary<uint, T> ToReadOnlyDictionary<T>(this T[] items)
        {
            var result = new Dictionary<uint, T>();

            for (var i = 0u; i < items.Length; i++)
            {
                result.Add(i, items[i]);
            }

            return new ReadOnlyDictionary<uint, T>(result);
        }
    }
}
