# Benchmarking Software Delivery for Software as a Medical Device (SaMD)

DORA publishes software delivery benchmarks in its annual [State of DevOps Report](https://dora.dev/publications) which show deployment frequency, lead time for changes, change failure rate, and mean time to recovery metrics for teams ranging from elite to low performance.

Software as a Medical Device (SaMD) teams must follow a wide range of standards such as IEC 62304 and ISO 13485 and, in some cases, must obtain regulatory clearance before deploying changes. Their code often runs in clinical IT environments, which can mean limited remote access and strict change control processes. These regulatory and operational constraints mean that standard DORA benchmarks may not reflect what 'good' looks like for SaMD teams. 

**Examples of SaMD include:**
- AI algorithms that analyze medical images to detect diseases
- Clinical decision support software that helps physicians diagnose conditions
- Software that analyzes patient data to predict health risks

The goal is to build benchmarks that reflect how SaMD teams operate. This project is a first step toward establishing benchmarks by analyzing public FDA 510(k) data.

## 510(k) data as a proxy

When a manufacturer makes a change to a medical device that could affect safety or effectiveness, they may need to submit a [510(k) premarket notification](https://www.fda.gov/medical-devices/premarket-submissions-selecting-and-preparing-correct-submission/premarket-notification-510k) to the FDA before marketing the device in the US.

The FDA exposes some of this information through the openFDA API, including:
- Device name
- Manufacturer
- Submission date
- Decision date

Not every software update will be significant enough to trigger a 510(k), but we can measure:
- How often significant changes happen
- How long those changes spend in review

## Dataset

Data was collected in November 2025.

1. A set of SaMD products from a range of manufacturers was selected
2. The FDA database was searched for associated product names because some manufacturers include the version number in the name of each submission
3. Products whose most recent 510(k) submission was before January 2020 were excluded

This resulted in a set of 52 actively developed SaMD products across manufacturers ranging from startups to publicly listed companies.

## Metrics

For each of the products, three metrics were tracked:

1. Mean days between submissions

A proxy for how often significant, regulated changes reach the market for that product. This is only calculated for products with at least two submissions.

2. Days since last submission

An indicator of recent regulatory activity. Shorter values suggest a product that is still being actively developed; longer values may suggest a stable or dormant product.

3. Mean time to decision

The average time from submission date to decision date for that product. This includes both the FDA's review and the manufacturer's time to respond to any requests for more information. Longer times can reflect complexity, submission quality, or process delays on either side.

## What the data shows

Across the 52 actively developed SaMD products, the distribution of these metrics looks like this:

| Percentile | Mean Days Between Submissions | Days Since Last Submission | Mean Time to Decision |
|:----------:|:-----------------------------:|:--------------------------:|:---------------------:|
| 10% | 244 | 320 | 84 |
| 25% | 434 | 463 | 123 |
| 50% | 611 | 777 | 153 |
| 75% | 908 | 1279 | 200 |
| 90% | 1348 | 1532 | 253 |
| **n** | **36** | **52** | **52** |

A few observations:
- Significant changes are infrequent. Even the most frequently updated products only ship 510(k)-level changes about every 8 months. The median product is closer to every 20 months.
- There’s a wide spread between products. At the slow end, some products go more than 3 years between significant regulated changes, while others submit several times within a similar period.
- Regulatory review is a substantial part of change lead time. Typical time to decision is 3–6 months (median ~5 months), which places a hard floor under how quickly certain types of changes can reach customers.

## What's next

This 510(k) view is useful because it is simple to generate and is grounded in objective, public data. 

The next step is to figure out how to collect finer-grained delivery metrics from SaMD teams, such as:
- Deployment frequency
- Lead time for changes
- Change failure rate
- Mean time to restore

If you want to help, please reach out.

All code used for this analysis is open source at [Benchmark-510k](https://github.com/macbe/SaMD-Delivery-Benchmarks/tree/main/Benchmark-510k).
