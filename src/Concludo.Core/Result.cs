using System;
using static Concludo.Core.FuncExtensions;

namespace Concludo.Core
{
    public class Result<TFailure, TSuccess> : IEquatable<Result<TFailure, TSuccess>>
    {
        protected Result(TSuccess value)
        {
            Value = value;
            Error = default;
        }

        protected Result(TFailure error)
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
        public static Result<TFailure, TSuccess> Return<TFailure, TSuccess>(
            TSuccess x)
            => Result<TFailure, TSuccess>.Success(x);

        public static Result<TFailure2, TSuccess2> Bimap<TFailure1, TFailure2, TSuccess1, TSuccess2>(
            this Result<TFailure1, TSuccess1> x,
            Func<TFailure1, TFailure2> fFailure,
            Func<TSuccess1, TSuccess2> fSuccess)
            => !x.IsSuccess
                ? Result<TFailure2, TSuccess2>.Failure(fFailure(x.Error))
                : Result<TFailure2, TSuccess2>.Success(fSuccess(x.Value));

        public static Result<TFailure, TSuccess2> MapSuccess<TFailure, TSuccess1, TSuccess2>(
            this Result<TFailure, TSuccess1> x,
            Func<TSuccess1, TSuccess2> f)
            => x.Bimap(Id, f);

        public static Result<TFailure2, TSuccess> MapFailure<TFailure1, TFailure2, TSuccess>(
            this Result<TFailure1, TSuccess> x,
            Func<TFailure1, TFailure2> f)
            => x.Bimap(f, Id);

        public static Result<TFailure, TSuccess2> Bind<TFailure, TSuccess1, TSuccess2>(
            this Result<TFailure, TSuccess1> mx,
            Func<TSuccess1, Result<TFailure, TSuccess2>> mf)
            => !mx.IsSuccess 
                ? Result<TFailure, TSuccess2>.Failure(mx.Error)
                : mf(mx.Value);
    }
}