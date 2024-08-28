using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Could have reviews and money per night
 * Good reviews attract more guest and stuff 
 * Money pays for improvements
 * 
 * Could have repeat guests (because of a positive experience)
 */



public class HotelManager : MonoBehaviour
{
    public static HotelManager Instance;

    [SerializeField] private RoomTileManager _RoomTileManager;
    [SerializeField] private TextAsset _PotentialGuestNames;

    public StayInfo SelectedStay = null;

    private DateTime _CurrentHotelTime = new DateTime(2024, 1, 1,12,0,0);
    private float HoursPerSecond = .5f;

    private GuestManager _GuestManager;

    //String is Day_Month
    private Dictionary<string,NightInfo> _StayDictionary;

    private List<StayInfo> _CurrentCheckinList = null;

    private int _StayCount = 0;

    private int _Cash = 0;

    private void Awake()
    {
        Instance = this;
        _GuestManager = new GuestManager(_PotentialGuestNames);
        Helper.RoomAssigned += RoomAssigned;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateStays();       
    }

    // Update is called once per frame
    void Update()
    {
        //General loop
        /*
         * Check out guests at 11am (or they are auto checked out)
         * Clean rooms
         * Check in guests at 3pm
         * Handle requests?
         * room service, towels, etc
         * REPEAT
         */
        _CurrentHotelTime = _CurrentHotelTime.AddHours(Time.deltaTime * HoursPerSecond);

        if(_CurrentHotelTime.Hour == GameConstants.CHECK_IN_TIME && _CurrentCheckinList == null)
        {
            PrepareForCheckIns();
        }

        //Any existing checkins should clear at midnight
        if(_CurrentCheckinList != null && _CurrentHotelTime.Hour == GameConstants.CLEAR_PENDING_STAYS_TIME)
        {
            UIManager.Instance.ClearPendingStays();
            _CurrentCheckinList = null;
            UIManager.Instance.SetCheckInCount(0);
        }

        UIManager.Instance?.SetHotelTimeText(_CurrentHotelTime);
    }

    private void RoomAssigned(StayInfo si)
    {
        _CurrentCheckinList.Remove(si);
        UIManager.Instance.RemoveStay(si);
        SelectedStay = null;
        UIManager.Instance.SetCheckInCount(_CurrentCheckinList.Count);
    }

    private void PrepareForCheckIns()
    {
        DateTime dt = new DateTime(_CurrentHotelTime.Ticks);
        string dayMonth = ConvertToDayMonthString(dt);
        _CurrentCheckinList = _StayDictionary[dayMonth].CheckinInfo;
        if(_CurrentCheckinList != null)
        {
            for(int i = 0; i < _CurrentCheckinList.Count;i++)
            {
                UIManager.Instance?.AddStay(_CurrentCheckinList[i]);
            }
        }
        else
        {
            Debug.LogError("Check in list is null!");
        }

        UIManager.Instance.SetCheckInCount(_CurrentCheckinList.Count);
    }

    public DateTime GetHotelTime()
    {
        return _CurrentHotelTime;
    }

    private void CreateGuest()
    {
        
    }

    private string ConvertToDayMonthString(DateTime dt)
    {
        return string.Format("{0}_{1}", dt.Day, dt.Month);
    }

    //There needs to be some logic with how stays are created
    //Max amount per night, etc.

    private StayInfo CreateStay(DateTime checkIn)
    {
        StayInfo si = new StayInfo();
        si.CheckInDate = new DateTime(checkIn.Ticks);
        int nights = UnityEngine.Random.Range(GameConstants.MIN_STAY_LENGTH, GameConstants.MAX_STAY_LENGTH+1);
        si.CheckOutDate = new DateTime(checkIn.Ticks).AddDays(nights);
        si.Nights = nights;
        si.ID = string.Format("Stay_{0}", _StayCount);
        _StayCount++;
        return si;
    }

    //Generates a month out
    private void GenerateStays()
    {
        _StayDictionary = new Dictionary<string, NightInfo>();
        //Creates 35 days ahead
        for (int i = 0; i < GameConstants.DAYS_IN_FUTURE_FOR_STAYS + GameConstants.MAX_STAY_LENGTH; i++)
        {
            DateTime dt = new DateTime(_CurrentHotelTime.Ticks).AddDays(i);
            NightInfo ni = new NightInfo();
            ni.AvailableRooms = GameConstants.ROOM_COUNT;
            ni.CheckinInfo = new List<StayInfo>();

            string dayMonth = ConvertToDayMonthString(dt);

            _StayDictionary.Add(dayMonth, ni);
        }

        //testing
        int dayCheckins = 6;//UnityEngine.Random.Range(1, GameConstants.ROOM_COUNT);
        for (int k = 0; k < dayCheckins; k++)
        {
            //Creates 30 days ahead
            for (int i = 0; i < GameConstants.DAYS_IN_FUTURE_FOR_STAYS; i++)
            {
                DateTime dt = new DateTime(_CurrentHotelTime.Ticks).AddDays(i);
                StayInfo stayInfo1 = CreateStay(dt);
                string dayMonth = ConvertToDayMonthString(dt);

                if (!_StayDictionary.ContainsKey(dayMonth))
                {
                    Debug.LogError("Error finding dayMonth key!");
                    return;
                }

                //First night
                //Actually adds stay
                NightInfo ni = _StayDictionary[dayMonth];
                //If no rooms are left, dont do anything
                if (ni.AvailableRooms == 0)
                {
                    continue;
                }
                ni.CheckinInfo.Add(stayInfo1);
                ni.AvailableRooms--;
                _StayDictionary[dayMonth] = ni;

                //Subsequent nights
                for (int j = 1; j < stayInfo1.Nights; j++)
                {
                    DateTime dt2 = new DateTime(dt.Ticks).AddDays(j);
                    string dayMonth2 = ConvertToDayMonthString(dt2);
                    if (!_StayDictionary.ContainsKey(dayMonth2))
                    {
                        Debug.LogError("Error finding dayMonth key!");
                        return;
                    }

                    //Just decrements nights available
                    NightInfo ni2 = _StayDictionary[dayMonth2];
                    //For now just remove the initial stay
                    //Dont touch available room count
                    if (ni2.AvailableRooms == 0)
                    {
                        ni.CheckinInfo.Remove(stayInfo1);
                        continue;
                    }
                    ni2.AvailableRooms--;
                    _StayDictionary[dayMonth2] = ni2;
                }
            }
        }

        //foreach(KeyValuePair<string,NightInfo> kvp in _StayDictionary)
        //{
        //    Debug.Log(string.Format("Date: {0} Rooms: {1}", kvp.Key, kvp.Value.AvailableRooms));
        //}
    }

    private void OnDestroy()
    {
        Helper.RoomAssigned -= RoomAssigned;
    }

    public void StayCheckedOut()
    {
        _Cash += GameConstants.COST_PER_NIGHT;
        UIManager.Instance.SetCashText(_Cash);
    }
}

public struct NightInfo
{
    public List<StayInfo> CheckinInfo;
    public int AvailableRooms;
}
