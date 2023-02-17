using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordFinderApp.Models
{
    internal class PreparedText
    {
        public bool IsNewLine { get; set; }
        public string PartOfText { get; set; }
        public bool IsSearchWord { get; set; }
    }
}
