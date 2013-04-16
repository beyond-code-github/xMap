namespace xMap.Tests
{
    using System.Collections.Generic;

    public class Dto
    {
        public int Id { get; set; }

        public string AnimalName { get; set; }

        public string Name { get; set; }
    }

    public class ComplexDto
    {
        public int Id { get; set; }

        public string AnimalName { get; set; }

        public string Name { get; set; }

        public List<ClubMemberships> Memberships { get; set; }
    }
}
