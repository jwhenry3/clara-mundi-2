using System.Collections.Generic;

namespace ClaraMundi
{
    public class Party
    {
        public bool established;
        public string LeaderId;
        public readonly List<string> MemberIds = new();
        public List<string> InvitedIds = new();
        public List<string> RequestedjoinerIds = new();
    }
}