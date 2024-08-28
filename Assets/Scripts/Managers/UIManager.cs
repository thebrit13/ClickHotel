using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI _HotelTimeText;
    [SerializeField] private TextMeshProUGUI _StatusText;
    [SerializeField] private TextMeshProUGUI _CashText;
    [SerializeField] private StayPanel _StayPanel;

    private void Awake()
    {
        Instance = this;
        SetCheckInCount(0);
        SetCashText(0);
    }

    public void SetHotelTimeText(DateTime hotelTime)
    {
        _HotelTimeText.text = hotelTime.ToShortTimeString();
    }

    public void AddStay(StayInfo stayInfo)
    {
        _StayPanel.AddStayCard(stayInfo,false);
    }

    public void RemoveStay(StayInfo stayInfo)
    {
        _StayPanel.RemoveCard(stayInfo);
    }

    public void SetCheckInCount(int amount)
    {
        _StatusText.text = string.Format("Checkins: {0}", amount);
    }

    public void ClearPendingStays()
    {
        _StayPanel.ClearAllCards();
    }

    public void SetCashText(int amount)
    {
        _CashText.text = string.Format("Cash: {0}", amount);
    }
        
}
