namespace PartsServer.Models;

public static class PartsFactory
{
    private static readonly object _locker = new();
    public static readonly Dictionary<string, Tuple<DateTime, List<Part>>> Parts = [];

    public static void Initialize(string authorizationToken)
    {
        lock (_locker)
        {
            Parts.Add(authorizationToken,
                Tuple.Create(DateTime.UtcNow.AddHours(1), DefaultParts.ToList()));
        }
    }

    private static IEnumerable<Part> DefaultParts
    {
        get
        {
            yield return new Part
            {
                PartID = "0545685192",
                PartName = "Large motherboard",
                Suppliers = ["A. Datum Corporation", "Allure Bays Corp", "Awesome Computers"],
                PartAvailableDate = new DateTime(2019, 10, 1, 0, 0, 0, kind: DateTimeKind.Unspecified),
                PartType = "Circuit Board",
            };
            yield return new Part
            {
                PartID = "0553801473",
                PartName = "RISC processor",
                Suppliers = ["Allure Bays Corp", "Contoso Ltd", "Parnell Aerospace"],
                PartAvailableDate = new DateTime(2021, 07, 12, 0, 0, 0, kind: DateTimeKind.Unspecified),
                PartType = "CPU",
            };
            yield return new Part
            {
                PartID = "0544272994",
                PartName = "CISC processor",
                Suppliers = ["Fabrikam, Inc", "A. Datum Corporation", "Parnell Aerospace"],
                PartAvailableDate = new DateTime(2020, 9, 4, 0, 0, 0, kind: DateTimeKind.Unspecified),
                PartType = "CPU",
            };
            yield return new Part
            {
                PartID = "141971189X",
                PartName = "High resolution card",
                Suppliers = ["Awesome Computers"],
                PartAvailableDate = new DateTime(2019, 11, 10, 0, 0, 0, kind: DateTimeKind.Unspecified),
                PartType = "Graphics Card",
            };
            yield return new Part
            {
                PartID = "1256324778",
                PartName = "240V/110V switchable",
                Suppliers = ["Reskit"],
                PartAvailableDate = new DateTime(2021, 10, 21, 0, 0, 0, kind: DateTimeKind.Unspecified),
                PartType = "PSU",
            };
        }
    }

    public static void ClearStaleData()
    {
        lock (_locker)
        {
            List<string> keys = [.. Parts.Keys];
            foreach (string? oneKey in keys)
            {
                if (Parts.TryGetValue(oneKey, out Tuple<DateTime, List<Part>>? result) &&
                    result.Item1 < DateTime.UtcNow)
                {
                    _ = Parts.Remove(oneKey);
                }
            }
        }
    }

    private static readonly Random _rng = new();
    public static string CreatePartID()
    {
        char[] ch = new char[10];
        for (int i = 0; i < 10; i++)
        {
            ch[i] = (char)('0' + _rng.Next(0, 9));
        }
        return new string(ch);
    }
}
