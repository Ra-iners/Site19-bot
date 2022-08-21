using System;

[Serializable]
public struct IWarning
{
    public ulong Caller; // ID of the user that created the warning
    public long Expires; // UNIX timestamp of when the warning should expire
    public string Reason; // Self-explanatory
    public string ID; // ID of the warning
}