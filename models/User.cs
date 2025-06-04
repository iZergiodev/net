using sisNet.models;

public class User : IUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.None;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; 
}