3.8.1 / 2022-07-07
==================
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/193): Bump Newtonsoft.Json from 12.0.3 to 13.0.1 in /Analytics 
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/192): Bump Newtonsoft.Json from 12.0.3 to 13.0.1 in /Test.E2E
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/187): Fix e2e test failing 
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/186): Changes made to e2e testing project so as to compile correctly in CircleCI 
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/184): Changed circle Ci config.yml 


3.8.0 / 2021-07-14
==================
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/181): Added catch on retry send to control other exception types
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/180): Added get functionality to properties of Config
* [Feature](https://github.com/segmentio/Analytics.NET/pull/178): Added .NET 5.0 Compatibility with corresponding CircleCI configuration and tests
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/175): Changed field accesibility to public for concrete implementations of base action
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/172): New circle ci configuration created
* [Fix](https://github.com/segmentio/Analytics.NET/pull/168): Changed how backo time is reset so when new action is sent is correctly reset
* [Feature](https://github.com/segmentio/Analytics.NET/pull/164): Added logging  and configuration for Max Retry Time around retries


3.7.1 / 2021-04-16
==================
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/163): Better flexibility and logging around retries


3.7.0 / 2021-03-26
==================
* [Improvement](https://github.com/segmentio/Analytics.NET/pull/162): Exception handling improvements 

3.6.0 / 2021-03-10
==================
 * [Improvement](https://github.com/segmentio/Analytics.NET/pull/159): Updating NuGet metadata 
 * [Improvement](https://github.com/segmentio/Analytics.NET/pull/158): Catch post async network exceptions and retry 
 * [Improvement](https://github.com/segmentio/Analytics.NET/pull/155): Adding support for dependency injection

3.5.0 / 2020-12-09
==================
 * [Improvement](https://github.com/segmentio/Analytics.NET/pull/151): Fixed behavior of confusing configuration parameter `Send`. It now defaults to `true` and means data will be sent to Segment.

3.4.2-beta / 2020-07-16
=======================
 * [Fix](https://github.com/segmentio/Analytics.NET/pull/143) Patched security vulnerabilities in dependencies

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
