using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WordFinderApp.Models;

namespace WordFinderApp.BusinessLogic
{
    internal class SearchEngine
    {
        private readonly Regex _regex;
        private readonly List<string> _textLines;
        private List<PreparedText> _preparedText;
        public int AllMatchesCount { get; private set; }

        public SearchEngine(Regex regex, List<string> textLines) 
        {
            _regex = regex;
            _textLines = textLines;
            _preparedText = new List<PreparedText>();
            PrepareText();
        }

        private void PrepareText()
        {
            foreach(string line in _textLines)
            {
                MatchCollection matches = _regex.Matches(line);
                int matchesCount = matches.Count;
                if (matchesCount > 0)
                {
                    AllMatchesCount += matchesCount;
                    int lastMatchIndex = 0;
                    for(int i = 0; i < matchesCount; i++)
                    {
                        int howManyCharsToGet = matches[i].Index - lastMatchIndex;

                        // If howManyCharsToGet equals 0 then the match occurred at the
                        // beginning of the line, and we are adding the search word. 
                        // If not 0 we'll add a part of the text.
                        if (howManyCharsToGet > 0)
                        {
                            string partOfText = line.Substring(lastMatchIndex, howManyCharsToGet);
                            PreparedText preparedText = new PreparedText
                            {
                                IsNewLine = false,
                                PartOfText = partOfText,
                                IsSearchWord = false,
                            };
                            _preparedText.Add(preparedText);
                        }

                        int lineLength = line.Length;
                        string foundValue = matches[i].Value;
                        int foundValueLength = foundValue.Length;
                        lastMatchIndex += foundValueLength + howManyCharsToGet;
                        int nextLoop = i + 1;

                        PreparedText searchWord = new PreparedText
                        { 
                            IsNewLine = nextLoop == matchesCount && lastMatchIndex == lineLength,
                            PartOfText = foundValue,
                            IsSearchWord = true
                        };
                        _preparedText.Add(searchWord);

                        // Check if there is any rest of the line of text.
                        if(nextLoop == matchesCount && lastMatchIndex < lineLength)
                        {
                            string lastPartOfLine = line.Substring(lastMatchIndex, lineLength - lastMatchIndex);
                            PreparedText preparedText = new PreparedText
                            { 
                                IsNewLine = true,
                                PartOfText = lastPartOfLine,
                                IsSearchWord = false
                            };
                            _preparedText.Add(preparedText);
                        }
                    }
                }
                else
                {
                    // Add a line without matches to the List
                    PreparedText preparedText = new PreparedText
                    { 
                        IsNewLine = true,
                        PartOfText = line,
                        IsSearchWord = false
                    };
                    _preparedText.Add(preparedText);
                }
            }
        }

        public void ShowText()
        {
            Console.WriteLine("There were " + AllMatchesCount.ToString() + " matches.");
            Console.WriteLine();
            
            foreach(var l in _preparedText)
            {
                
                if (l.IsSearchWord)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.BackgroundColor = ConsoleColor.Yellow;
                }

                if(l.IsNewLine)
                {
                    Console.WriteLine(l.PartOfText);
                }
                else
                {
                    Console.Write(l.PartOfText);
                }

                if (l.IsSearchWord)
                    Console.ResetColor();
            }
        }
    }
}
