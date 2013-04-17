namespace xMap.Tests.Integration.Sql
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using Machine.Specifications;

    public abstract class SqlClassMappings
    {
        protected static TestDbContext testDb;

        protected static IQueryable<Animal> multipleAnimals;

        protected static IQueryable<Animal> animals;

        protected static IQueryable<Cat> cats;

        protected static List<Dto> result;

        private Establish context = () =>
            {
                testDb = new TestDbContext();
                Database.SetInitializer(new DropCreateDatabaseAlways<TestDbContext>());
                testDb.Database.Initialize(true);

                testDb.AnimalsCollection.Add(new Cat { Name = "Aeris" });
                testDb.AnimalsCollection.Add(new Cat { Name = "MacBeth" });
                testDb.AnimalsCollection.Add(new Dog { Name = "PugFace", Age = 5 });

                testDb.AnimalsCollection.Add(
                    new Dog
                        {
                            Name = "Fido",
                            Age = 8,
                            DogClubMemberships =
                                new List<ClubMemberships> { new ClubMemberships { ClubName = "Dog club" } }
                        });

                testDb.EmployeesCollection.Add(new Employee { Name = "Pete", HomeTown = "London" });
                testDb.EmployeesCollection.Add(new Employee { Name = "Karl", HomeTown = "Slough" });
                testDb.EmployeesCollection.Add(new Employee { Name = "Toby", HomeTown = "Watlington" });

                testDb.SaveChanges();

                cats = testDb.AnimalsCollection.OfType<Cat>();
                animals = testDb.AnimalsCollection.OfType<Cat>();
                multipleAnimals = testDb.AnimalsCollection;
            };
    }

    public class When_mapping_a_single_type : SqlClassMappings
    {
        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat", Name = o.Name });
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

    public class When_mapping_a_base_type : SqlClassMappings
    {
        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { AnimalName = "Unknown", Name = o.Name });
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

    public class When_performing_composite_mapping : SqlClassMappings
    {
        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
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

    public class When_performing_composite_mapping_via_interfaces : SqlClassMappings
    {
        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((INamedEntity o) => new Dto { Name = o.Name });
            xMap.Define((Employee o) => new Dto { Origin = o.HomeTown }).DerivedFrom<INamedEntity>();
        };

        private Because of = () => result = testDb.EmployeesCollection.Map<Employee, Dto>().ToList();

        private It should_map_two_records = () => result.Count.ShouldEqual(3);

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Pete");

        private It should_map_the_name_property_for_the_second_record =
             () => result.ElementAt(1).Name.ShouldEqual("Karl");

        private It should_map_the_name_property_for_the_third_record =
             () => result.ElementAt(2).Name.ShouldEqual("Toby");
    }

    public class When_performing_symmetrical_composite_mapping_with_multiple_derived_types : SqlClassMappings
    {
        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Dog o) => new Dto { AnimalName = "Dog" }).DerivedFrom<Animal>();
        };

        private Because of = () => result = multipleAnimals.Where(o => o.Name != "Fido").Map<Animal, Dto>().ToList();

        private It should_ignore_the_implicit_mappings = () => result.Count.ShouldEqual(3);

        private It should_use_the_constant_value_in_the_first_record =
            () => result.ElementAt(0).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_use_the_constant_value_in_the_second_record =
           () => result.ElementAt(1).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_second_record =
            () => result.ElementAt(1).Name.ShouldEqual("MacBeth");

        private It should_use_the_constant_value_in_the_third_record =
           () => result.ElementAt(2).AnimalName.ShouldEqual("Dog");

        private It should_map_the_name_property_for_the_third_record =
            () => result.ElementAt(2).Name.ShouldEqual("PugFace");
    }

    public class When_performing_asymetrical_composite_mapping_with_simple_types : SqlClassMappings
    {
        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new Dto { Name = o.Name });
            xMap.Define((Cat o) => new Dto { AnimalName = "Cat" }).DerivedFrom<Animal>();
            xMap.Define((Dog o) => new Dto { AnimalName = "Dog", Age = o.Age }).DerivedFrom<Animal>();
        };

        private Because of = () => result = multipleAnimals.Where(o => o.Name != "Fido").Map<Animal, Dto>().ToList();

        private It should_ignore_the_implicit_mappings = () => result.Count.ShouldEqual(3);

        private It should_use_the_constant_value_in_the_first_record =
            () => result.ElementAt(0).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("Aeris");

        private It should_use_the_constant_value_in_the_second_record =
           () => result.ElementAt(1).AnimalName.ShouldEqual("Cat");

        private It should_map_the_name_property_for_the_second_record =
            () => result.ElementAt(1).Name.ShouldEqual("MacBeth");

        private It should_use_the_constant_value_in_the_third_record =
           () => result.ElementAt(2).AnimalName.ShouldEqual("Dog");

        private It should_map_the_name_property_for_the_third_record =
            () => result.ElementAt(2).Name.ShouldEqual("PugFace");
    }

    public class When_performing_symmetrical_mapping_with_complex_types : SqlClassMappings
    {
        protected static List<ComplexDto> result;

        private Establish context = () =>
        {
            xMap.Reset();
            xMap.Define((Animal o) => new ComplexDto { Name = o.Name });
            xMap.Define((Dog o) => new ComplexDto { AnimalName = "Dog", Memberships = o.DogClubMemberships })
                .DerivedFrom<Animal>();
        };

        private Because of = () => result = multipleAnimals.Map<Animal, ComplexDto>().OrderByDescending(o => o.Name).ToList();

        private It should_map_all_the_dog_records = () => result.Count.ShouldEqual(2);

        private It should_use_the_constant_value_in_the_first_record =
            () => result.ElementAt(0).AnimalName.ShouldEqual("Dog");

        private It should_map_the_name_property_for_the_first_record =
            () => result.ElementAt(0).Name.ShouldEqual("PugFace");

        private It should_use_the_constant_value_in_the_second_record =
           () => result.ElementAt(1).AnimalName.ShouldEqual("Dog");

        private It should_map_the_name_property_for_the_second_record =
            () => result.ElementAt(1).Name.ShouldEqual("Fido");

        private It should_populate_the_list_for_the_second_record =
            () => result.ElementAt(1).Memberships.Count().ShouldEqual(1);

        private It should_map_the_complex_property_for_the_second_record =
            () => result.ElementAt(1).Memberships.ElementAt(0).ClubName.ShouldEqual("Dog club");
    }
}
