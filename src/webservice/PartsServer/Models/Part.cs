namespace PartsServer.Models;

public class Part
{
    public required string PartID { get; set; }
    public required string PartName { get; set; }
    public required List<string> Suppliers { get; set; }
    public DateTime PartAvailableDate { get; set; }
    public required string PartType { get; set; }
    public string Href => $"api/parts/{PartID}";
}
