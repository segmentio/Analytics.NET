using System;
using System.Collections.Generic;
using RudderStack.Model;
using RudderStack.Stats;

namespace RudderStack
{
    public delegate void FailedHandler(BaseAction action, System.Exception e);
    public delegate void SucceededHandler(BaseAction action);

    public interface IRudderAnalyticsClient : IDisposable
    {
        Statistics Statistics { get; set; }
        string WriteKey { get; }
        RudderConfig Config { get; }

        event FailedHandler Failed;
        event SucceededHandler Succeeded;

        #region Identify

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you
        /// recognize and records visitor traits you can rudder by.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        ///
        /// <param name="traits">A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can rudder your users by any trait you record.
        /// Pass in values in key-value format. String key, then its value
        /// { String, Integer, Boolean, Double, or Date are acceptable types for a value. } </param>
        void Identify(string userId, IDictionary<string, object> traits);

        /// <summary>
        /// Identifying a visitor ties all of their actions to an ID you
        /// recognize and records visitor traits you can rudder by.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.</param>
        ///
        /// <param name="traits">A dictionary with keys like "email", "name", “subscriptionPlan” or
        /// "friendCount”. You can rudder your users by any trait you record.
        /// Pass in values in key-value format. String key, then its value
        /// { String, Integer, Boolean, Double, or Date are acceptable types for a value. } </param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Identify(string userId, IDictionary<string, object> traits, RudderOptions options);

        #endregion

        #region Group

        /// <summary>
        /// The `group` method lets you associate a user with a group. Be it a company,
        /// organization, account, project, team or whatever other crazy name you came up
        /// with for the same concept! It also lets you record custom traits about the
        /// group, like industry or number of employees.
        /// </summary>
        ///
        /// <param name="userId">The visitor's database identifier after they log in, or you know
        /// who they are. By explicitly grouping a user, you tie all of their actions to their group.</param>
        ///
        /// <param name="groupId">The group's database identifier after they log in, or you know
        /// who they are.</param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Group(string userId, string groupId, RudderOptions options);

        /// <summary>
        /// The `group` method lets you associate a user with a group. Be it a company,
        /// organization, account, project, team or whatever other crazy name you came up
        /// with for the same concept! It also lets you record custom traits about the
        /// group, like industry or number of employees.
        /// </summary>
        ///
        /// <param name="userId">The visitor's database identifier after they log in, or you know
        /// who they are. By explicitly grouping a user, you tie all of their actions to their group.</param>
        ///
        /// <param name="groupId">The group's database identifier after they log in, or you know
        /// who they are.</param>
        ///
        /// <param name="traits">A dictionary with group keys like "name", “subscriptionPlan”.
        /// You can rudder your users by any trait you record. Pass in values in key-value format.
        /// String key, then its value { String, Integer, Boolean, Double, or Date are acceptable types for a value. } </param>
        void Group(string userId, string groupId, IDictionary<string, object> traits);

        /// <summary>
        /// The `group` method lets you associate a user with a group. Be it a company,
        /// organization, account, project, team or whatever other crazy name you came up
        /// with for the same concept! It also lets you record custom traits about the
        /// group, like industry or number of employees.
        /// </summary>
        ///
        /// <param name="userId">The visitor's database identifier after they log in, or you know
        /// who they are. By explicitly grouping a user, you tie all of their actions to their group.</param>
        ///
        /// <param name="groupId">The group's database identifier after they log in, or you know
        /// who they are.</param>
        ///
        /// <param name="traits">A dictionary with group keys like "name", “subscriptionPlan”.
        /// You can rudder your users by any trait you record. Pass in values in key-value format.
        /// String key, then its value { String, Integer, Boolean, Double, or Date are acceptable types for a value. } </param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Group(string userId, string groupId, IDictionary<string, object> traits, RudderOptions options);

        #endregion

        #region MyRegion

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. </param>
        ///
        /// <param name="eventName">The event name you are tracking. It is recommended
        /// that it is in human readable form. For example, "Bought T-Shirt"
        /// or "Started an exercise"</param>
        void Track(string userId, string eventName);

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. </param>
        ///
        /// <param name="eventName">The event name you are tracking. It is recommended
        /// that it is in human readable form. For example, "Bought T-Shirt"
        /// or "Started an exercise"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the event
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        void Track(string userId, string eventName, IDictionary<string, object> properties);

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it
        /// so that you can analyze and rudder by those events later.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="eventName">The event name you are tracking. It is recommended
        /// that it is in human readable form. For example, "Bought T-Shirt"
        /// or "Started an exercise"</param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Track(string userId, string eventName, RudderOptions options);

        /// <summary>
        /// Whenever a user triggers an event on your site, you’ll want to track it
        /// so that you can analyze and rudder by those events later.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="eventName">The event name you are tracking. It is recommended
        /// that it is in human readable form. For example, "Bought T-Shirt"
        /// or "Started an exercise"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the event
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Track(string userId, string eventName, IDictionary<string, object> properties, RudderOptions options);

        #endregion

        #region Alias

