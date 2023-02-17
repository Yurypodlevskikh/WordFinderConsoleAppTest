using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using WordFinderApp.BusinessLogic;
using WordFinderApp.Helpers;

namespace WordFinderApp
{
    internal class WordFinder
    {
        private static string? filePath;
        public static void WordFinderFunctionality(string? argsPath)
        {
            filePath = argsPath;
            StartPage();
        }

        static void StartPage()
        {
            while (true)
            {
                Console.WriteLine("Choose which text file (.txt) you would like to search?\n");
                Console.WriteLine("1. Default file.\n");
                Console.WriteLine("2. Open a file using a pasted path.\n");
                Console.WriteLine("3. Open a file from a directory on your computer.");

                Console.WriteLine();
                Console.Write(MessageCodeToString(4));
                int? userChoise = ValidationHelper.CheckUserIntInput(Console.ReadKey().KeyChar.ToString());

                if (userChoise != null)
                {
                    if (userChoise == 1)
                    {
                        SearchInDefaultFile();
                    }
                    else if(userChoise == 2)
                    {
                        SearchInFileFromPath();
                    }
                    else if (userChoise == 3)
                    {
                        SearchInFileFromDirectory();
                    }
                    else
                    {
                        Console.WriteLine("\n" + MessageCodeToString(6));
                        WouldYouLikeToContinue(MessageCodeToString(2));
                    }
                }
                else
                {
                    Console.WriteLine("\n" + MessageCodeToString(1));
                    WouldYouLikeToContinue(MessageCodeToString(2));
                }

                Console.Clear();
            }
        }

        static void SearchInDefaultFile()
        {
            if (string.IsNullOrEmpty(filePath) && !ValidationHelper.IsTextFile(filePath))
                filePath = "text.txt";

            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                Console.Clear();
                Console.WriteLine("The file does not exist.");
                WouldYouLikeToContinue(MessageCodeToString(2));
            }
            else 
            {
                WorkingWithTheTextFile();
            }
        }

        static void SearchInFileFromPath()
        {
            bool fileFromPathContinue = true;
            while(fileFromPathContinue)
            {
                Console.Clear();
                Console.Write("Paste the path: ");
                string? userPath = Console.ReadLine();

                if (string.IsNullOrEmpty(userPath))
                {
                    Console.WriteLine();
                    Console.WriteLine(MessageCodeToString(1));
                    Console.WriteLine();
                    DrawLine();
                    if (WouldYouLikeToContinue(MessageCodeToString(3)) == null)
                        fileFromPathContinue = false;
                }
                else
                {
                    if (userPath.StartsWith("\"") && userPath.EndsWith("\""))
                        userPath = userPath.Replace("\"", "");

                    DriveInfo[] drives = DriveInfo.GetDrives();
                    bool thePathIsValid = false;
                    foreach (DriveInfo drive in drives)
                    {
                        if (userPath.StartsWith(drive.Name))
                        {
                            thePathIsValid = true;
                            break;
                        }
                    }

                    if (thePathIsValid == false)
                    {
                        Console.WriteLine();
                        Console.WriteLine("The path to the text file is not valid.");
                        Console.WriteLine("Set a path to an existing disc.");
                        Console.WriteLine();
                        DrawLine();
                        if (WouldYouLikeToContinue(MessageCodeToString(3)) == null)
                            fileFromPathContinue = false;
                    }
                    else
                    {
                        if(!ValidationHelper.IsTextFile(userPath))
                        {
                            Console.WriteLine("This is not a text file. Only text files are valid.");
                            Console.WriteLine();
                            DrawLine();
                            if (WouldYouLikeToContinue(MessageCodeToString(3)) == null)
                                fileFromPathContinue = false;
                        }
                        else
                        {
                            filePath = userPath;
                            FileInfo fileInfo = new FileInfo(filePath);
                            if (!fileInfo.Exists)
                            {
                                Console.Clear();
                                Console.WriteLine("The file does not exist.");
                                WouldYouLikeToContinue(MessageCodeToString(2));
                            }
                            else
                            {
                                WorkingWithTheTextFile();
                            }
                        }
                        
                    }
                }
            }
        }

