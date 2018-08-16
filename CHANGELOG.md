
3.3.1-alpha / 2018-07-09
========================

  * [Fix](https://github.com/segmentio/Analytics.NET/commit/e4e28dbffca4f5407eff7595a284457a2d3fab4f): Fix error handling behaviour.
  * [Fix](https://github.com/segmentio/Analytics.NET/pull/83): Fix issues with sending requests that are too large.
  * [Fix](https://github.com/segmentio/Analytics.NET/pull/85): Fix possible deadlock.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/86): Allow setting maximum batch size in configuration.

3.3.0-alpha / 2018-05-01
========================

Note: 3.3.0-alpha includes a bug which can cause long lived clients to stop sending data to the Segment API. A [fix](https://github.com/segmentio/Analytics.NET/pull/83) is available in the latest release (3.3.1-alpha) and it is recommended you upgrade to the latest version.

  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/80): Allow sending destination specific options.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/75): gzip network requests.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/74): Send library information in user-agent.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/73): Better error handling and retry behaviour.
  * [Feature](https://github.com/segmentio/Analytics.NET/pull/67): Target .NET Framework 4.5.1.
  * [Feature](https://github.com/segmentio/Analytics.NET/pull/69): Target Xamarin (iOS, Android, UWP).

3.2.0-alpha / 2017-11-08
========================

  * [Feature](https://github.com/segmentio/Analytics.NET/pull/64): Allow custom host to be set.
  * [Feature](https://github.com/segmentio/Analytics.NET/pull/66): Support .NET standard 2.0.

3.1.0-alpha / 2017-10-04
========================

  * [Feature](https://github.com/segmentio/Analytics.NET/pull/55): Multi platform support.

3.0.0 / 2017-08-07
==================

  * [Feature](https://github.com/segmentio/Analytics.NET/pull/51): Target .NET standard.

2.0.3 / 2017-01-02
==================

  * [Fix](https://github.com/segmentio/Analytics.NET/pull/34): Use fully qualified assembly name for JSON.NET dependency.
  * [Fix](https://github.com/segmentio/Analytics.NET/pull/48): Fix overflow in stats.
  * [Fix](https://github.com/segmentio/Analytics.NET/pull/35): Documentation fixes.
  * [Fix](https://github.com/segmentio/Analytics.NET/pull/33): Fix exception message when `userId` is omitted in Alias.
  * [Fix](https://github.com/segmentio/Analytics.NET/pull/43): Consolidate error phrasing and capitalization.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/47): Accept `IDictionary` instead of `Dict` or custom types.

2.0.2 / 2015-09-30
==================

  * [Fix](https://github.com/segmentio/Analytics.NET/pull/27): Fix nuspec links in documentation.
  * [Fix](https://github.com/segmentio/Analytics.NET/pull/30): Fix timestamp formatting.
  * [Fix](https://github.com/segmentio/Analytics.NET/pull/26): Allow identify events to be sent without anonymousId.

2.0.1 / 2015-07-01
==================

  * [Fix](https://github.com/segmentio/Analytics.NET/pull/23): All events to be sent with only anonymousId
