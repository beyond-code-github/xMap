namespace xMap.Tests.Integration.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    public class When_performing_asymetrical_composite_mapping_with_complex_types : SqlClassMappings
    {
        protected static Exception ex;

        protected static List<ComplexDto> result;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new ComplexDto { Name = o.Name });
            xMap.Define((Cat o) => new ComplexDto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Dog o) => new ComplexDto { AnimalName = "Dog", Memberships = o.DogClubMemberships.AsEnumerable() })
                .DerivedFrom<Animal>();
        };

        private Because of = () => ex = Catch.Exception(() => result = multipleAnimals.Map<Animal, ComplexDto>().ToList());

        private It should_throw_an_exception = () => ex.ShouldBeOfType<NotSupportedException>();
    }
}
