# Benchmark-510k

A command-line tool that benchmarks 510(k) submissions using publicly available data.

## Installation

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

### Steps

```bash
# Clone the repository
git clone https://github.com/macbe/SaMD-Delivery-Benchmarks.git

# Navigate to the Benchmark-510k directory
cd SaMD-Delivery-Benchmarks/Benchmark-510k

# Build the project
dotnet build
```

## Input

The application reads `devices.json` which specifies which medical devices to track, including multiple name variations for devices that have been submitted under different version numbers or naming conventions.

### devices.json Structure

```json
{
  "devices": [
    {
      "manufacturer": "Manufacturer Name",
      "aliases": [
        "Device Name",
        "Device Name v2.0",
        "Device Name Version 3"
      ]
    }
  ]
}
```

- `manufacturer`: The manufacturer name as it appears in FDA 510(k) records (required)
- `aliases`: Array of device name variations to search for and combine (required)

**Important Notes:**
- Device and manufacturer names must match exactly as they appear in the FDA database
- If a device alias is not found, you'll see a warning message during execution
- Each entry in the `devices` array represents one logical device, even if it has multiple aliases

## Usage

### Run the Application

From the project directory:

```bash
dotnet run
```

### Output

The application displays a percentile analysis table showing benchmarks:

- **Mean Days Between Submissions**: Average time between consecutive 510(k) submissions for a device
- **Days Since Last Submission**: How long since each device's most recent submission
- **Mean Time to Decision**: Average time from submission to decision

Percentiles (P10, P25, P50, P75, P90) are calculated across all devices to show distribution of performance. The bottom row (n) shows the number of devices that had sufficient data for each metric.

**Example Output:**

```
| Percentile | Mean Days Between Submissions | Days Since Last Submission | Mean Time to Decision |
| 10% | 244 | 320 | 84 |
| 25% | 434 | 463 | 123 |
| 50% | 611 | 777 | 153 |
| 75% | 908 | 1279 | 200 |
| 90% | 1348 | 1532 | 253 |
|  n  | 36 | 52 | 52 |
```

## Data Source

This application uses the [openFDA API](https://open.fda.gov/apis/device/510k/) to retrieve 510(k) clearance data. The API is public and does not require authentication for basic usage.
