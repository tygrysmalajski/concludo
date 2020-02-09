using System;
using NUnit.Framework;
using Concludo.Core;
using static Concludo.Core.Result;
using static Concludo.Core.FuncExtensions;

namespace Concludo.Tests.Result
{
    [TestFixture]
    public class ResultTests
    {
        private const string Value = "foo";

        /// <summary>
        /// bimap id id ≡ id
        /// </summary>
        [Test]
        public void Bifunctor_Identity()
        {
            var result = Result<string, string>.Success(Value);

            Assert.That(
                result.Bimap(Id, Id).Value,
                Is.EqualTo(Id(Value)));
        }

        /// <summary>
        /// return a >>= f ≡ f a
        /// </summary>
        [Test]
        public void Monad_LeftIdentity()
        {
            var f = (Func<string, Result<string, string>>)
                (x => Result<string, string>.Success(x));

            Assert.That(
                Return<string, string>(Value).Bind(f),
                Is.EqualTo(f(Value)));
        }

        /// <summary>
        /// m >>= return ≡ m
        /// </summary>
        [Test]
        public void Monad_RightIdentity()
        {
            var m = Result<string, string>.Success(Value);

            Assert.That(
                m.Bind(Return<string, string>),
                Is.EqualTo(m));
        }

        /// <summary>
        /// (m >>= f) >>= g ≡ m >>= (\x -> f x >>= g)
        /// </summary>
        [Test]
        public void Monad_Associativity()
        {
            var m = Result<string, string>.Success(Value);

            Func<string, Result<string, string>> Func(string error)
                => x => Result<string, string>.Failure(error);

            var f = Func("f");
            var g = Func("g");

            Assert.That(
                m.Bind(f).Bind(g),
                Is.EqualTo(m.Bind(x => f(x).Bind(g))));
        }
    }
}