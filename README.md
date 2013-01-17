Analytics.NET
=============

Analytics.NET is a .NET / C# client for [Segment.io](https://segment.io). If you're using client-side javascript, check out [analytics.js](https://github.com/segmentio/analytics.js).

### .NET Analytics Made Simple

[Segment.io](https://segment.io) is the cleanest, simplest API for recording analytics data.

Setting up a new analytics solution can be a real pain. The APIs from each analytics provider are slightly different in odd ways, code gets messy, and developers waste a bunch of time fiddling with long-abandoned client libraries. We want to save you that pain and give you an clean, efficient, extensible analytics setup.

[Segment.io](https://segment.io) wraps all those APIs in one beautiful, simple API. Then we route your analytics data wherever you want, whether it's Google Analytics, Mixpanel, Customer io, Chartbeat, or any of our other integrations. After you set up Segment.io you can swap or add analytics providers at any time with a single click. You won't need to touch code or push to production. You'll save valuable development time so that you can focus on what really matters: your product.

```csharp
Analytics.Initialize("YOUR_API_SECRET");
Analytics.Track("user@gmail.com", "Played a Song");
```

and turn on integrations with just one click at [Segment.io](https://segment.io).

![](http://i.imgur.com/YnBWI.png)

More on integrations [here](#integrations).

### High Performance

This client uses an internal queue to efficiently send your events in aggregate, rather than making an HTTP
request every time. This means that it is safe to use in your high scale web server controllers, or in your backend services
without worrying that it will make too many HTTP requests and slow down the program. You no longer need to use a message queue to have analytics.

[Feedback is very welcome!](mailto:friends@segment.io)

## Quick-start

If you haven't yet, get an API secret [here](https://segment.io).

Use NuGet to install:
```bash
Install-Package Analytics
```

or clone the directory and build the project yourself. 

#### Initialize the client

You can create separate Analytics.NET clients, but the easiest and recommended way is to use the static Analytics singleton client. 

```csharp
Analytics.Initialize("YOUR_API_SECRET");
```

#### Identify a User

Identifying a user ties all of their actions to an ID you recognize and records user traits you can segment by.

```csharp
string sessionId = "auto_generated_session_id";
string userId = "user@gmail.com";
Traits traits = new Traits() {
    { "Subscription Plan", "Free" },
    { "Friends", 30 } 
};

Segmentio.Client.Identify(sessionId, userId, traits);
```

You must provide a *sessionId* or a *userId*. Send both when possible.

**sessionId** (string) is a unique id associated with an anonymous user **before** they are logged in. Even if the user
is logged in, you can still send us the **sessionId** or you can use null.

**userId** (string) is the user's id **after** they are logged in. It's the same id as which you would 
recognize a signed-in user in your system. Note: you must provide either a `sessionId` or a `userId`.

**traits** (Segmentio.Model.Traits) is a dictionary with keys like `subscriptionPlan` or `age`. You only need to record a trait once, no need to send it again.

#### Track an Action

Whenever a user triggers an event on your site, you’ll want to track it.

```csharp
Segmentio.Client.Track(sessionId, userId, "Played a Song", new Properties() {
    { "Artist", "The Beatles" },
    { "Song", "Eleanor Rigby" } 
});
```

**sessionId** (string) is a unique id associated with each user's session. Most web frameworks provide a session id 
you can use here.  If you don't have one, you can use null. 

**userId** (string) is the user's id **after** they are logged in. It's the same id as which you would recognize a 
signed-in user in your system. Note: you must provide either a `sessionId` or a `userId`.

**event** (string) is a human readable description like "Played a Song", "Printed a Report" or "Updated Status". 
You’ll be able to segment by when and how many times each event was triggered.

**properties** (Segmentio.Model.Properties) is a dictionary with items that describe the event in more detail. 
This argument is optional, but highly recommended—you’ll find these properties extremely useful later.
More on accepted value types [here](#allowed-traitproperty-values).

That's it, just two functions!

## Integrations

There are two main modes of analytics integration: client-side and server-side. You can use just one, or both.

#### Client-side vs. Server-side

* **Client-side analytics** - (via [analytics.js](https://github.com/segmentio/analytics.js)) works by loading in other integrations
in the browser.

* **Server-side analytics** - (via [analytics-node](https://github.com/segmentio/analytics-node) and other server-side libraries) works
by sending the analytics request to [Segment.io](https://segment.io). Our servers then route the message to your desired integrations.

Some analytics services have REST APIs while others only support client-side integrations.

You can learn which integrations are supported server-side vs. client-side on your [project's integrations]((https://segment.io) page.

### Advanced

### Batching Behavior

By default, the client will flush:

+ the first time it gets a message
+ every message (control with `flushAt`)
+ if 10 seconds has passed since the last flush (control with `flushAfter`)

#### Enable Batching

Batching allows you to not send an HTTP request every time you submit a message.
 In high scale environments, it's a good idea to set `flushAt` to about 25, meaning the client will flush every 25 messages.

```csharp
Analytics.Initialize("YOUR_API_SECRET", new Options().SetFlushAt(25));
````

#### Flush Whenever You Want

At the end of your program, you may want to flush to make sure there's nothing left in the queue.

```csharp
Analytics.Flush();
```

#### Why Batch?

This client is built to support high performance environments. That means it is safe to use Analytics.NET in a web server that is serving hundreds of requests per second.

**How does the batching work?**

Every action **does not** result in an HTTP request, but is instead queued in memory. Messages are flushed in batch in the background, which allows for much faster operation.

**What happens if there are just too many messages?**

If the client detects that it can't flush faster than it's receiving messages, it'll simply stop accepting messages. This means your program won't crash because of a backed up analytics queue.

### Understanding the Client Options

If you hate defaults, than you'll love how configurable the Analytics.NET is.
Check out these gizmos:

```csharp
Analytics.Initialize("YOUR_API_SECRET", new Options()
                                        .SetFlushAt(50)
                                        .SetFlushAfter(TimeSpan.FromSeconds(10))
                                        .SetMaxQueueSize(10000));
```

* **flushAt** (int) - Flush after this many messages are in the queue.
* **flushAfter** (int) - Flush after this much time has passed since the last flush.
* **maxQueueSize** (int) - Stop accepting messages into the queue after this many messages are backlogged in the queue.

### Multiple Clients

Different parts of your app may require different types of batching. In that case, you can initialize different `Analytics.NET` client instances. `Analytics.Initialize` becomes the `Client`'s constructor.

```csharp
Client client = new Client("testsecret", new Options()
                                    .SetFlushAt(50)
                                    .SetFlushAfter(TimeSpan.FromSeconds(10))
                                    .SetMaxQueueSize(10000));
client.Track(..);
```
#### Troubleshooting

Use events to receive successful or failed events.
```csharp
Analytics.Initialize("YOUR_API_SECRET");

Analytics.Client.Succeeded += Client_Succeeded;
Analytics.Client.Failed += Client_Failed;

void Client_Failed(BaseAction action, System.Exception e)
{
    Console.WriteLine(String.Format("Action {0} failed : {1}", action.GetAction(), e.Message));
}

void Client_Succeeded(BaseAction action)
{
    Console.WriteLine(String.Format("Action {0} succeeded.", action.GetAction()));
}
```

#### SessionId and UserId
**Web Framework**
```csharp
// user is not logged in, we only have a session ID
Analytics.Client.Identify(Session.SessionID, null, traits);
...
// user logs in, so we want to tie all their previous actions to their new identity
Analytics.Client.Identify(Session.SessionID, User.Email, traits);
```

**Desktop App, Or Other**
```csharp
// we don't have a session ID here, just provide a userId
Analytics.Client.Identify(null, User.Email, traits);
```
#### Allowed Trait/Property Values

**Allowed**:      string, int, double, bool, DateTime

**NOT Allowed**: arrays, lists, complex objects, exceptions, etc ...

```csharp
Analytics.Client.Track(sessionId, userId, "Played a Song", new Properties() {

    // Allowed
    { "Artist", "The Beatles" },                     // strings allowed
    { "Plays", 10 },                                 // ints allowed
    { "Duration", 126.3 },                           // double allowed
    { "DRM", false },                                // bool allowed
    { "Started", DateTime.Now },                     // DateTime allowed
    
    // NOT Allowed
    { "Comments", new List<string>() { "A", "B" } }, // Lists / Arrays NOT ALLOWED, will be removed
    { "Exception", new Exception("TROLOLOL") },      // Complex Objects NOT ALLOWED, will be removed
});

```

#### Importing Historical Data

You can import previous data by using the Identify / Track override that accepts a timestamp on each Identify / Track. If you are calling Identify and Track as things happen in real time, we recommend that you leave the timestamp out and let our servers timestamp your requests.



## Testing

Go to `BasicTests.cs` in Visual Studio, right click the file and click "Run Tests".

## License

```
WWWWWW||WWWWWW
 W W W||W W W
      ||
    ( OO )__________
     /  |           \
    /o o|    MIT     \
    \___/||_||__||_|| *
         || ||  || ||
        _||_|| _||_||
       (__|__|(__|__|
```

(The MIT License)

Copyright (c) 2012 Segment.io Inc. <friends@segment.io>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
