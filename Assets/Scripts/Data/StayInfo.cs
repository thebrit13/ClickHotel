using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Could have room requirements (i.e. king bed, two queens, suite, etc.)
 */
public class StayInfo
{
    public string ID;
    public DateTime CheckInDate;
    public DateTime CheckOutDate;
    public int Nights;
    public List<GuestInfo> Guests;
}
