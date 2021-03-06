﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Lemonad.ErrorHandling.Extensions {
    public static class MaybeExtensions {
        /// <summary>
        /// Creates a <see cref="Maybe{T}"/> who will have the value <paramref name="item"/>.
        /// </summary>
        /// <param name="item">
        /// The value of <see cref="Maybe{T}"/>.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the <see cref="Maybe{T}"/>.
        /// </typeparam>
        /// <returns>
        /// A <see cref="Maybe{T}"/> whose value will be <paramref name="item"/>.
        /// </returns>
        [Pure]
        public static Maybe<TSource> Some<TSource>(this TSource item) => new Maybe<TSource>(item, true);

        /// <summary>
        /// Creates a <see cref="Maybe{T}"/> who will have no value.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the <see cref="Maybe{T}"/>.
        /// </typeparam>
        /// <returns>
        /// A <see cref="Maybe{T}"/> who will have no value.
        /// </returns>
        [Pure]
        public static Maybe<TSource> None<TSource>() => Maybe<TSource>.None;

        /// <summary>
        /// Creates a <see cref="Maybe{T}"/> who will have no value.
        /// </summary>
        /// <param name="item">
        /// The value that will be considered to not have a value.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the <see cref="Maybe{T}"/>.
        /// </typeparam>
        /// <returns>
        /// A <see cref="Maybe{T}"/> who will have no value.
        /// </returns>
        [Pure]
        public static Maybe<TSource> None<TSource>(this TSource item) => new Maybe<TSource>(item, false);

        /// <summary>
        /// Works just like <see cref="Enumerable.FirstOrDefault{TSource}(System.Collections.Generic.IEnumerable{TSource})"/> but returns a <see cref="Maybe{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the <see cref="IEnumerable{T}"/>.
        /// </typeparam>
        /// <returns>
        /// Returns the first element of a sequence inside a <see cref="Maybe{T}"/>.
        /// </returns>
        public static Maybe<TSource> FirstMaybe<TSource>(this IEnumerable<TSource> source) {
            switch (source) {
                case IList<TSource> list when list.Count > 0:
                    return list[0];
                case IReadOnlyList<TSource> readOnlyList when readOnlyList.Count > 0:
                    return readOnlyList[0];
                default:
                    using (var e = source.GetEnumerator()) {
                        return e.MoveNext() ? e.Current : Maybe<TSource>.None;
                    }
            }
        }

        /// <summary>
        /// Works just like <see cref="Enumerable.FirstOrDefault{TSource}(System.Collections.Generic.IEnumerable{TSource})"/> but returns a <see cref="Maybe{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The <see cref="IEnumerable{T}"/>.
        /// </param>
        /// <param name="predicate">
        /// A function to test each element for a condition.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the <see cref="IEnumerable{T}"/>.
        /// </typeparam>
        /// <returns>
        /// Returns the first element of a sequence who matches the <paramref name="predicate"/> inside a <see cref="Maybe{T}"/>.
        /// </returns>
        public static Maybe<TSource> FirstMaybe<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate) {
            foreach (var element in source)
                if (predicate(element))
                    return element;

            return Maybe<TSource>.None;
        }

        /// <summary>
        ///  Converts an <see cref="IEnumerable{T}"/> of <see cref="Maybe{T}"/> into an <see cref="IEnumerable{T}"/> with the value of the <see cref="Maybe{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The <see cref="IEnumerable{T}"/> of <see cref="Maybe{T}"/>.
        /// </param>
        /// <typeparam name="TSource">
        /// The type inside the <see cref="Maybe{T}"/>.
        /// </typeparam>
        /// <returns>
        /// A sequence which can contain 0-n amount of values.
        /// </returns>
        public static IEnumerable<TSource> Values<TSource>(this IEnumerable<Maybe<TSource>> source) =>
            source.SelectMany(x => x.AsEnumerable);


        /// <summary>
        /// Executes <see cref="Maybe{T}.Match"/> for each element in the sequence.
        /// </summary>
        public static IEnumerable<TResult> Match<TSource, TResult>(this IEnumerable<Maybe<TSource>> source,
            Func<TSource, TResult> someSelector, Func<TResult> noneSelector) =>
            source.Select(x => x.Match(someSelector, noneSelector));

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> of <see cref="Maybe{T}"/> into an <see cref="IEnumerable{T}"/> of <typeparamref name="TResult"/> for each element which do not have a value.
        /// </summary>
        /// <param name="source">
        /// The <see cref="IEnumerable{T}"/> of <see cref="Maybe{T}"/>.
        /// </param>
        /// <param name="selector">
        /// A function to return a value for each <see cref="Maybe{T}"/>.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the <see cref="Maybe{T}"/>.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The return type of cuntion <paramref name="selector"/>.
        /// </typeparam>
        public static IEnumerable<TResult> NoValues<TSource, TResult>(this IEnumerable<Maybe<TSource>> source,
            Func<TResult> selector) => source.Where(x => x.HasValue == false).Select(_ => selector());

        /// <summary>
        /// Works like <see cref="None{TSource}(TSource)"/> but with an <paramref name="predicate"/> to test the element.
        /// </summary>
        /// <param name="source">
        /// The element to be passed into <see cref="Maybe{T}"/>.
        /// </param>
        /// <param name="predicate">
        /// A function to test the element.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the <paramref name="source"/>.
        /// </typeparam>
        [Pure]
        public static Maybe<TSource> None<TSource>(this TSource source, Func<TSource, bool> predicate) =>
            predicate != null
                ? Some(source).Filter(x => !predicate(x))
                : throw new ArgumentNullException(nameof(predicate));

        /// <summary>
        /// Works like <see cref="Some{TSource}(TSource)"/> but with an <paramref name="predicate"/> to test the element.
        /// </summary>
        /// <param name="source">
        /// The element to be passed into <see cref="Maybe{T}"/>.
        /// </param>
        /// <param name="predicate">
        /// A function to test the element.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the <paramref name="source"/>.
        /// </typeparam>
        [Pure]
        public static Maybe<TSource> Some<TSource>(this TSource source, Func<TSource, bool> predicate) =>
            predicate != null
                ? Some(source).Filter(predicate)
                : throw new ArgumentNullException(nameof(predicate));

        /// <summary>
        /// Converts an <see cref="Nullable{T}"/> to an <see cref="Maybe{T}"/>.
        /// </summary>
        /// <param name="source">
        /// The element from the <see cref="Nullable{T}"/> to be passed into <see cref="Maybe{T}"/>.
        /// </param>
        /// <typeparam name="TSource">
        /// The type of the <paramref name="source"/>.
        /// </typeparam>
        [Pure]
        public static Maybe<TSource> ToMaybe<TSource>(this TSource? source) where TSource : struct =>
            source.HasValue ? Some(source.Value) : None<TSource>();

        /// <summary>
        /// Converts an <see cref="Maybe{T}"/> to an <see cref="Result{T,TError}"/>.
        /// </summary>
        /// <param name="source">
        /// The element from the <see cref="Maybe{T}"/> to be passed into <see cref="Result{T,TError}"/>.
        /// </param>
        /// <param name="errorSelector">
        /// A function to be executed if there is no value inside the <see cref="Maybe{T}"/>.
        /// </param>
        /// <typeparam name="T">
        /// The type inside the <see cref="Maybe{T}"/>.
        /// </typeparam>
        /// <typeparam name="TError">
        /// The type representing an error for the <see cref="Result{T,TError}"/>.
        /// </typeparam>
        /// <returns></returns>
        [Pure]
        public static Result<T, TError>
            ToResult<T, TError>(this Maybe<T> source, Func<TError> errorSelector) {
            if (source.HasValue)
                return source.Value;
            return errorSelector != null
                ? errorSelector()
                : throw new ArgumentNullException(nameof(errorSelector));
        }
    }
}
