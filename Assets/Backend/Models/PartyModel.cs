using System;
using System.Collections.Generic;

[Serializable]
public class PartyModel
{
    public string PartyId;
    public string Leader;
    public List<string> Members = new();
    public List<string> Invitations = new();
    public List<string> Requests = new();
}