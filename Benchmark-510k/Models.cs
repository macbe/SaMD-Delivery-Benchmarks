using System.Text.Json.Serialization;

// Input
public record TrackedDevices
{
    public List<DeviceQuery> Devices { get; init; } = [];
}

public record DeviceQuery
{
    public required string Manufacturer { get; init; }
    public List<string> Aliases { get; init; } = [];
}

// Open FDA response
public record OpenFdaResponse
{
    [JsonPropertyName("meta")]
    public OpenFdaMeta? Meta { get; init; }

    [JsonPropertyName("results")]
    public List<OpenFda510kResult> Results { get; init; } = [];
}

public record OpenFdaMeta
{
    [JsonPropertyName("results")]
    public OpenFdaResultsMeta? Results { get; init; }
}

public record OpenFdaResultsMeta
{
    [JsonPropertyName("skip")]
    public int Skip { get; init; }

    [JsonPropertyName("limit")]
    public int Limit { get; init; }

    [JsonPropertyName("total")]
    public int Total { get; init; }
}

public record OpenFda510kResult
{
    [JsonPropertyName("applicant")]
    public string? Applicant { get; init; }

    [JsonPropertyName("device_name")]
    public string? DeviceName { get; init; }

    [JsonPropertyName("date_received")]
    public string? DateReceived { get; init; }

    [JsonPropertyName("decision_date")]
    public string? DecisionDate { get; init; }
}

// Domain
public record DeviceData
{
    public required string DeviceName { get; init; }
    public required string Manufacturer { get; init; }
    public List<SubmissionRecord> Submissions { get; init; } = [];

    public DateOnly? LastSubmissionDate() =>
        Submissions.Any() ? Submissions.Last().SubmissionDate : null;

    public double? MeanDaysToDecision()
    {
        var days = Submissions
            .Where(s => s.DecisionDate.HasValue)
            .Select(s => (double)(s.DecisionDate!.Value.DayNumber - s.SubmissionDate.DayNumber))
            .ToList();

        return days.Any() ? days.Average() : null;
    }

    public double? MeanDaysBetweenSubmissions()
    {
        if (Submissions.Count < 2) return null;

        var first = Submissions.First().SubmissionDate;
        var last = Submissions.Last().SubmissionDate;
        var daysDifference = last.DayNumber - first.DayNumber;

        return daysDifference / (double)(Submissions.Count - 1);
    }

    public int? DaysSinceLastSubmission()
    {
        var lastSubmission = LastSubmissionDate();
        if (!lastSubmission.HasValue) return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return today.DayNumber - lastSubmission.Value.DayNumber;
    }
}

public record SubmissionRecord(
    DateOnly SubmissionDate,
    DateOnly? DecisionDate
);
