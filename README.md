# What is RudderStack?

[RudderStack](https://rudderstack.com/) is a **customer data pipeline** tool for collecting, routing and processing data from your websites, apps, cloud tools, and data warehouse.

More information on RudderStack can be found [here](https://github.com/rudderlabs/rudder-server).

Released under the MIT License.

### Latest Version

`1.0.0`

## Getting Started with .NET SDK

Install `RudderAnalytics` using `NuGet`
```
Install-Package RudderAnalytics -Version <version>
```

## Initialize the ```Client```

```
using RudderStack;

RudderAnalytics.Initialize(
    WRITE_KEY,
    new RudderConfig(dataPlaneUrl: DATA_PLANE_URL)
);
```

## Send Events

```
RudderAnalytics.Client.Track(
    "userId",
    "CTA Clicked",
    new Dictionary<string, object> { {"plan", "premium"}, }
);
```

## Contact Us

If you come across any issues while configuring or using this integration, please feel free to start a conversation on our [Slack](https://resources.rudderstack.com/join-rudderstack-slack) channel. We will be happy to help you.
