using System.Net.Http.Json;
using System.Text.Json;

namespace PartsClient.Data;

public static class PartsManager
{
    private static readonly string _baseAddress = "https://mslearnpartsserver2189925075.azurewebsites.net";
    private static readonly string _url = $"{_baseAddress}/api/";
    private static string _authorizationKey;
    private static HttpClient _client;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static async Task<HttpClient> GetClient()
    {
        if (_client != null)
        {
            return _client;
        }

        _client = new HttpClient();

        if (string.IsNullOrEmpty(_authorizationKey))
        {
            _authorizationKey = await _client.GetStringAsync($"{_url}login");
            _authorizationKey = JsonSerializer.Deserialize<string>(_authorizationKey);
        }

        _client.DefaultRequestHeaders.Add("Authorization", _authorizationKey);
        _client.DefaultRequestHeaders.Add("Accept", "application/json");

        return _client;
    }

    public static async Task<IEnumerable<Part>> GetAll()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            return [];
        }

        HttpClient client = await GetClient();
        string result = await client.GetStringAsync($"{_url}parts");

        return JsonSerializer.Deserialize<List<Part>>(result, _jsonSerializerOptions);
    }

    public static async Task<Part> Add(string partName, string supplier, string partType)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            return new Part();
        }

        Part part = new()
        {
            PartName = partName,
            Suppliers = new List<string>([supplier]),
            PartID = string.Empty,
            PartType = partType,
            PartAvailableDate = DateTime.Now.Date
        };
        HttpRequestMessage msg = new(HttpMethod.Post, $"{_url}parts")
        {
            Content = JsonContent.Create<Part>(part)
        };
        HttpResponseMessage response = await _client.SendAsync(msg);
        _ = response.EnsureSuccessStatusCode();
        string returnedJson = await response.Content.ReadAsStringAsync();
        Part insertedPart = JsonSerializer.Deserialize<Part>(returnedJson, _jsonSerializerOptions);
        return insertedPart;
    }

    public static async Task Update(Part part)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            return;
        }

        HttpRequestMessage msg = new(HttpMethod.Put, $"{_url}parts/{part.PartID}")
        {
            Content = JsonContent.Create(part)
        };
        HttpClient client = await GetClient();
        HttpResponseMessage response = await client.SendAsync(msg);
        _ = response.EnsureSuccessStatusCode();
    }

    public static async Task Delete(string partID)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            return;
        }

        HttpRequestMessage msg = new(HttpMethod.Delete, $"{_url}parts/{partID}");
        HttpClient client = await GetClient();
        HttpResponseMessage response = await client.SendAsync(msg);
        _ = response.EnsureSuccessStatusCode();
    }
}
