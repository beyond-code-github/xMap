namespace xMap.Tests.Unit
{
    using System;

    using Machine.Specifications;

    using global::xMap.Exceptions;

    public abstract class SetupExceptions
    {
        protected static Exception ex;
    }

    public class When_defining_a_condition_on_both_the_base_and_derived_types_ : SetupExceptions
    {
        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name }).For<User>(o => o.IsAdmin);
        };

        private Because of = () => ex = Catch.Exception(() =>
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>().For<User>(o => !o.IsAdmin));

        private It should_throw_an_exception = () => ex.ShouldBeOfType<ConditionAlreadySpecifiedException>();
    }

}
