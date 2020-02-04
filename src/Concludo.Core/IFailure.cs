namespace Concludo.Core
{
    public interface IFailure<T>
    {
        T Value { get; }
        IFailure<T> Combine(IFailure<T> other);
    }

    public struct StringFailure : IFailure<string>
    {
        public StringFailure(string value)
            => Value = value;

        public static IFailure<string> New(string value)
            => new StringFailure(value);

        public string Value { get; }

        public IFailure<string> Combine(IFailure<string> other)
            => new StringFailure($"{Value}: {other.Value}");

        public override string ToString()
            => Value;
    }
}