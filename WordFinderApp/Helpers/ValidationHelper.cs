using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordFinderApp.Helpers
{
    internal class ValidationHelper
    {
        public static bool IsTextFile(string path)
        {
            return path != null && path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
        }

        public static int? CheckUserIntInput(string? input)
        {
            if (int.TryParse(input, out int inputInt))
            {
                return inputInt;
            }
            else
            {
                return null;
            }
        }

        public static bool DirectoryAllowed(DirectoryInfo directoryInfo)
        {
            DirectoryInfo? allowedDirectory = null;
            try
            {
                allowedDirectory = directoryInfo.EnumerateDirectories().FirstOrDefault();
            }
            catch { }
            return allowedDirectory != null;
        }
    }
}
