using System.Collections.Generic;

namespace ClaraMundi
{
    public class Party
    {
        public bool established;
        public string Leader;
        public List<string> Members = new();
        public List<string> Invitations = new();
        public List<string> Requests = new();
    }
}