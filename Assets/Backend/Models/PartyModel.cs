using System;
using System.Collections.Generic;

[Serializable]
public class PartyModel
{
    public string PartyId;
    public string Leader;
    public List<string> Members = new List<string>();
    public List<string> Invitations = new List<string>();
    public List<string> Requests = new List<string>();
}