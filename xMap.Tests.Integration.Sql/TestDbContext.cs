namespace xMap.Tests.Integration.Sql
{
    using System.Data.Entity;

    public class TestDbContext : DbContext
    {
        public DbSet<Animal> AnimalsCollection { get; set; }

        public DbSet<Employee> EmployeesCollection { get; set; }
    }
}
