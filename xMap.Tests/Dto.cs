namespace xMap.Tests
{
    using System.Collections.Generic;

    public class Dto
    {
        public int Id { get; set; }

        public string AnimalName { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Origin { get; set; }

        public string Owner { get; set; }
    }

    public class ComplexDto
    {
        public int Id { get; set; }

        public string AnimalName { get; set; }

        public string Name { get; set; }

        public IEnumerable<ClubMemberships> Memberships { get; set; }
    }
}
