using System.Collections.Generic;

namespace ClaraMundi
{
    public class Party
    {
        public string leader;
        public List<string> invited = new();
        public List<string> members = new();
        public List<string> requests = new();
    }
}