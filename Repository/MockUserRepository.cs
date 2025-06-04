using System.Collections.Generic;
using System.Linq;
using sisNet.models;

namespace sisNet.Repository
{
    public class MockUserRepository
    {
        private readonly List<IUser> _users;

        public MockUserRepository()
        {
            _users = new List<IUser>
        {
            new User { Id = 1, Name = "cliente", Role = UserRole.Cliente, Username = "cliente", Password = "123" },
            new User { Id = 2, Name = "backoffice", Role = UserRole.Backoffice, Username = "backoffice", Password = "123" },
            new User { Id = 3, Name = "comercial", Role = UserRole.Comercial, Username = "comercial", Password = "123" },
            new User { Id = 4, Name = "admin", Role = UserRole.Admin, Username = "admin", Password = "123" },
            new User { Id = 5, Name = "jefeZona", Role = UserRole.JefeZona, Username = "jefeZona", Password = "123" }
        };
        }

        public IEnumerable<IUser> GetAll() => _users;
        public IUser? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);
        public void Add(IUser user) => _users.Add(user);
        public IUser? Authenticate(string username, string password)
        {
            return _users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);
        }
    }
}