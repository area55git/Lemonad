﻿using System;
using System.Diagnostics.Contracts;

namespace Lemonad.ErrorHandling {
    public static class Maybe {
        public static void Match<TSource>(this Maybe<TSource> source, Action<TSource> someAction,
            Action noneAction) {
            if (someAction == null)
                throw new ArgumentNullException(nameof(someAction));
            if (noneAction == null)
                throw new ArgumentNullException(nameof(noneAction));

            if (source.HasValue) someAction(source.Value);
            else noneAction();
        }

        [Pure]
        public static Maybe<TSource> None<TSource>() => Maybe<TSource>.Identity;

        [Pure]
        public static Maybe<TSource> None<TSource>(this TSource item) => new Maybe<TSource>(item, false);

        [Pure]
        public static Maybe<TSource> NoneWhen<TSource>(this TSource item, Func<TSource, bool> predicate) =>
            predicate != null
                ? Some(item).SomeWhen(x => !predicate(x))
                : throw new ArgumentNullException(nameof(predicate));

        [Pure]
        public static Maybe<TSource> NoneWhen<TSource>(this Maybe<TSource> source,
            Func<TSource, bool> predicate) => source.SomeWhen(x => !predicate(x));

        [Pure]
        public static Maybe<TSource> Some<TSource>(this TSource item) => new Maybe<TSource>(item, true);

        [Pure]
        public static Maybe<TSource> NoneWhenNull<TSource>(this TSource item) =>
            item.NoneWhen(EquailtyFunctions.IsNull);

        [Pure]
        public static Maybe<TSource> NoneWhenNull<TSource>(this Maybe<TSource> item) =>
            item.NoneWhen<TSource>(EquailtyFunctions.IsNull);

        [Pure]
        public static Maybe<TSource> SomeWhen<TSource>(this TSource item, Func<TSource, bool> predicate) =>
            predicate != null
                ? Some(item).SomeWhen(predicate)
                : throw new ArgumentNullException(nameof(predicate));

        [Pure]
        public static Maybe<TSource> ConvertToMaybe<TSource>(this TSource? item) where TSource : struct =>
            item.HasValue ? Some(item.Value) : None<TSource>();

        [Pure]
        public static Maybe<TSource> SomeWhen<TSource>(this Maybe<TSource> source,
            Func<TSource, bool> predicate) => predicate != null
            ? source.HasValue
                ? predicate(source.Value)
                    ? Some(source.Value)
                    : None<TSource>()
                : None<TSource>()
            : throw new ArgumentNullException(nameof(predicate));

        [Pure]
        public static Maybe<TResult>
            Map<TSource, TResult>(this Maybe<TSource> source, Func<TSource, TResult> selector) =>
            selector != null
                ? source.HasValue ? Some(selector(source.Value)) : None<TResult>()
                : throw new ArgumentNullException(nameof(selector));

        [Pure]
        public static Maybe<TResult> FlatMap<TSource, TResult>(this Maybe<TSource> source,
            Func<TSource, Maybe<TResult>> selector) {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (!source.HasValue) return None<TResult>();
            var result = selector(source.Value);
            return result.HasValue ? Some(result.Value) : None<TResult>();
        }

        [Pure]
        public static TResult Match<TSource, TResult>(this Maybe<TSource> source, Func<TSource, TResult> someSelector,
            Func<TResult> noneSelector) => someSelector == null
            ? throw new ArgumentNullException(nameof(someSelector))
            : (noneSelector == null
                ? throw new ArgumentNullException(nameof(noneSelector))
                : (source.HasValue ? someSelector(source.Value) : noneSelector()));

        [Pure]
        public static Maybe<TResult> FlatMap<TSource, TResult>(this Maybe<TSource> source,
            Func<TSource, TResult?> selector) where TResult : struct =>
            source.HasValue ? selector(source.Value).ConvertToMaybe() : None<TResult>();

        [Pure]
        public static Maybe<TResult> FlatMap<TSource, TResult>(this Maybe<TSource> source,
            TResult? nullable) where TResult : struct => source.FlatMap(_ => nullable);

        [Pure]
        public static Maybe<TResult> FlatMap<TSource, TSelector, TResult>(this Maybe<TSource> source,
            TSelector? nullable, Func<TSource, TSelector, TResult> resultSelector) where TSelector : struct =>
            source.FlatMap(_ => nullable, resultSelector);

        [Pure]
        public static Maybe<TResult> FlatMap<TSource, TResult>(this Maybe<TSource> source,
            Maybe<TResult> maybe) => source.FlatMap(_ => maybe);

        [Pure]
        public static Maybe<TResult> FlatMap<TSource, TSelector, TResult>(this Maybe<TSource> source,
            Maybe<TSelector> maybe, Func<TSource, TSelector, TResult> resultSelector) =>
            source.FlatMap(_ => maybe, resultSelector);

        [Pure]
        public static Maybe<TResult> FlatMap<TSource, TSelector, TResult>(
            this Maybe<TSource> source,
            Func<TSource, Maybe<TSelector>> selector,
            Func<TSource, TSelector, TResult> resultSelector) =>
            resultSelector != null
                ? selector != null
                    ? source.FlatMap(src => selector(src).Map(elem => resultSelector(src, elem)))
                    : throw new ArgumentNullException(nameof(selector))
                : throw new ArgumentNullException(nameof(resultSelector));

        [Pure]
        public static Maybe<TResult> FlatMap<TSource, TSelector, TResult>(
            this Maybe<TSource> source,
            Func<TSource, TSelector?> selector,
            Func<TSource, TSelector, TResult> resultSelector) where TSelector : struct => source.FlatMap(
            src => selector(src).ConvertToMaybe().Map(elem => resultSelector(src, elem)));
    }
}