using DogGo.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using DogGo.Repositories;

namespace DogGo.Repositories
{
    public interface IOwnerRepository
    {
        List<Owner> GetAllOwners();
        Owner GetOwnerById(int id);
    }


}