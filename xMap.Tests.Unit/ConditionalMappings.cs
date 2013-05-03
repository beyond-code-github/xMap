namespace xMap.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Machine.Specifications;

    using global::xMap.Exceptions;

    public abstract class ConditionalMappings
    {
        protected static Exception ex;

        protected static List<Dto> result;
    }

    #region Single mappings

    public class When_a_conditional_mapping_criteria_is_matched : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name }).For<User>(o => o.IsAdmin);

            user = new User { IsAdmin = true };
            cat = new Cat { Name = "Aeris", Owner = "Pete & Kathryn" };
        };

        private Because of = () => singleresult = cat.MapFor(user).To<Dto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");
    }

    public class When_a_conditional_mapping_criteria_is_matched_over_a_default : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name, Owner = o.Owner }).For<User>(o => o.IsAdmin);

            user = new User { IsAdmin = true };
            cat = new Cat { Name = "Aeris", Owner = "Pete & Kathryn" };
        };

        private Because of = () => singleresult = cat.MapFor(user).To<Dto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");

        private It should_map_the_owner_property_for_the_first_record =
           () => singleresult.Owner.ShouldEqual("Pete & Kathryn");
    }

    public class When_a_conditional_mapping_is_told_to_include_defaults : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name });
            xMap.Define((Cat o) => new Dto { Owner = o.Owner }).For<User>(o => o.IsAdmin).WithDefaults();

            user = new User { IsAdmin = true };
            cat = new Cat { Name = "Aeris", Owner = "Pete & Kathryn" };
        };

        private Because of = () => singleresult = cat.MapFor(user).To<Dto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");

        private It should_map_the_owner_property_for_the_first_record =
           () => singleresult.Owner.ShouldEqual("Pete & Kathryn");
    }

    public class When_a_conditional_mapping_is_not_told_to_include_defaults : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name });
            xMap.Define((Cat o) => new Dto { Owner = o.Owner }).For<User>(o => o.IsAdmin);

            user = new User { IsAdmin = true };
            cat = new Cat { Name = "Aeris", Owner = "Pete & Kathryn" };
        };

        private Because of = () => singleresult = cat.MapFor(user).To<Dto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => string.IsNullOrEmpty(singleresult.AnimalName).ShouldBeTrue();

        private It should_map_the_name_property_for_the_first_record =
            () => string.IsNullOrEmpty(singleresult.Name).ShouldBeTrue();

        private It should_map_the_owner_property_for_the_first_record =
           () => singleresult.Owner.ShouldEqual("Pete & Kathryn");
    }

    public class When_a_conditional_mapping_criteria_is_not_matched_and_there_is_no_alternative : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name }).For<User>(o => o.IsAdmin);

            user = new User { IsAdmin = false };
            cat = new Cat { Name = "Aeris", Owner = "Pete & Kathryn" };
        };

        private Because of = () => ex = Catch.Exception(() => singleresult = cat.MapFor(user).To<Dto>());

        private It should_not_map_the_record = () => ex.ShouldBeOfType<NoMappingsFoundException>();
    }

    public class When_a_conditional_mapping_criteria_is_not_matched_and_there_is_a_default_alternative : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name, Owner = o.Owner }).For<User>(o => o.IsAdmin);

            user = new User { IsAdmin = false };
            cat = new Cat { Name = "Aeris", Owner = "Pete & Kathryn" };
        };

        private Because of = () => singleresult = cat.MapFor(user).To<Dto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");

        private It should_map_using_the_default_that_does_not_include_owner = () => string.IsNullOrEmpty(singleresult.Owner).ShouldBeTrue();
    }

    #endregion

    #region Multiple mappings

    public class When_a_conditional_mapping_criteria_is_matched_for_multiple_records : ConditionalMappings
    {
        protected static User user;

        protected static IQueryable<Animal> animals;

        private Establish context = () =>
            {
                user = new User { IsAdmin = true };

                xMap.Reset();
                xMap.Define((Animal o) => new Dto { AnimalName = "Unknown", Name = o.Name, Owner = o.Owner }).For<User>(o => o.IsAdmin);

                animals = new List<Animal> { new Cat { Name = "Aeris", Owner = "Peter & Kathryn" }, new Cat { Name = "MacBeth", Owner = "Peter & Kathryn" } }.AsQueryable();
            };

        private Because of = () => result = animals.MapFor(user).To<Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Unknown");

        private It should_map_the_owner_property_for_the_first_record =
           () => result.ElementAt(0).Owner.ShouldEqual("Peter & Kathryn");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");

        private It should_map_the_owner_property_for_the_second_record =
            () => result.ElementAt(1).Owner.ShouldEqual("Peter & Kathryn");
    }

    public class When_a_conditional_mapping_criteria_is_matched_over_a_default_for_multiple_records : ConditionalMappings
    {
        protected static User user;

        protected static IQueryable<Cat> cats;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Unknown", Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Unknown", Name = o.Name, Owner = o.Owner }).For<User>(o => o.IsAdmin);

            cats = new List<Cat> { new Cat { Name = "Aeris", Owner = "Peter & Kathryn" }, new Cat { Name = "MacBeth", Owner = "Peter & Kathryn" } }.AsQueryable();
        };

        private Because of = () => result = cats.MapFor(user).To<Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Unknown");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_map_the_owner_property_for_the_first_record =
           () => result.ElementAt(0).Owner.ShouldEqual("Peter & Kathryn");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");

        private It should_map_the_owner_property_for_the_second_record =
            () => result.ElementAt(1).Owner.ShouldEqual("Peter & Kathryn");
    }

    public class When_a_conditional_mapping_is_told_to_include_defaults_for_multiple_records : ConditionalMappings
    {
        protected static User user;

        protected static IQueryable<Cat> cats;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Unknown", Name = o.Name });
            xMap.Define((Cat o) => new Dto { Owner = o.Owner }).For<User>(o => o.IsAdmin).WithDefaults();

            cats = new List<Cat> { new Cat { Name = "Aeris", Owner = "Peter & Kathryn" }, new Cat { Name = "MacBeth", Owner = "Peter & Kathryn" } }.AsQueryable();
        };

        private Because of = () => result = cats.MapFor(user).To<Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Unknown");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_map_the_owner_property_for_the_first_record =
           () => result.ElementAt(0).Owner.ShouldEqual("Peter & Kathryn");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");

        private It should_map_the_owner_property_for_the_second_record =
            () => result.ElementAt(1).Owner.ShouldEqual("Peter & Kathryn");
    }

    public class When_a_conditional_mapping_is_told_not_to_include_defaults_for_multiple_records : ConditionalMappings
    {
        protected static User user;

        protected static IQueryable<Cat> cats;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Unknown", Name = o.Name });
            xMap.Define((Cat o) => new Dto { Owner = o.Owner }).For<User>(o => o.IsAdmin);

            cats = new List<Cat> { new Cat { Name = "Aeris", Owner = "Peter & Kathryn" }, new Cat { Name = "MacBeth", Owner = "Peter & Kathryn" } }.AsQueryable();
        };

        private Because of = () => result = cats.MapFor(user).To<Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_not_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => string.IsNullOrEmpty(o.AnimalName));

        private It should_not_map_the_name_property_for_the_first_record =
            () => string.IsNullOrEmpty(result.ElementAt(0).Name).ShouldBeTrue();

        private It should_map_the_owner_property_for_the_first_record =
           () => result.ElementAt(0).Owner.ShouldEqual("Peter & Kathryn");

        private It should_not_map_the_name_property_for_the_second_record =
            () => string.IsNullOrEmpty(result.ElementAt(1).Name).ShouldBeTrue();

        private It should_map_the_owner_property_for_the_second_record =
            () => result.ElementAt(1).Owner.ShouldEqual("Peter & Kathryn");
    }

    public class When_a_conditional_mapping_criteria_is_not_matched_and_there_is_no_alternatice_for_multiple_records : ConditionalMappings
    {
        protected static User user;

        protected static IQueryable<Animal> animals;

        private Establish context = () =>
        {
            user = new User { IsAdmin = false };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { AnimalName = "Unknown", Name = o.Name }).For<User>(o => o.IsAdmin);

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Cat { Name = "MacBeth" } }.AsQueryable();
        };

        private Because of = () => ex = Catch.Exception(() => result = animals.MapFor(user).To<Dto>().ToList());

        private It should_not_map_the_records = () => ex.ShouldBeOfType<NoMappingsFoundException>();
    }

    public class When_a_conditional_mapping_is_not_matched_but_there_is_a_default_for_multiple_records : ConditionalMappings
    {
        protected static User user;

        protected static IQueryable<Cat> cats;

        private Establish context = () =>
        {
            user = new User { IsAdmin = false };

            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Unknown", Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Unknown", Name = o.Name, Owner = o.Owner }).For<User>(o => o.IsAdmin);

            cats = new List<Cat> { new Cat { Name = "Aeris", Owner = "Peter & Kathryn" }, new Cat { Name = "MacBeth", Owner = "Peter & Kathryn" } }.AsQueryable();
        };

        private Because of = () => result = cats.MapFor(user).To<Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Unknown");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_not_map_the_owner_property_for_the_first_record =
           () => string.IsNullOrEmpty(result.ElementAt(0).Owner).ShouldBeTrue();

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");

        private It should_not_map_the_owner_property_for_the_second_record =
            () => string.IsNullOrEmpty(result.ElementAt(1).Owner).ShouldBeTrue();
    }

    #endregion

    #region Composite conditional single mappings

    public class When_performing_composite_conditional_mapping_on_single_record : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { AnimalName = "Unknown", Name = o.Name }).For<User>(o => o.IsAdmin);

            cat = new Cat { Name = "Aeris" };
        };

        private Because of = () => singleresult = cat.MapFor<Animal, User>(user).To<Dto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Unknown");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");
    }

    public class When_performing_composite_conditional_mapping_on_a_single_record : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>().For<User>(o => o.IsAdmin);

            cat = new Cat { Name = "Aeris" };
        };

        private Because of = () => singleresult = cat.MapFor(user).To<Dto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");
    }

    public class When_performing_composite_conditional_mapping_on_a_single_record_that_is_not_matched : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = false };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>().For<User>(o => o.IsAdmin);

            cat = new Cat { Name = "Aeris" };
        };

        private Because of = () => ex = Catch.Exception(() => singleresult = cat.MapFor(user).To<Dto>());

        private It should_throw_an_exception = () => ex.ShouldBeOfType<NoMappingsFoundException>();
    }

    public class When_performing_composite_conditional_mapping_on_a_single_record_where_the_condition_is_not_matched_but_there_is_a_default : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = false };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Cat o) => new Dto { Owner = o.Owner }).For<User>(o => o.IsAdmin).WithDefaults();

            cat = new Cat { Name = "Aeris", Owner = "Peter & Kathryn" };
        };

        private Because of = () => singleresult = cat.MapFor(user).To<Dto>();

        private It should_map_the_record_using_the_base_type = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");

        private It should_not_map_the_owner_property_for_the_first_record =
            () => string.IsNullOrEmpty(singleresult.Owner).ShouldBeTrue();
    }

    public class When_performing_composite_conditional_mapping_on_a_single_record_when_told_to_use_defaults : ConditionalMappings
    {
        protected static Dto singleresult;

        protected static Cat cat;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Cat o) => new Dto { Owner = o.Owner }).For<User>(o => o.IsAdmin).WithDefaults();

            cat = new Cat { Name = "Aeris", Owner = "Peter & Kathryn" };
        };

        private Because of = () => singleresult = cat.MapFor(user).To<Dto>();

        private It should_map_the_record_using_the_base_type = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");

        private It should_not_map_the_owner_property_for_the_first_record =
            () => singleresult.Owner.ShouldEqual("Peter & Kathryn");
    }

    #endregion

    #region Composite conditional multiple mappings

    public class When_performing_composite_conditional_mapping_on_a_derived_type : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { AnimalName = "Unknown", Name = o.Name }).For<User>(o => o.IsAdmin);

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Cat { Name = "MacBeth" } }.AsQueryable();
        };

        private Because of = () => result = animals.MapFor(user).To<Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Unknown");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");
    }

    public class When_performing_composite_conditional_mapping_on_a_derived_type_with_no_match : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = false };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { AnimalName = "Unknown", Name = o.Name }).For<User>(o => o.IsAdmin);

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Cat { Name = "MacBeth" } }.AsQueryable();
        };

        private Because of = () => ex = Catch.Exception(() => result = animals.MapFor(user).To<Dto>().ToList());

        private It should_throw_an_exception = () => ex.ShouldBeOfType<NoMappingsFoundException>();
    }

    public class When_performing_composite_conditional_mapping : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>().For<User>(o => o.IsAdmin);

            animals = new List<Animal> { new Cat { Id = 1, Name = "Aeris" }, new Cat { Id = 2, Name = "MacBeth" } }.AsQueryable();
        };

        private Because of = () => result = animals.MapFor(user).To<Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");
    }

    public class When_performing_composite_conditional_mapping_that_is_not_matched : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = false };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>().For<User>(o => o.IsAdmin);

            animals = new List<Animal> { new Cat { Id = 1, Name = "Aeris" }, new Cat { Id = 2, Name = "MacBeth" } }.AsQueryable();
        };

        private Because of = () => ex = Catch.Exception(() => result = animals.MapFor(user).To<Dto>().ToList());

        private It should_throw_an_exception = () => ex.ShouldBeOfType<NoMappingsFoundException>();
    }

    public class When_performing_composite_conditional_mapping_that_is_not_matched_but_there_is_a_default : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = false };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Owner = o.Owner }).For<User>(o => o.IsAdmin);

            animals = new List<Animal> { new Cat { Id = 1, Name = "Aeris", Owner = "Peter & Kathryn" }, new Cat { Id = 2, Name = "MacBeth", Owner = "Peter & Kathryn" } }.AsQueryable();
        };

        private Because of = () => result = animals.MapFor(user).To<Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_not_map_the_owner_property_for_the_first_record =
            () => string.IsNullOrEmpty(result.ElementAt(0).Owner).ShouldBeTrue();

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");

        private It should_not_map_the_owner_property_for_the_second_record =
            () => string.IsNullOrEmpty(result.ElementAt(1).Owner).ShouldBeTrue();
    }

    public class When_performing_composite_conditional_mapping_when_told_to_use_defaults : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Cat o) => new Dto { Owner = o.Owner }).For<User>(o => o.IsAdmin).WithDefaults();

            animals = new List<Animal> { new Cat { Id = 1, Name = "Aeris", Owner = "Peter & Kathryn" }, new Cat { Id = 2, Name = "MacBeth", Owner = "Peter & Kathryn" } }.AsQueryable();
        };

        private Because of = () => result = animals.MapFor(user).To<Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_not_map_the_owner_property_for_the_first_record =
            () => result.ElementAt(0).Owner.ShouldEqual("Peter & Kathryn");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");

        private It should_not_map_the_owner_property_for_the_second_record =
            () => result.ElementAt(1).Owner.ShouldEqual("Peter & Kathryn");
    }

    public class When_performing_composite_conditional_mapping_on_multiple_derived_types : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>().For<User>(o => o.IsAdmin);
            xMap.Define((Dog o) => new Dto { AnimalName = "Dog" }).DerivedFrom<Animal>().For<User>(o => o.IsAdmin);

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Cat { Name = "MacBeth" }, new Dog { Name = "Pugface" } }.AsQueryable();
        };

        private Because of = () => result = animals.MapFor(user).To<Dto>().ToList();

        private It should_map_three_records = () => result.Count.ShouldEqual(3);

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_map_the_animalname_property_for_the_first_record =
        () => result.ElementAt(0).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");

        private It should_map_the_animalname_property_for_the_second_record =
        () => result.ElementAt(1).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_third_record =
             () => result.ElementAt(2).Name.ShouldEqual("Pugface");

        private It should_map_the_animalname_property_for_the_third_record =
        () => result.ElementAt(2).AnimalName.ShouldEqual("Dog");
    }

    public class When_performing_composite_conditional_mapping_on_multiple_derived_types_but_no_match : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = false };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>().For<User>(o => o.IsAdmin);
            xMap.Define((Dog o) => new Dto { AnimalName = "Dog" }).DerivedFrom<Animal>().For<User>(o => o.IsAdmin);

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Cat { Name = "MacBeth" }, new Dog { Name = "Pugface" } }.AsQueryable();
        };

        private Because of = () => ex = Catch.Exception(() => result = animals.MapFor(user).To<Dto>().ToList());

        private It should_throw_an_exception = () => ex.ShouldBeOfType<NoMappingsFoundException>();
    }

    public class When_performing_composite_conditional_mapping_on_multiple_derived_types_of_which_one_does_not_match : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = false };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>().For<User>(o => o.IsAdmin);
            xMap.Define((Dog o) => new Dto { AnimalName = "Dog" }).DerivedFrom<Animal>();

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Cat { Name = "MacBeth" }, new Dog { Name = "Pugface" } }.AsQueryable();
        };

        private Because of = () => result = animals.MapFor(user).To<Dto>().ToList();

        private It should_map_only_one_record = () => result.Count.ShouldEqual(1);

        private It should_map_the_name_property_for_the_record =
              () => result.ElementAt(0).Name.ShouldEqual("Pugface");

        private It should_map_the_animalname_property_for_the_record =
        () => result.ElementAt(0).AnimalName.ShouldEqual("Dog");
    }

    public class When_performing_composite_conditional_mapping_on_multiple_derived_types_controlled_via_the_base_type : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = true };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name }).For<User>(o => o.IsAdmin);
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Dog o) => new Dto { AnimalName = "Dog" }).DerivedFrom<Animal>();

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Cat { Name = "MacBeth" }, new Dog { Name = "Pugface" } }.AsQueryable();
        };

        private Because of = () => result = animals.MapFor(user).To<Dto>().ToList();

        private It should_map_three_records = () => result.Count.ShouldEqual(3);

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_map_the_animalname_property_for_the_first_record =
        () => result.ElementAt(0).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");

        private It should_map_the_animalname_property_for_the_second_record =
        () => result.ElementAt(1).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_third_record =
             () => result.ElementAt(2).Name.ShouldEqual("Pugface");

        private It should_map_the_animalname_property_for_the_third_record =
        () => result.ElementAt(2).AnimalName.ShouldEqual("Dog");
    }

    public class When_performing_composite_conditional_mapping_on_multiple_derived_types_but_the_base_does_not_match : ConditionalMappings
    {
        protected static IQueryable<Animal> animals;

        protected static User user;

        private Establish context = () =>
        {
            user = new User { IsAdmin = false };

            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name }).For<User>(o => o.IsAdmin);
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Dog o) => new Dto { AnimalName = "Dog" }).DerivedFrom<Animal>();

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Cat { Name = "MacBeth" }, new Dog { Name = "Pugface" } }.AsQueryable();
        };

        private Because of = () => ex = Catch.Exception(() => result = animals.MapFor(user).To<Dto>().ToList());

        private It should_throw_an_exception = () => ex.ShouldBeOfType<NoMappingsFoundException>();
    }

    #endregion

    #region Edge cases

    public class When_a_conditional_mapping_with_nested_projection_is_told_to_include_defaults : ConditionalMappings
    {
        protected static ComplexDto singleresult;

        protected static Dog dog;

        protected static User user;

        private Establish context = () =>
            {
                xMap.Reset();
                xMap.Define((Dog o) => new ComplexDto { AnimalName = "Dog" });
                xMap.Define(
                    (Dog o) =>
                    new ComplexDto
                        {
                            Name = o.Name,
                            HomeClub = o.DogClubMemberships.FirstOrDefault(d => d.ClubName == "Home")
                        })
                    .For<User>(o => o.IsAdmin)
                    .WithDefaults();

                user = new User { IsAdmin = true };
                dog = new Dog
                          {
                              Name = "Pugface",
                              Owner = "Pete & Kathryn",
                              DogClubMemberships =
                                  new Collection<ClubMemberships>(
                                  new List<ClubMemberships>
                                      {
                                          new ClubMemberships { ClubName = "Away" },
                                          new ClubMemberships { ClubName = "Home" },
                                      })
                          };
            };

        private Because of = () => singleresult = dog.MapFor(user).To<ComplexDto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Dog");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Pugface");

        private It should_project_the_home_club_for_the_first_record =
            () => singleresult.HomeClub.ClubName.ShouldEqual("Home");
    }

    #endregion
}
