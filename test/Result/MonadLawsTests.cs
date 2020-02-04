using System;
using NUnit.Framework;
using Concludo.Core;

namespace Concludo.Tests.Result
{
    [TestFixture]
    public class MonadLawsTests
    {
        /// <summary>
        /// return a >>= f ≡ f a
        /// </summary>
        [Test]
        public void LeftIdentity()
        {
            const string value = "foo";
            var rule = (Func<string, Result<string, string>>)
                (x => Result<string, string>.Success(x));

            Assert.That(
                Concludo.Core.Result.Return(value).Bind(rule),
                Is.EqualTo(rule(value)));
        }

        /// <summary>
        /// m >>= return ≡ m
        /// </summary>
        [Test]
        public void RightIdentity()
        {
            var subject = Result<string, string>.Success("foo");

            Assert.That(
                subject.Bind(Concludo.Core.Result.Return),
                Is.EqualTo(subject));
        }

        /// <summary>
        /// (m >>= f) >>= g ≡ m >>= (\x -> f x >>= g)
        /// </summary>
        [Test]
        public void Associativity()
        {
            var subject = Result<string, string>.Success("foo");

            Func<string, Result<string, string>> Rule(string error)
                => x => Result<string, string>.Success(error);

            var rule1 = Rule("rule1");
            var rule2 = Rule("rule2");

            Assert.That(
                subject.Bind(rule1).Bind(rule2), Is.EqualTo(
                    subject.Bind(x => rule1(x).Bind(rule2))));
        }
    }
}