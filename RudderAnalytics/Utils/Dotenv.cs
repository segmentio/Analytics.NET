using System;
using System.IO;

namespace RudderStack.Utils
{
    public static class DotEnv
    {
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File doesn't exists");
                return;
            }

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}