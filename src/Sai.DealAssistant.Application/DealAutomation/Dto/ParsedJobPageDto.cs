namespace Sai.DealAssistant.Application.DealAutomation.Dto;

public class ParsedJobPageDto
{
    public string? Text { get; set; }
    public string? Title { get; set; }
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public IEnumerable<string>? Responsibilities { get; set; }
    public IEnumerable<string>? Requirements { get; set; }
    public IEnumerable<string>? NiceToHave { get; set; }
    public IEnumerable<string>? Perks { get; set; }
}
