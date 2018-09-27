using System;
using System.Collections.Generic;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace Segment.E2ETest
{
    class Program
    {
        public static int Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication(throwOnUnexpectedArg: true);

            CommandOption type = app.Option("-t | --type <type>",
                "<track|identify|page|group|alias>", CommandOptionType.SingleValue);
            CommandOption writeKey = app.Option("-w | --writeKey <writeKey>", 
                "WriteKey to send message", CommandOptionType.SingleValue);
            CommandOption userId = app.Option("-ui | --userId <userId>",
                "UserId in message options", CommandOptionType.SingleValue);
            CommandOption evt = app.Option("-e | --event <event>",
                "Event name in message options", CommandOptionType.SingleValue);
            CommandOption properties = app.Option("-p | --properties <properties>",
                "JSON object that contains properties of message", CommandOptionType.SingleValue);
            CommandOption name = app.Option("-n | --name <name>",
                "Name in message options", CommandOptionType.SingleValue);
            CommandOption traits = app.Option("-tr | --traits <traits>",
                "JSON object that presents traits", CommandOptionType.SingleValue);
            CommandOption groupId = app.Option("-gi | --groupId <groupId>",
                "GroupId in message options", CommandOptionType.SingleValue);
            CommandOption previousId = app.Option("-pi | --previousId <previousId>",
                "PreviousId in message options", CommandOptionType.SingleValue);

            app.HelpOption("-? | -h | --help");
            app.OnExecute(() =>
            {
                if (type.HasValue())
                {
                    if (!writeKey.HasValue())
                    {
                        Console.WriteLine("WriteKey is required.");
                        return 1;
                    }

                    Tests test = new Tests(writeKey.Value());

                    Logger.Handlers += Logger_Handlers;

                    switch (type.Value())
                    {
                        case "track":
                            test.Track(userId.Value(), evt.Value(), GetOptionAsDictionary(properties));
                            break;
                        case "identify":
                            test.Identify(userId.Value(), GetOptionAsDictionary(traits));
                            break;
                        case "page":
                            test.Page(userId.Value(), name.Value(), GetOptionAsDictionary(properties));
                            break;
                        case "group":
                            test.Group(userId.Value(), groupId.Value(), GetOptionAsDictionary(traits));
                            break;
                        case "alias":
                            test.Alias(previousId.Value(), groupId.Value());
                            break;
                        default:
                            app.ShowHelp();
                            return 1;
                    }

                    return 0;
                }
                return 1;
            });

            try
            {
                return app.Execute(args);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 2;
            }
        }

        private static IDictionary<string, object> GetOptionAsDictionary(CommandOption value)
        {
            try
            {
                if (!value.HasValue())
                    return new Dictionary<string, object>();

                return JsonConvert.DeserializeObject<Dictionary<string, object>>(value.Value());
            }
            catch (System.Exception)
            {
                throw new System.Exception(value.ValueName + " must be a JSON object.");
            }
        }

        private static void Logger_Handlers(Logger.Level level, string message, IDictionary<string, object> args)
        {
            Console.WriteLine(message);
        }
    }
}
