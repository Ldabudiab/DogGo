using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public DogRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       select Dog.Id as DogId, Dog.Name, Breed, Notes, ImageUrl, Owner.Id as OwnerId, Owner.Name as OwnerName
                       from Dog
                       join Owner on Owner.Id = Dog.OwnerId
                       
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Dog> dogs = new List<Dog>();
                        while (reader.Read())
                        {
                            Dog dog = new Dog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Owner = new Owner
                                {
                                    Name = reader.GetString(reader.GetOrdinal("OwnerName"))
                                }

                               
                            };

                            dogs.Add(dog);
                        }

                        return dogs;
                    }
                }
            }
        }
       
        public void UpdateDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Dog
                            SET 
                                [Name] = @name, 
                                Breed = @breed, 
                                OwnerId = @ownerId
                               
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", dog.Name);                 
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@id", dog.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Dog (Dog.Name, Breed, OwnerId )
                    OUTPUT INSERTED.ID
                    VALUES (@name, @breed, @ownerId);
                ";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);                   
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);

                    int id = (int)cmd.ExecuteScalar();

                    dog.Id = id;
                }
            }
        }
        public void DeleteDog(int dogId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Dog
                            WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", dogId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Dog GetDogById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        select Dog.Id as DogId, Dog.Name, Breed, Notes, ImageUrl, Owner.Id as OwnerId, Owner.Name as OwnerName
                        from Dog
                        join Owner on Owner.Id = Dog.OwnerId
                        WHERE Dog.Id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Dog dog = new Dog
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Owner = new Owner
                                {
                                    Name = reader.GetString(reader.GetOrdinal("OwnerName"))
                                }
                            };

                            return dog;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
    }
}