        static void SearchInFileFromDirectory()
        {
            while(true)
            {
                Console.Clear();

                List<DirectoryInfo> allowedDrives = new List<DirectoryInfo>();
                DriveInfo[] drives = DriveInfo.GetDrives();

                if(drives.Length > 0)
                {
                    foreach (DriveInfo drive in drives)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(drive.Name);
                        if (ValidationHelper.DirectoryAllowed(directoryInfo))
                            allowedDrives.Add(directoryInfo);
                    }

                    int? drivesCount = allowedDrives?.Count();
                    DirectoryInfo selectedDrive = null;
                    if (drivesCount != null && drivesCount > 0)
                    {
                        if (drivesCount == 1)
                        {
                            selectedDrive = allowedDrives.First();
                        }
                        else if (drivesCount > 1)
                        {
                            for (int i = 0; i < drivesCount; i++)
                            {
                                Console.WriteLine($"{i + 1}. {allowedDrives[i].Name}");
                            }

                            Console.WriteLine("\nChoose the drive where the necessary text file is saved: ");
                            int? userChoise = ValidationHelper.CheckUserIntInput(Console.ReadLine());

                            if (userChoise != null && userChoise <= drivesCount)
                            {
                                selectedDrive = allowedDrives[(int)userChoise - 1];
                            }
                            else
                            {
                                Console.WriteLine("\n" + MessageCodeToString(1) + "\n");
                                WouldYouLikeToContinue(MessageCodeToString(2));
                            }
                        }

                        if(selectedDrive != null)
                        {
                            ShowDirectoriesAndFiles(selectedDrive);
                        }
                        else
                        {
                            Console.WriteLine("There are no selected drives.");
                            WouldYouLikeToContinue(MessageCodeToString(2));
                        }
                    }
                    else
                    {
                        Console.WriteLine("There are no drives to which you have access.");
                        WouldYouLikeToContinue(MessageCodeToString(2));
                    }
                }
                else
                {
                    Console.WriteLine("There are problems with your drives.");
                    WouldYouLikeToContinue(MessageCodeToString(2));
                }
            }
        }

        static void WorkingWithTheTextFile()
        {
            bool workingWithFile = true;
            while (workingWithFile)
            {
                Console.Clear();

                string fileName = Path.GetFileNameWithoutExtension(filePath)!;
                Console.WriteLine($"The name of the default file is \"{fileName}\"");
                Console.WriteLine();

                Console.WriteLine("Select search type.\n");
                Console.WriteLine("1. Common search: all matches.\n");
                Console.WriteLine("2. Exact search: exactly this world.");

                Console.WriteLine();
                Console.Write(MessageCodeToString(4));
                int? userChoise = ValidationHelper.CheckUserIntInput(Console.ReadKey().KeyChar.ToString());

                if (userChoise != null)
                {
                    Regex regex = null;
                    string? regExString = ReadTextHelper.GetSearchRegExpression((int)userChoise, fileName);
                    if (regExString != null)
                    {
                        regex = new Regex(regExString, RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("\n" + MessageCodeToString(6));
                        WouldYouLikeToContinue(MessageCodeToString(2));
                    }

                    if (regex != null)
                    {
                        List<string> textLines = ReadTextHelper.ReadTextFromFile(filePath);

                        SearchEngine searchEngine = new SearchEngine(regex, textLines);

                        Console.Clear();
                        Console.WriteLine($"Word \"{fileName}\" occures in the text {searchEngine.AllMatchesCount} times.");
                        Console.WriteLine();

                        DrawLine();
                        if (WouldYouLikeToContinue(MessageCodeToString(5)) == true)
                        {
                            Console.Clear();

                            searchEngine.ShowText();

                            Console.WriteLine();
                            DrawLine();
                            if (WouldYouLikeToContinue(MessageCodeToString(3)) == null)
                                workingWithFile = false;
                        }
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("\n" + MessageCodeToString(1));
                    WouldYouLikeToContinue(MessageCodeToString(2));
                }
            }
        }

        static void ShowDirectoriesAndFiles(DirectoryInfo directory)
        {
            bool openDirectories = true;
            while (openDirectories)
            {
                Console.Clear();

                List<DirectoryInfo> allowedDirs = new List<DirectoryInfo>();
                FileInfo[] files = null;
                string exception = "";

                try
                {
                    DirectoryInfo[] dirs = directory.GetDirectories();
                    foreach (var dir in dirs)
                    {
                        if (ValidationHelper.DirectoryAllowed(dir))
                            allowedDirs.Add(dir);
                    }

                    files = directory.GetFiles("*.txt");
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                }

                List<string> subdirectoryFiles = new List<string>();
                int listNumbering = 1;

                if (allowedDirs.Count > 0)
                {
                    DrawLine();
                    Console.WriteLine("Subdirectories");
                    DrawLine();

                    foreach (DirectoryInfo dir in allowedDirs)
                    {
                        subdirectoryFiles.Add(dir.FullName);
                        Console.WriteLine(listNumbering + ". " + dir.Name);
                        listNumbering++;
                    }

                    Console.WriteLine();
                }

                if (files != null && files.Length > 0)
                {
                    DrawLine();
                    Console.WriteLine("Files");
                    DrawLine();
                    foreach (FileInfo file in files)
                    {
                        subdirectoryFiles.Add(file.FullName);
                        Console.WriteLine(listNumbering + ". " + file.Name);
                        listNumbering++;
                    }
                }

                if (subdirectoryFiles.Count() > 0)
                {
                    Console.WriteLine();
                    Console.Write(MessageCodeToString(4));
                    int? userChoise = ValidationHelper.CheckUserIntInput(Console.ReadLine());

                    if (userChoise != null && userChoise <= subdirectoryFiles.Count())
                    {
                        Console.WriteLine();
                        directory = new DirectoryInfo(subdirectoryFiles[(int)userChoise - 1]);

                        // If the attribute is not a Directory, then a text file is selected.
                        if(!directory.Attributes.HasFlag(FileAttributes.Directory))
                        {
                            // Read and search in the text.
                            filePath = directory.FullName;
                            WorkingWithTheTextFile();
                        }
                    }
                    else
                    {
                        Console.WriteLine(MessageCodeToString(1));
                        
                        if (WouldYouLikeToContinue(MessageCodeToString(3)) == null)
                            openDirectories = false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(exception))
                        exception = "There are no subdirectories or files.";

                    Console.WriteLine(exception);
                    if (WouldYouLikeToContinue(MessageCodeToString(2)) == true)
                        openDirectories = false;
                }
            }
        }

        static bool? WouldYouLikeToContinue(string message)
        {
            bool? toBeContinued = null;
            bool wrongChar = true;
            while(wrongChar)
            {
                Console.WriteLine(message);
                char userAnswer = Char.ToUpper(Console.ReadKey().KeyChar);
                switch (userAnswer)
                {
                    case 'Y':
                        wrongChar = false;
                        toBeContinued = true;
                        break;
                    case 'R':
                        wrongChar = false;
                        break;
                    case 'N':
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("To answer, please press corresponding key.");
                        break;
                }
            }
            return toBeContinued;
        }

        static void DrawLine()
        {
            Console.WriteLine("* " + new string('-', 25) + " *");
        }

        static string MessageCodeToString(int errorCode)
        {
            switch(errorCode)
            {
                case 1:
                    return "Invalid input.";
                case 2:
                    return "Would you like to try again \"Y\" or close the app \"N\"?";
                case 3:
                    return "Would you like to try again \"Y\", start over \"R\" or close the app \"N\"?";
                case 4:
                    return "Write the corresponding number: ";
                case 5:
                    return "Would you like to read the whole text \"Y\", start over \"R\" or close the app \"N\"?";
                case 6:
                    return "The chosen number is out of range.";
                default:
                    return "Something went wrong.";
            }
        }
    }
}
