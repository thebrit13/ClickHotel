using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum RoomStatus
{
    CLEAN,
    DIRTY,
    OCCUPIED
}

public class RoomTile : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    public string ID;

    private RoomStatus _RoomStatus = RoomStatus.CLEAN;

    [SerializeField] private Slider _StaySlider;
    [SerializeField] private TextMeshProUGUI _StatusText;

    //private System.Action<RoomTile> _OnClick;
    private System.Action<RoomTile> _MouseEnterTile;
    private System.Action<RoomTile> _MouseExitTile;

    private StayInfo _CurrentStayInfo = null;

    public void Setup(string id, System.Action<RoomTile> mouseEnterCard, System.Action<RoomTile> mouseExitCard)
    {
        ID = id;
        //_OnClick = onClick;
        _MouseEnterTile = mouseEnterCard;
        _MouseExitTile = mouseExitCard;

        SetRoomStatus(RoomStatus.CLEAN);
    }

    public void OnClick()
    {
        //_OnClick?.Invoke(this);

        if(_RoomStatus == RoomStatus.DIRTY)
        {
            SetRoomStatus(RoomStatus.CLEAN);
            return;
        }
    }

    private void Update()
    {
        if (_CurrentStayInfo != null)
        {
            int stayInHours = (int)(_CurrentStayInfo.CheckOutDate - _CurrentStayInfo.CheckInDate).TotalHours;

            double stayInHoursLeft = (_CurrentStayInfo.CheckOutDate - HotelManager.Instance.GetHotelTime()).TotalHours;

            _StaySlider.value = 1.0f - (float)(stayInHoursLeft / stayInHours);

            //For now
            if (stayInHoursLeft <= 0)
            {
                CheckOutRoom();
            }
        }
    }

    public void CheckInRoom(StayInfo stayInfo)
    {
        _CurrentStayInfo = stayInfo;
        SetRoomStatus(RoomStatus.OCCUPIED);
        Helper.RoomAssigned?.Invoke(stayInfo);
    }

    public void CheckOutRoom()
    {
        _CurrentStayInfo = null;
        SetRoomStatus(RoomStatus.DIRTY);
        HotelManager.Instance?.StayCheckedOut();
    }

    private void SetRoomStatus(RoomStatus newStatus)
    {
        _RoomStatus = newStatus;
        _StatusText.text = newStatus.ToString();
    }

    public RoomStatus GetRoomStatus()
    {
        return _RoomStatus;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _MouseEnterTile?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _MouseExitTile(this);
    }
}
