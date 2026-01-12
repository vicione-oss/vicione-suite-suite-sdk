using Microsoft.EntityFrameworkCore;
using Sdk.Backend.Persistence;
using TestModule.Backend.Contracts;

namespace TestModule.Backend.DbContext;

//Mock this for unit tests:
public interface IReferenceDbContext : IModuleDbContext
{
    public DbSet<Employees> Employees { get; }
    public DbSet<Departments> Departments { get; }
    public DbSet<DepartmentEmployees> DepartmentEmployees { get; }
    public DbSet<DepartmentManager> DepartmentManagers { get; }
    public DbSet<Titles> Titles { get; }
    public DbSet<Salaries> Salaries { get; }
}
