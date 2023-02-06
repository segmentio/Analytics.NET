using System;
using System.Collections.Generic;
using System.Linq;
using RudderStack;

namespace Sloth.Common
{
    public class Utils
    {
        public static void DoJourney()
        {
            var anonUserId = Guid.NewGuid().ToString();
            var user1Id = Guid.NewGuid().ToString();
            var user2Id = Guid.NewGuid().ToString();
            var accountId = Guid.NewGuid().ToString();

            AnonymousUserVisitsWebsite(anonUserId);
            //UserU1SignsUpForNewTrialAccount(anonUserId, user1Id, accountId);
            //UserU1SendInviteToAnotherUserU2(user1Id);
            UserU1SignsOutOfApp(user1Id);
            //UserU1SignsBackIntoApp(user1Id);
            //TrialsEndsAndUserU1RequestsAccountToBeDeleted(user1Id, user2Id, accountId);
        }

        public static Logger.LogHandler LoggerOnHandlers = (level, message, args) =>
        {
            if (args != null)
                message = args.Keys.Aggregate(message,
                    (current, key) => current + $" {"" + key}: {"" + args[key]},");

            Console.WriteLine($"[{level}] {message}");
        };

        private static void AnonymousUserVisitsWebsite(string anonUserId)
        {
            //identify page load
            RudderAnalytics.Client.Page(anonUserId, "Home");

            //identify anon user
            RudderAnalytics.Client.Identify(anonUserId, new Dictionary<string, object> { { "subscription", "inactive" }, });

            // track CTA click
            RudderAnalytics.Client.Track(anonUserId, "CTA Clicked", new Dictionary<string, object> { { "plan", "premium" }, });
        }
        private static void UserU1SignsUpForNewTrialAccount(string anonUserId, string user1Id, string accountId)
        {
            //page
            RudderAnalytics.Client.Page(anonUserId, "Sign Up", new Dictionary<string, object> { { "url", "https://wwww.example.com/sign-up" }, });

            //new account created
            RudderAnalytics.Client.Track(anonUserId, "Account Created", new Dictionary<string, object>
            {
                {"account_name", "Acme Inc"},
            });

            //create new user
            RudderAnalytics.Client.Track(user1Id, "Signed Up", new Dictionary<string, object>
            {
                {"type", "organic"},
                {"first_name", "Peter"},
                {"last_name", "Gibbons"},
                {"email", "pgibbons@initech.com"},
                {"phone", "410-555-9412"},
                {"username", "pgibbons"},
                {"title", "Mr"},
            });

            // alias anon id to new user
            RudderAnalytics.Client.Alias(anonUserId, user1Id);

            //add user to account (group)
            RudderAnalytics.Client.Group(user1Id, accountId, new Dictionary<string, object> { { "role", "Owner" }, });

            //confirm track call
            RudderAnalytics.Client.Track(user1Id, "Account Added User", new Dictionary<string, object>
            {
                {"role", "Owner"},
            });

            //start account trial
            RudderAnalytics.Client.Track(user1Id, "Trial Started", new Dictionary<string, object>
            {
                {"trial_start_date", DateTime.Now},
                {"trial_end_date", DateTime.Now.AddDays(7)},
                {"trial_plan_name", "premium"},
            });
        }

        private static void UserU1SendInviteToAnotherUserU2(string user1Id)
        {
            //page
            RudderAnalytics.Client.Page(user1Id, "Dashboard", new Dictionary<string, object>
            {
                {"url", "https://wwww.example.com/dashboard"},
            });

            //invite sent
            RudderAnalytics.Client.Track(user1Id, "Invite Sent", new Dictionary<string, object>
            {
                {"invitee_email", "janedoe@gmail.com"},
                {"invitee_first_name", "Jane"},
                {"invitee_last_name", "Doe"},
                {"invitee_role", "Admin"},
            });
        }

        private static void UserU1SignsOutOfApp(string user1Id)
        {
            //signed out
            RudderAnalytics.Client.Track(user1Id, "Signed Out", new Dictionary<string, object>
            {
                {"username", "pgibbons"},
            });
        }

        private static void UserU1SignsBackIntoApp(string user1Id)
        {
            //page
            RudderAnalytics.Client.Page(user1Id, "Dashboard", new Dictionary<string, object>
            {
                {"url", "https://www.example.com/dashboard"},
            });

            // signed in
            RudderAnalytics.Client.Track(user1Id, "Signed In", new Dictionary<string, object>
            {
                {"username", "pgibbons"},
            });
        }

        private static void TrialsEndsAndUserU1RequestsAccountToBeDeleted(string user1Id, string user2Id, string accountId)
        {
            //page
            RudderAnalytics.Client.Page(user1Id, "Dashboard", new Dictionary<string, object>
            {
                {"url", "https://wwww.example.com/account/settings"},
            });

            //trial ended
            RudderAnalytics.Client.Track(accountId, "Trial Ended", new Dictionary<string, object>
            {
                {"trial_start_date", DateTime.Now},
                {"trial_end_date", DateTime.Now.AddDays(7)},
                {"trial_plan_name", "premium"},
            });

            //track user requests account deletion on upgrade request
            RudderAnalytics.Client.Track(user1Id, "Account Delete Requested", new Dictionary<string, object>
            {
                {"account_id", accountId},
            });

            //remover User (U2) from account
            RudderAnalytics.Client.Track(user2Id, "Account Removed User");

            //remover User (U1) from account
            RudderAnalytics.Client.Track(user1Id, "Account Removed User");

            //delete Account
            RudderAnalytics.Client.Track(accountId, "Account Deleted", new Dictionary<string, object>
            {
                {"account_name", "Acme Inc"},
            });

            //sign out User (U1)
            RudderAnalytics.Client.Track(user1Id, "Signed Out", new Dictionary<string, object>
            {
                {"username", "pgibbons"},
            });
        }

        public static void PrintSummary()
        {
            Console.WriteLine($"Submitted: {RudderAnalytics.Client.Statistics.Submitted}");
            Console.WriteLine($"Failed: {RudderAnalytics.Client.Statistics.Failed}");
            Console.WriteLine($"Succeeded: {RudderAnalytics.Client.Statistics.Succeeded}");
        }
    }
}
