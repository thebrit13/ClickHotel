using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestManager 
{
    private string[] _PotentialGuestNames;

    public GuestManager(TextAsset guestNamesTA)
    {
        _PotentialGuestNames = guestNamesTA.text.Split('\n');
    }

    public string GetRandomName()
    {
        return _PotentialGuestNames[Random.Range(0, _PotentialGuestNames.Length)];
    }
}
