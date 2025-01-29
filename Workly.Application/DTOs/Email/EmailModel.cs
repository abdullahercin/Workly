namespace Workly.Application.DTOs.Email;

public class EmailModel
{
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
}