        /// <summary>
        /// Aliases an anonymous user into an identified user.
        /// </summary>
        ///
        /// <param name="previousId">The anonymous user's id before they are logged in.</param>
        ///
        /// <param name="userId">the identified user's id after they're logged in.</param>
        void Alias(string previousId, string userId);

        /// <summary>
        /// Aliases an anonymous user into an identified user.
        /// </summary>
        ///
        /// <param name="previousId">The anonymous user's id before they are logged in.</param>
        ///
        /// <param name="userId">the identified user's id after they're logged in.</param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Alias(string previousId, string userId, RudderOptions options);

        #endregion

        #region Page

        /// <summary>
        /// The `page` method let your record whenever a user sees a webpage on
        /// your website, and attach a `name`, `category` or `properties` to the webpage load.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the webpage, like "Sign-up", "Login"</param>
        void Page(string userId, string name);

        /// <summary>
        /// The `page` method let your record whenever a user sees a webpage on
        /// your website, and attach a `name`, `category` or `properties` to the webpage load.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the webpage, like "Sign-up", "Login"</param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Page(string userId, string name, RudderOptions options);

        /// <summary>
        /// The `page` method let your record whenever a user sees a webpage on
        /// your website, and attach a `name`, `category` or `properties` to the webpage load.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the webpage, like "Sign-up", "Login"</param>
        ///
        /// <param name="category">The (optional) category of the webpage, like "Authentication", "Sports"</param>
        void Page(string userId, string name, string category);

        /// <summary>
        /// The `page` method let your record whenever a user sees a webpage on
        /// your website, and attach a `name`, `category` or `properties` to the webpage load.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the webpage, like "Sign-up", "Login"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the page
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        void Page(string userId, string name, IDictionary<string, object> properties);

        /// <summary>
        /// The `page` method let your record whenever a user sees a webpage on
        /// your website, and attach a `name`, `category` or `properties` to the webpage load.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the webpage, like "Sign-up", "Login"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the page
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Page(string userId, string name, IDictionary<string, object> properties, RudderOptions options);

        /// <summary>
        /// The `page` method let your record whenever a user sees a webpage on
        /// your website, and attach a `name`, `category` or `properties` to the webpage load.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the webpage, like "Sign-up", "Login"</param>
        ///
        /// <param name="category">The (optional) category of the mobile screen, like "Authentication", "Sports"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the page
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Page(string userId, string name, string category, IDictionary<string, object> properties, RudderOptions options);

        #endregion

        #region Screen

        /// <summary>
        /// The `screen` method let your record whenever a user sees a mobile screen on
        /// your mobile app, and attach a `name`, `category` or `properties` to the screen.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the mobile screen, like "Sign-up", "Login"</param>
        void Screen(string userId, string name);

        /// <summary>
        /// The `screen` method let your record whenever a user sees a mobile screen on
        /// your mobile app, and attach a `name`, `category` or `properties` to the screen.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the mobile screen, like "Sign-up", "Login"</param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Screen(string userId, string name, RudderOptions options);

        /// <summary>
        /// The `screen` method let your record whenever a user sees a mobile screen on
        /// your mobile app, and attach a `name`, `category` or `properties` to the screen.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the mobile screen, like "Sign-up", "Login"</param>
        ///
        /// <param name="category">The (optional) category of the mobile screen, like "Authentication", "Sports"</param>
        void Screen(string userId, string name, string category);

        /// <summary>
        /// The `screen` method let your record whenever a user sees a mobile screen on
        /// your mobile app, and attach a `name`, `category` or `properties` to the screen.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the mobile screen, like "Sign-up", "Login"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the screen
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        void Screen(string userId, string name, IDictionary<string, object> properties);

        /// <summary>
        /// The `screen` method let your record whenever a user sees a mobile screen on
        /// your mobile app, and attach a `name`, `category` or `properties` to the screen.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the mobile screen, like "Sign-up", "Login"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the screen
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Screen(string userId, string name, IDictionary<string, object> properties, RudderOptions options);

        /// <summary>
        /// The `screen` method let your record whenever a user sees a mobile screen on
        /// your mobile app, and attach a `name`, `category` or `properties` to the screen.
        /// </summary>
        ///
        /// <param name="userId">The visitor's identifier after they log in, or you know
        /// who they are. By
        /// explicitly identifying a user, you tie all of their actions to their identity.
        /// This makes it possible for you to run things like rudder-based email campaigns.</param>
        ///
        /// <param name="name">The name of the mobile screen, like "Sign-up", "Login"</param>
        ///
        /// <param name="category">The (optional) category of the mobile screen, like "Authentication", "Sports"</param>
        ///
        /// <param name="properties"> A dictionary with items that describe the screen
        /// in more detail. This argument is optional, but highly recommended —
        /// you’ll find these properties extremely useful later.</param>
        ///
        /// <param name="options">Options allowing you to set timestamp, anonymousId, target integrations,
        /// and the context of the message.</param>
        void Screen(string userId, string name, string category, IDictionary<string, object> properties, RudderOptions options);

        #endregion

        /// <summary>
        /// Blocks until all messages are flushed
        /// </summary>
        void Flush();
    }
}
