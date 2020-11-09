# What is RudderStack?

**Short answer:**
Rudder is an open-source Segment alternative written in Go, built for the enterprise. .

**Long answer:**
Rudder is a platform for collecting, storing and routing customer event data to dozens of tools. Rudder is open-source, can run in your cloud environment (AWS, GCP, Azure or even your data-centre) and provides a powerful transformation framework to process your event data on the fly.

Released under MIT License

### Latest Version

`1.0.0`

## Getting Started with Python SDK

Install `rudder-sdk-python` using `pip`
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
    new Dictionary<string, object> {# {"plan", "premium"}, }
);
```

## Contact Us
If you come across any issues while configuring or using RudderStack, please feel free to [contact us](https://rudderstack.com/contact/) or start a conversation on our [Discord](https://discordapp.com/invite/xNEdEGw) channel. We will be happy to help you.
