using System;
using System.Collections.Generic;
using System.Linq;

namespace Concludo.Core
{
    public static class ResultExtensions
    {
        public static Result<IEnumerable<TFailure>, TSuccess> Sequence<TFailure, TSuccess>(
            this IEnumerable<Result<TFailure, TSuccess>> results)
        {
            var errors = results
                .Where(r => !r.IsSuccess)
                .Select(r => r.Error);

            return errors.Any()
                ? Result<IEnumerable<TFailure>, TSuccess>.Failure(errors)
                : Result<IEnumerable<TFailure>, TSuccess>.Success(
                    results.First(r => r.IsSuccess).Value);
        }

        public static string Print<TFailure, TSuccess>(
            this Result<TFailure, TSuccess> result)
            => result.Print(FormatError);

        public static string Print<TSuccess>(
            this Result<IEnumerable<string>, TSuccess> result)
            => result.Print(errors => string
                .Join("; ", errors
                    .Select(FormatError)));

        private static string Print<TFailure, TSuccess>(
            this Result<TFailure, TSuccess> result,
            Func<TFailure, string> toString)
        {
            toString = toString ?? (x => x.ToString());
            return result.IsSuccess
                ? PrintResult(nameof(result.Success), result.Value.ToString())
                : PrintResult(nameof(result.Failure), toString(result.Error));

            string PrintResult(string @case, string value)
                => $"{nameof(Result)}<{typeof(TSuccess).Name}> ({@case} [{value}])";
        }
        private static string FormatError<TFailure>(
            TFailure error)
            => $"\"{error}\"";
    }
}