using Dapper;
using ImageUploadMvc.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ImageUploadMvc.Data.Repositories;

public interface IPersonRepository
{
    Task AddPerson(Person person);
    Task DeletePerson(int id);
    Task<IEnumerable<Person>> GetPeople();
    Task<Person?> GetPersonById(int id);
    Task UpdatePerson(Person person);
}

public class PersonRepository : IPersonRepository
{
    private readonly IConfiguration _configuration;
    private string _connectionString;

    public PersonRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("default");
    }

    public async Task AddPerson(Person person)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        string sql = "insert into Person(FirstName,LastName,ProfilePicture) values(@FirstName,@LastName,@ProfilePicture)";
        await connection.ExecuteAsync(sql, new
        {
            FirstName = person.FirstName,
            LastName = person.LastName,
            ProfilePicture = person.ProfilePicture,
        });
    }

    public async Task UpdatePerson(Person person)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        string sql = "update Person set FirstName=@FirstName,LastName=@LastName,ProfilePicture=@ProfilePicture where Id=@Id";
        await connection.ExecuteAsync(sql, person);
    }

    public async Task DeletePerson(int id)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        string sql = "delete from Person where id=@id";
        await connection.ExecuteAsync(sql, new { id });
    }

    public async Task<Person?> GetPersonById(int id)
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        string sql = "select * from Person where id=@id";
        var person = await connection.QueryFirstOrDefaultAsync<Person>(sql, new { id });
        return person;
    }

    public async Task<IEnumerable<Person>> GetPeople()
    {
        using IDbConnection connection = new SqlConnection(_connectionString);
        string sql = "select * from Person";
        var person = await connection.QueryAsync<Person>(sql);
        return person;
    }

}
