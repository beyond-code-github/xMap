namespace xMap.Tests.Unit
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    public abstract class ClassMappings
    {
        protected static List<Dto> result;
    }

    public class When_mapping_an_isolated_type_single_record : ClassMappings
    {
        protected static Dto singleresult;
        protected static Cat cat;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name });

            cat = new Cat { Name = "Aeris" };
        };

        private Because of = () => singleresult = cat.Map<Cat, Dto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");
    }

    public class When_mapping_an_isolated_type : ClassMappings
    {
        protected static IQueryable<Cat> cats;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name });

            cats = new List<Cat> { new Cat { Name = "Aeris" }, new Cat { Name = "MacBeth" } }.AsQueryable();
        };

        private Because of = () => result = cats.Map<Cat, Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");
    }

    public class When_mapping_a_derived_type_single_record : ClassMappings
    {
        protected static Dto singleresult;
        protected static Cat cat;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { AnimalName = "Unknown", Name = o.Name });

            cat = new Cat { Name = "Aeris" };
        };

        private Because of = () => singleresult = cat.Map<Animal, Dto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Unknown");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");
    }

    public class When_mapping_a_derived_type : ClassMappings
    {
        protected static IQueryable<Animal> animals;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { AnimalName = "Unknown", Name = o.Name });

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Cat { Name = "MacBeth" } }.AsQueryable();
        };

        private Because of = () => result = animals.Map<Animal, Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Unknown");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");
    }

    public class When_performing_composite_mapping_single_record : ClassMappings
    {
        protected static Dto singleresult;
        protected static Cat cat;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();

            cat = new Cat { Name = "Aeris" };
        };

        private Because of = () => singleresult = cat.Map<Cat, Dto>();

        private It should_map_the_record = () => singleresult.ShouldNotBeNull();

        private It should_use_the_constant_value_in_all_mapped_records =
            () => singleresult.AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => singleresult.Name.ShouldEqual("Aeris");
    }

    public class When_performing_composite_mapping : ClassMappings
    {
        protected static IQueryable<Animal> animals;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();

            animals = new List<Animal> { new Cat { Id = 1, Name = "Aeris" }, new Cat { Id = 2, Name = "MacBeth" } }.AsQueryable();
        };

        private Because of = () => result = animals.Map<Animal, Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_all_mapped_records =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("MacBeth");
    }

    public class When_performing_composite_mapping_with_implicit_derived_type : ClassMappings
    {
        protected static IQueryable<Animal> animals;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Dog { Name = "PugFace" } }.AsQueryable();
        };

        private Because of = () => result = animals.Map<Animal, Dto>().ToList();

        private It should_ignore_the_implicit_mappings = () => result.Count.ShouldEqual(1);

        private It should_use_the_constant_value_in_the_explicit_record =
            () => result.ShouldEachConformTo(o => o.AnimalName == "Cat");

        private It should_map_the_name_property_for_the_explicit_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");
    }

    public class When_performing_composite_mapping_multiple_derived_types : ClassMappings
    {
        protected static IQueryable<Animal> animals;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Dog o) => new Dto { AnimalName = "Dog" }).DerivedFrom<Animal>();

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Dog { Name = "PugFace" } }.AsQueryable();
        };

        private Because of = () => result = animals.Map<Animal, Dto>().ToList();

        private It should_ignore_the_implicit_mappings = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_the_first_record =
            () => result.ElementAt(0).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_use_the_constant_value_in_the_second_record =
           () => result.ElementAt(1).AnimalName.ShouldEqual("Dog");

        private It should_map_the_name_property_for_the_second_record =
            () => result.ElementAt(1).Name.ShouldEqual("PugFace");
    }

    public class When_performing_composite_mapping_with_different_projections : ClassMappings
    {
        protected static IQueryable<Animal> animals;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Dog o) => new Dto { AnimalName = "Dog", Age = o.Age }).DerivedFrom<Animal>();

            animals = new List<Animal> { new Cat { Name = "Aeris" }, new Dog { Name = "PugFace", Age = 5 } }.AsQueryable();
        };

        private Because of = () => result = animals.Map<Animal, Dto>().ToList();

        private It should_map_the_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_the_first_record =
            () => result.ElementAt(0).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_use_the_constant_value_in_the_second_record =
           () => result.ElementAt(1).AnimalName.ShouldEqual("Dog");

        private It should_map_the_name_property_for_the_second_record =
            () => result.ElementAt(1).Name.ShouldEqual("PugFace");

        private It should_map_the_age_property_for_the_third_record =
           () => result.ElementAt(1).Age.ShouldEqual(5);
    }

    public class When_performing_composite_mapping_with_complex_types : ClassMappings
    {
        protected static IQueryable<Animal> animals;

        protected static List<ComplexDto> result;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new ComplexDto { Name = o.Name, Memberships = null });
            xMap.Define((Cat o) => new ComplexDto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Dog o) => new ComplexDto { AnimalName = "Dog", Memberships = o.DogClubMemberships.AsEnumerable() })
                .DerivedFrom<Animal>();

            animals =
                new List<Animal>
                    {
                        new Cat { Name = "Aeris" },
                        new Dog { Name = "PugFace", DogClubMemberships = new List<ClubMemberships> { new ClubMemberships { ClubName = "Dog club" } } }
                    }
                    .AsQueryable();
        };

        private Because of = () => result = animals.Map<Animal, ComplexDto>().ToList();

        private It should_ignore_the_implicit_mappings = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_the_first_record =
            () => result.ElementAt(0).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_use_the_constant_value_in_the_second_record =
           () => result.ElementAt(1).AnimalName.ShouldEqual("Dog");

        private It should_map_the_name_property_for_the_second_record =
            () => result.ElementAt(1).Name.ShouldEqual("PugFace");

        private It should_populate_the_list_for_the_second_record =
            () => result.ElementAt(1).Memberships.Count().ShouldEqual(1);

        private It should_map_the_complex_property_for_the_second_record =
            () => result.ElementAt(1).Memberships.ElementAt(0).ClubName.ShouldEqual("Dog club");
    }
}
