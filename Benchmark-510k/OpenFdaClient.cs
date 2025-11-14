using System.Globalization;
using System.Text.Json;
using System.Web;

public class OpenFdaClient
{
    private const string OpenFdaBaseUrl = "https://api.fda.gov/device/510k.json";
    private const int MaxResultsPerRequest = 1000;
    private readonly HttpClient _httpClient;

    public OpenFdaClient()
    {
        _httpClient = new HttpClient();
    }

    public async Task<List<DeviceData>> FetchAllSubmissionsAsync(TrackedDevices trackedDevices)
    {
        var allDevices = new List<DeviceData>();

        foreach (var device in trackedDevices.Devices)
        {
            var allSubmissionsForDevice = new List<SubmissionRecord>();

            foreach (var alias in device.Aliases)
            {
                Console.WriteLine($"Fetching data for {device.Manufacturer} {alias}...");
                var submissions = await SearchDeviceAsync(device.Manufacturer, alias);
                allSubmissionsForDevice.AddRange(submissions);
            }

            // Use the shortest alias as the canonical device name
            var canonicalName = device.Aliases.OrderBy(a => a.Length).FirstOrDefault() ?? "Unknown";

            if (allSubmissionsForDevice.Any())
            {
                var deviceData = new DeviceData
                {
                    DeviceName = canonicalName,
                    Manufacturer = device.Manufacturer,
                    Submissions = allSubmissionsForDevice
                        .DistinctBy(sr => sr.SubmissionDate)
                        .OrderBy(sr => sr.SubmissionDate)
                        .ToList()
                };

                Console.WriteLine($"Adding {deviceData.DeviceName} with {deviceData.Submissions.Count} submissions to {deviceData.Submissions.Last().SubmissionDate}");
                allDevices.Add(deviceData);
            }
        }

        return allDevices;
    }

    private async Task<List<SubmissionRecord>> SearchDeviceAsync(string manufacturerName, string deviceName)
    {
        var searchQuery = $"applicant:\"{manufacturerName}\" AND device_name:\"{deviceName}\"";
        var results = await FetchAllResultsAsync(searchQuery);

        if (results.Count == 0)
        {
            Console.WriteLine($"Warning: No results found for '{deviceName}' by '{manufacturerName}'");
        }

        return results;
    }

    private async Task<List<SubmissionRecord>> FetchAllResultsAsync(string searchQuery)
    {
        var allSubmissions = new List<SubmissionRecord>();
        var skip = 0;

        while (true)
        {
            var url = BuildApiUrl(searchQuery, skip, MaxResultsPerRequest);

            string response;
            try
            {
                response = await _httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                break;
            }

            var fdaResponse = JsonSerializer.Deserialize<OpenFdaResponse>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (fdaResponse?.Results == null || fdaResponse.Results.Count == 0)
            {
                break;
            }

            var submissions = fdaResponse.Results
                .Select(MapToSubmissionRecord)
                .Where(s => s.SubmissionDate != DateOnly.MinValue)
                .ToList();

            allSubmissions.AddRange(submissions);

            var total = fdaResponse.Meta?.Results?.Total ?? 0;
            if (allSubmissions.Count >= total)
            {
                break;
            }

            skip += MaxResultsPerRequest;
            await Task.Delay(100); // Be respectful to the API
        }

        return allSubmissions;
    }

    private static string BuildApiUrl(string searchQuery, int skip, int limit)
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams["search"] = searchQuery;
        queryParams["limit"] = limit.ToString();

        if (skip > 0)
        {
            queryParams["skip"] = skip.ToString();
        }

        return $"{OpenFdaBaseUrl}?{queryParams}";
    }

    private static SubmissionRecord MapToSubmissionRecord(OpenFda510kResult result)
    {
        var submissionDate = ParseFdaDate(result.DateReceived) ?? DateOnly.MinValue;
        var decisionDate = ParseFdaDate(result.DecisionDate);

        return new SubmissionRecord(submissionDate, decisionDate);
    }

    private static DateOnly? ParseFdaDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        if (DateOnly.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out var date))
        {
            return date;
        }

        if (DateOnly.TryParse(dateString, out var parsedDate))
        {
            return parsedDate;
        }

        return null;
    }
}
