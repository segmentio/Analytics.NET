//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Segment
{
    using Segment.Model;

    /// <summary>
    /// Analytics Logging.
    /// </summary>
    public class Logger
    {
        #region Events

        /// <summary>
        /// A logging event handler.   
        /// </summary>
        /// <param name="level">The <see cref="Segment.Logger.Level"/> of the log event (debug, info, warn, error).</param>
        /// <param name="message">The log message.</param>
        /// <param name="args">Optional arguments for the message.</param>
        public delegate void LogHandler(Level level, string message, Dict args);

        public static event LogHandler Handlers;

        #endregion
        
        /// <summary>
        /// The logging level of the message.
        /// </summary>
        public enum Level
        {
            DEBUG,
            INFO,
            WARN,
            ERROR
        }

        internal static void Debug(string message)
        {
            Log(Level.DEBUG, message, null);
        }

        internal static void Debug(string message, Dict args)
        {
            Log(Level.DEBUG, message, args);
        }

        internal static void Info(string message)
        {
            Log(Level.INFO, message, null);
        }

        internal static void Info(string message, Dict args)
        {
            Log(Level.INFO, message, args);
        }

        internal static void Warn(string message)
        {
            Log(Level.WARN, message, null);
        }

        internal static void Warn(string message, Dict args)
        {
            Log(Level.WARN, message, args);
        }

        internal static void Error(string message)
        {
            Log(Level.ERROR, message, null);
        }

        internal static void Error(string message, Dict args)
        {
            Log(Level.ERROR, message, args);
        }

        private static void Log(Level level, string message, Dict args)
        {
            if (Handlers != null)
            {
                Handlers(level, message, args);
            }
        }
    }
}
