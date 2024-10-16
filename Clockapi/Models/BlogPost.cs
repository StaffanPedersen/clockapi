namespace ClockApi.Models;
using Interfaces;
using System.ComponentModel.DataAnnotations;

public class BlogPost : IBlogController
{
    public int Id { get; set; }
    [MaxLength(200)]
    public string? Title { get; set; }
    [MaxLength(50000)]
    public string? Text { get; set; }
    
    public string? Image { get; set; }
}
