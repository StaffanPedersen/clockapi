
namespace ClockApi.Interfaces;
   
public interface IBlogController
{
    public int Id { get; set; }
    public string? Title { get; set; }

    public string? Text { get; set; }

    public string? Image { get; set; }
}