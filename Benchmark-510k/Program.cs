using System.Text.Json;
using MathNet.Numerics.Statistics;

var trackedDevices = JsonSerializer.Deserialize<TrackedDevices>(File.ReadAllText("devices.json"), new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
}) ?? throw new InvalidOperationException($"Failed to read devices.json");

var openFdaClient = new OpenFdaClient();
var devices = await openFdaClient.FetchAllSubmissionsAsync(trackedDevices);

var meanDecisionDays = devices.Select(d => d.MeanDaysToDecision()).OfType<double>().ToList();
var meanDaysBetweenSubmissions = devices.Select(d => d.MeanDaysBetweenSubmissions()).OfType<double>().ToList();
var daysSinceLastSubmission = devices.Select(d => d.DaysSinceLastSubmission()).OfType<int>().Select(v => (double)v).ToList();

Console.WriteLine("| Percentile | Mean Days Between Submissions | Days Since Last Submission | Mean Time to Decision |");
Console.WriteLine("|:----------:|:-----------------------------:|:--------------------------:|:---------------------:|");
foreach (var p in new[] { 10, 25, 50, 75, 90 })
{
    Console.WriteLine($"| {p}% | {Statistics.Percentile(meanDaysBetweenSubmissions, p):F0} | {Statistics.Percentile(daysSinceLastSubmission, p):F0} | {Statistics.Percentile(meanDecisionDays, p):F0} |");
}
Console.WriteLine($"| **n** | **{meanDaysBetweenSubmissions.Count}** | **{daysSinceLastSubmission.Count}** | **{meanDecisionDays.Count}** |");
