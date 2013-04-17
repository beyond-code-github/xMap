namespace xMap.Tests
{
    using System.Collections.Generic;

    public class Dog : Animal
    {
        public ICollection<ClubMemberships> DogClubMemberships { get; set; }

        public int Age { get; set; }
    }
}
