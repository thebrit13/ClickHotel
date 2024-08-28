using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StayCard : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Image _BGImage;
    private StayInfo _StayInfo;
    private System.Action<StayCard> _MouseEnter;
    private System.Action<StayCard> _MouseExit;

    public void Setup(StayInfo si,System.Action<StayCard> mouseEnterCard, System.Action<StayCard> mouseExitCard)
    {
        _StayInfo = si;
        _MouseEnter = mouseEnterCard;
        _MouseExit = mouseExitCard;
    }

    public void OnClick()
    {
        //HotelManager.Instance.SelectedStay = _StayInfo;
    }

    public StayInfo GetStayInfo()
    {
        return _StayInfo;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _MouseEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _MouseExit?.Invoke(this);
    }

    public void ToggleRaycast(bool on)
    {
        _BGImage.raycastTarget = on;
    }
}
