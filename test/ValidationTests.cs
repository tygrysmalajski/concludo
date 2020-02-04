using System.Collections.Generic;
using NUnit.Framework;
using Concludo.Core;

namespace Concludo.Tests
{
    [TestFixture]
    public class ValidationTests
    {
        [Test]
        public void ComplexValidation()
        {
            Rule<T> fieldRequired<T>() => Rule.Required<T>(
                x => x != null,
                "Null");

            var stringRequired = Rule.Required<string>(
                value => !string.IsNullOrEmpty(value),
                nameof(string.IsNullOrEmpty));

            var nameRequired = Rule.Compose<Person, string>(
                person => person.Name,
                stringRequired,
                nameof(Person.Name));

            var streetRequired = Rule.Compose<Address, string>(
                address => address.Street,
                stringRequired,
                nameof(Address.Street));

            var addressRequired = Rule.Compose<Person, Address>(
                person => person.Address,
                fieldRequired<Address>(),
                nameof(Person.Address));

            var addressValidation = addressRequired
                .AndThen(Rule.Compose<Person, Address>(
                    person => person.Address,
                    streetRequired,
                    nameof(Person.Address)));

            Result<IEnumerable<string>, Person> personValidation(params Person[] persons)
                => new[]
                {
                    nameRequired,
                    addressValidation
                }
                .Apply(persons)
                .Sequence();

            var person = new Person() { Name = "", Address = new Address() };
            var result = personValidation(person);

            TestContext.Out.WriteLine(result.Print());
            Assert.That(result.IsSuccess, Is.False);
        }
    }

    class Person
    {
        public string Name { get; set; }
        public Address Address { get; set; }
    }

    class Address
    {
        public string Street { get; set; }
    }
}