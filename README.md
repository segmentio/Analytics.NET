Segmentio.NET
=============

[Segment.io](https://segment.io) is a segmentation-focused analytics platform. If you haven't yet,
register for a project [here](https://segment.io).

This is an official C#/.NET client that wraps the [Segment.io REST API](https://segment.io/docs) .

You can use this driver to identify your users and track their events into your Segment.io project.

## Design

This client uses batching to efficiently record your events in aggregate, rather than making an HTTP
request every time. This means that it is safe to use in your web server controllers, or in back-end services
without worrying that it will make too many HTTP requests and slow down the system.

Check out the source to see how the batching, and async HTTP requests are handled. Feedback is very welcome!

## How to Use

#### Install
For now, build the binary and include it with your .NET project. 

#### Initialize the client

You can create separate Segmentio clients, but the easiest and recommended way is to use the static Segmentio singleton client. 

```csharp
string apiKey = isProduction ? PRODUCTION_API_KEY : DEVELOPMENT_API_KEY;
Segmentio.Initialize(apiKey);
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

**sessionId** (string) is a unique id associated with each user's session. Most web frameworks provide a session id 
you can use here.  If you don't have one, you can use null. 

**userId** (string) is usually an email, but any unique ID will work. This is how you recognize a signed-in user
in your system. Note: it can be null if the user is not logged in, but then a sessionId must be provided. By explicitly identifying a user, you tie all of
their actions to their identity. This makes it possible for you to run things like segment-based email campaigns. Either this or the sessionId must be supplied.
More on that [here](#sessionid-and-userid).

**traits** (Segmentio.Model.Traits) is a dictionary with keys like “Subscription Plan” or “Favorite Genre”. You can segment your 
users by any trait you record. Once you record a trait, no need to send it again, so the traits argument is optional.

#### Track an Action

Whenever a user triggers an event on your site, you’ll want to track it so that you can analyze and segment by those events later.

```csharp
Segmentio.Client.Track(sessionId, userId, "Played a Song", new Properties() {
    { "Artist", "The Beatles" },
    { "Song", "Eleanor Rigby" } 
});

```

**sessionId** (string) is a unique id associated with each user's session. Most web frameworks provide a session id 
you can use here.  If you don't have one, you can use null. 

**userId** (string) is usually an email, but any unique ID will work. This is how you recognize a signed-in user
in your system. Note: it can be null if the user is not logged in, but then a sessionId must be provided. By explicitly identifying a user, you tie all of
their actions to their identity. This makes it possible for you to run things like segment-based email campaigns. Either this or the sessionId must be supplied.
More on that [here](#sessionid-and-userid).

**event** (string) is a human readable description like "Played a Song", "Printed a Report" or "Updated Status". You’ll be able to segment by when and how many times each event was triggered.

**properties** (Segmentio.Model.Properties) is a dictionary with items that describe the event in more detail. This argument is optional, but highly recommended—you’ll find these properties extremely useful later.

### Advanced

#### Troubleshooting

Use events to receive successful or failed events.
```csharp
Segmentio.Initialize(API_KEY);

Segmentio.Client.Succeeded += Client_Succeeded;
Segmentio.Client.Failed += Client_Failed;

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
Segmentio.Client.Identify(Session.SessionID, null, traits);
...
// user logs in, so we want to tie all their previous actions to their new identity
Segmentio.Client.Identify(Session.SessionID, User.Email, traits);
```

**Desktop App, Or Other**
```csharp
// we don't have a session ID here, just provide a userId
Segmentio.Client.Identify(null, User.Email, traits);
```

#### Importing Historical Data

You can import previous data by using the Identify / Track override that accepts a timestamp on each Identify / Track. If you are calling Identify and Track as things happen in real time, we recommend that you leave the timestamp out and let our servers timestamp your requests.


#### License

(The MIT License)

Copyright (c) 2012 Segment.io Inc. <friends@segment.io>

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the 'Software'), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.