Reference Apps
================

These reference apps are provided as an example of using the Analytics.NET library. The folder is structure in the following way:

### `Sloth.Basic`
A sample app which imports the Segment SDK and exhibits the full functionality exposed without major configuration changes.

### `Sloth.Enterprise`
A sample app which imports the Segment SDK and exhibits how to configure Segment SDK to operate at high throughput, operate under proxy service, leverage batching and data compression.

This app demonstrates common configuration changes such as custom queue size, API hostname, and compression.

### `Sloth.Common`
A library of common functionality shared by both reference apps. This is not meant to function as a standalone app.

Usage
=====

Create a new Source (or use an existing one) from within the [Segment App](https://app.segment.com/). Take note of your [write key](https://segment.com/docs/connections/find-writekey/).

First, set an environment variable `writeKey` to your source's write key:

```
export writeKey=YOURWRITEKEY
```

Within either `Sloth.Basic` or `Sloth.Enterprise`, the easiest way to run the apps is to use Docker:

```
make docker_run
```

Alternatively, you can run the apps directly on your machine, using [.NET Core 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1):

```
make run
```

**Note:** out of the box, `Sloth.Enterprise` is configured to send data to `https://api.segment.dev`, which doesn't exist. This is purely done as a demonstration of the host configuration functionality. If you want to see the library working, you can point it to a working hostname or remove that configuration parameter.