using System;

namespace sisNet.models;

public enum UserRole
{
    Cliente,
    Backoffice,
    Comercial,
    Admin,
    JefeZona,
    None
}

public interface IUser
{
    int Id { get; set; }
    string Name { get; set; }
    UserRole Role { get; set; }
    string Username { get; set; }
    string Password { get; set; }

}
