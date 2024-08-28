using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper 
{
    public static System.Action<StayInfo> RoomAssigned;
    public static System.Action RoomAssignmentFailed;
    public static StayCard ActiveStayCard = null;
}
