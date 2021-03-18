using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    class RoommateRepository : BaseRepository
    {
        public RoommateRepository(string connectionString) : base(connectionString) { }

        public List<Roommate> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Roommate";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Roommate> roommates = new List<Roommate>();

                    while (reader.Read())
                    {
                        int idValue = reader.GetInt32(reader.GetOrdinal("Id"));
                        string firstNameValue = reader.GetString(reader.GetOrdinal("FirstName"));
                        string lastNameValue = reader.GetString(reader.GetOrdinal("LastName"));
                        int rentPortionValue = reader.GetInt32(reader.GetOrdinal("RentPortion"));
                        DateTime moveInValue = reader.GetDateTime(reader.GetOrdinal("MoveInDate"));
                        int roomIdValue = reader.GetInt32(reader.GetOrdinal("RoomId"));

                        Room theirRoom = new Room
                        {
                            Id = roomIdValue
                        };

                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            FirstName = firstNameValue,
                            LastName = lastNameValue,
                            RentPortion = rentPortionValue,
                            Room = theirRoom
                        };

                        roommates.Add(roommate);
                    }

                    reader.Close();

                    return roommates;
                }
            }
        }

        ///<summary>
        /// Returns a single roommate with the given id.
        /// </summary>
        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT rmm.Id, rmm.FirstName, rmm.RentPortion, r.Name as 'Room' 
                                            FROM Roommate rmm 
                                            LEFT JOIN Room r on rmm.RoomId = r.Id 
                                            WHERE rmm.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Roommate roommate = null;
                    Room theirRoom = null;
                    if (reader.Read())
                    {
                        theirRoom = new Room
                        {
                            Name = reader.GetString(reader.GetOrdinal("Room"))
                        };
                        roommate = new Roommate
                        {
                            Id = id,
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                            Room = theirRoom
                        };

                    }

                    reader.Close();

                    return roommate;
                }
            }
        }
    }
}
