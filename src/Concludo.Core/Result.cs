using System;

namespace Concludo.Core
{
    public struct Result<TFailure, TSuccess> : IEquatable<Result<TFailure, TSuccess>>
    {
        private Result(TSuccess value)
        {
            Value = value;
            Error = default;
        }
        private Result(TFailure error)
        {
            Value = default;
            Error = error;
        }

        public static Result<TFailure, TSuccess> Success(
            TSuccess value)
            => new Result<TFailure, TSuccess>(value);

        public static Result<TFailure, TSuccess> Failure(
            TFailure error)
            => new Result<TFailure, TSuccess>(error);

        public TSuccess Value { get; }

        public TFailure Error { get; }

        public bool IsSuccess
            => !Value?.Equals(default(TSuccess)) ?? false;

        public override string ToString()
            => this.Print();

        public bool Equals(Result<TFailure, TSuccess> other)
            => Value?.Equals(other.Value) ?? true
                || Error.Equals(other.Error);

        public override int GetHashCode()
            => HashCode.Combine(Value, Error);
    }

    public static class Result
    {
        public static Result<string, TSubject> Return<TSubject>(
            TSubject x)
            => Result<string, TSubject>.Success(x);

        public static Result<TFailure, TSuccess2> Map<TFailure, TSuccess1, TSuccess2>(
           this Func<TSuccess1, TSuccess2> f,
           Result<TFailure, TSuccess1> x)
           => !x.IsSuccess
               ? Result<TFailure, TSuccess2>.Failure(x.Error)
               : Result<TFailure, TSuccess2>.Success(f(x.Value));

        public static Result<TFailure, TSuccess2> Bind<TFailure, TSuccess1, TSuccess2>(
            this Result<TFailure, TSuccess1> x,
            Func<TSuccess1, Result<TFailure, TSuccess2>> f)
            => !x.IsSuccess 
                ? Result<TFailure, TSuccess2>.Failure(x.Error)
                : f(x.Value);
    }
}