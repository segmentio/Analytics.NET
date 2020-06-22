
3.4.1-alpha / 2020-05-21
========================
  * [Fix](https://github.com/segmentio/Analytics.NET/pull/136): Critical bug fixes

3.4.0-alpha / 2020-05-20
========================

Note: 3.4.0-alpha contains a change to configuration parameter names that may trigger obsolete messages. It is recommended you migrate to the new parameters defined in [#118](https://github.com/segmentio/Analytics.NET/pull/118).

  * [Fix](https://github.com/segmentio/Analytics.NET/pull/103): Additional fixes with sending requests that are too large.
  * [Fix](https://github.com/segmentio/Analytics.NET/pull/109): Fix first event is never being batched.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/116): Expose ability to FlushAsync in a non-blocking manner.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/118): Update configuration parameters to unified names with other Segment libraries.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/126): Unified project structure.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/132): Project can now be built on any operating system supporting .NET Core SDK.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/130): Supports Dependency Injection for better support within .NET Core SDK.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/120): Maximum batch size of 500KB and message size of 32KB is now enforced.
  * [Improvement](https://github.com/segmentio/Analytics.NET/pull/119): Added capability for multi-threading using .NET Framework 3.5.
  * [Feature](https://github.com/segmentio/Analytics.NET/pull/109): Allow user defined interval for flushing events.
  * [Feature](https://github.com/segmentio/Analytics.NET/pull/115): Network requests now have exponential back-off and jitter.
  * [Feature](https://github.com/segmentio/Analytics.NET/pull/118): Allow user defined user agent header.
  * [Feature](https://github.com/segmentio/Analytics.NET/pull/117): Enable configurable concurrency.
  * [Feature](https://github.com/segmentio/Analytics.NET/pull/134): Added send configuration parameter to toggle making HTTP requests on event send.

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
