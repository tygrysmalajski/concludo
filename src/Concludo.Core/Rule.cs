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
            => value =>
                predicate(value)
                    ? Result<string, TSubject>.Success(value)
                    : Result<string, TSubject>.Failure(error);

        public static Rule<TSubject> Compose<TSubject, TSubjectProp>(
            Func<TSubject, TSubjectProp> propertyOf,
            Rule<TSubjectProp> rule,
            string error)
            => subject =>
            {
                var result = rule(propertyOf(subject));
                return result.IsSuccess
                    ? Result<string, TSubject>.Success(subject)
                    : Result<string, TSubject>.Failure(
                        new StringFailure(error)
                            .Combine(new StringFailure(result.Error))
                            .Value);
            };

        public static IEnumerable<Result<string, TSubject>> Apply<TSubject>(
            this IEnumerable<Rule<TSubject>> rules,
            IEnumerable<TSubject> subjects)
            => rules.SelectMany(rule
                => subjects.Select(subject
                    => rule(subject)));

        /// <summary>
        /// Kleisli composition: (a -> m b) -> (b -> m c) -> (a -> m c)
        /// </summary>
        public static Rule<TSubject> AndThen<TSubject>(
            this Rule<TSubject> rule1,
            Rule<TSubject> rule2)
            => subject => rule1(subject).Bind(rule2.Invoke);
    }
}