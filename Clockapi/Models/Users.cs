namespace ClockApi.Models;
using Interfaces;
using System.ComponentModel.DataAnnotations;

public class User : IUser
{
    public int Id { get; set; }
    [MaxLength(100)]
    public required string Username { get; set; }
    [MaxLength(100)]
    public required string PasswordHash { get; set; }
    public required string Salt { get; set; }
    public required string Roles { get; set; } // Comma-separated roles
    [MaxLength(100)]
    public required string Email { get; set; } 
}