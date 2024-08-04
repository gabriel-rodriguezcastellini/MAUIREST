using System.Text;

namespace PartsClient.Data;

[Serializable]
public class Part
{
    public string PartID { get; set; }

    public string PartName { get; set; }

    public string TheSuppliers { get; set; }

    public string PartType { get; set; }

    public List<string> Suppliers { get; set; } = [];
    public DateTime PartAvailableDate { get; set; }

    public string SupplierString
    {
        get
        {
            StringBuilder result = new();
            foreach (string supplier in Suppliers)
            {
                _ = result.Append($"{supplier}, ");
            }
            return result.ToString().Trim(',', ' ');
        }
    }
}
