using System;
using System.Collections.Generic;
using System.Linq;
using static Concludo.Core.EnumerableExtensions;

namespace Concludo.Core
{
    public class SummaryResult<TFailure, TSuccess> :
        Result<IEnumerable<TFailure>, TSuccess>,
        IEquatable<SummaryResult<TFailure, TSuccess>>
    {
        protected SummaryResult(TSuccess value)
            : base(value) { }

        protected SummaryResult(IEnumerable<TFailure> error)
            : base(error) { }

        public new static SummaryResult<TFailure, TSuccess> Success(
            TSuccess value)
            => new SummaryResult<TFailure, TSuccess>(value);

        public new static SummaryResult<TFailure, TSuccess> Failure(
            IEnumerable<TFailure> error)
            => new SummaryResult<TFailure, TSuccess>(error);

        public override string ToString()
            => this.Print();

        public bool Equals(SummaryResult<TFailure, TSuccess> other)
            => IsSuccess
                ? Result<TFailure, TSuccess>.Equals(this, other)
                : Error.SequenceEqual(other.Error);

        public override int GetHashCode()
            => HashCode.Combine(Value, Error
                .Aggregate(42, HashCode.Combine));
    }

    public static class SummaryResult
    {
        public static SummaryResult<TFailure, TSuccess> ToSummaryResult<TFailure, TSuccess>(
            this Result<IEnumerable<TFailure>, TSuccess> result)
            => !result.IsSuccess
                ? SummaryResult<TFailure, TSuccess>.Failure(result.Error)
                : SummaryResult<TFailure, TSuccess>.Success(result.Value);

        public static SummaryResult<TFailure, TSuccess> Apply<TFailure, TSuccess>(
            this SummaryResult<TFailure, TSuccess> result1,
            SummaryResult<TFailure, TSuccess> result2)
        {
            if (!result1.IsSuccess && !result2.IsSuccess)
                return SummaryResult<TFailure, TSuccess>
                    .Failure(result1.Error.Concat(result2.Error));
            
            if (!result1.IsSuccess)
                return SummaryResult<TFailure, TSuccess>
                    .Failure(result1.Error);

            if (!result2.IsSuccess)
                return SummaryResult<TFailure, TSuccess>
                    .Failure(result2.Error);

            return SummaryResult<TFailure, TSuccess>
                .Success(result1.Value);
        }

        public static SummaryResult<TFailure, TSuccess> Reduce<TFailure, TSuccess>(
            this IEnumerable<Result<TFailure, TSuccess>> results)
            => results
                .Select(r => r
                    .MapFailure(Singleton)
                    .ToSummaryResult())
                .Aggregate(Apply);
    }
}