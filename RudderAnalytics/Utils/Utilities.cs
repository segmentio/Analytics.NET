using System;
using System.Collections.Generic;
using System.Text;

namespace RudderStack.Utils
{
    public static class Utilities
    {
        public static string getParentPath(int steps, string currentPath)
        {
            if (currentPath != null)
            {
                String parentPath = System.IO.Directory.GetParent(currentPath).ToString();
                for (int i = 1; i < steps; i++)
                {
                    parentPath = System.IO.Directory.GetParent(parentPath).ToString();
                }
                return parentPath;
            }
            return null;
        }
    }
}
