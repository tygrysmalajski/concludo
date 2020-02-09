using System;
using System.Collections.Generic;
using System.Linq;

namespace Concludo.Core
{
    public delegate Result<string, TSubject> Rule<TSubject>(TSubject subject);

    public static class Rule
    {
        public static Rule<TSubject> Required<TSubject>(
            Func<TSubject, bool> predicate,
            string error)
            => subject =>
                predicate(subject)
                    ? Result<string, TSubject>.Success(subject)
                    : Result<string, TSubject>.Failure(error);

        public static Rule<TSubject> Compose<TSubject, TSubjectProp>(
            Func<TSubject, TSubjectProp> propertyOf,
            Rule<TSubjectProp> rule,
            string outerError)
            => subject 
                => rule(propertyOf(subject))
                    .Bimap(innerError 
                        => new StringFailure(outerError)
                            .Combine(new StringFailure(innerError))
                            .Value, _
                        => subject);

        public static IEnumerable<Result<string, TSubject>> Apply<TSubject>(
            this Rule<TSubject>[] rules,
            TSubject subject)
            => rules.Select(rule
                => rule(subject));

        /// <summary>
        /// Kleisli composition: (a -> m b) -> (b -> m c) -> (a -> m c)
        /// </summary>
        public static Rule<TSubject> AndThen<TSubject>(
            this Rule<TSubject> rule1,
            Rule<TSubject> rule2)
            => subject => rule1(subject).Bind(rule2.Invoke);
    }
}