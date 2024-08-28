using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayPanel : MonoBehaviour
{
    [SerializeField] private StayCard _StayCard;
    [SerializeField] private Transform _Content;

    //Is currently not cleared out!
    private Queue<StayInfo> _QueuedStaysToDisplay = new Queue<StayInfo>();
    private int _StayCardsShowing = 0;

    private StayCard _CurrentCardOver = null;
    private Vector2 _BeginningCardPosition = Vector2.zero;

    private void Awake()
    {
        Helper.RoomAssignmentFailed += RoomAssignmentFailed;
    }

    private void Update()
    {
        if (_CurrentCardOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _BeginningCardPosition = _CurrentCardOver.transform.position;
                Helper.ActiveStayCard = _CurrentCardOver;
            }
            else if (Input.GetMouseButton(0))
            {
                _CurrentCardOver.transform.position = Input.mousePosition;
            }
            //Mouse up is handled in RoomTileManager
            //else if (Input.GetMouseButtonUp(0))
            //{
                //_CurrentCardOver.transform.position = _BeginningCardPosition;
                //_CurrentCardOver.ToggleRaycast(true);
                //_CurrentCardOver.gameObject.SetActive(false);
            //}
        }
    }

    private void RoomAssignmentFailed()
    {
        Helper.ActiveStayCard.transform.position = _BeginningCardPosition;
        Helper.ActiveStayCard = null;
    }

    public void AddStayCard(StayInfo stayInfo, bool queued)
    {
        if (_StayCardsShowing < GameConstants.MAX_STAY_CARD_SHOWING)
        {
            StayCard sc = Instantiate(_StayCard, _Content);
            sc.Setup(stayInfo,OnMouseEnterCard,OnMouseExitCard);
            _StayCardsShowing++;
        }
        else
        {
            if (!queued)
            {
                _QueuedStaysToDisplay.Enqueue(stayInfo);
            }
            else
            {
                Debug.LogError("This shouldnt happen...");
            }
        }
    }

    public void ClearAllCards()
    {
        //Remove all children
        for (int i = 0; i < _Content.childCount; i++)
        {
            Destroy(_Content.GetChild(i).gameObject);
        }
        _StayCardsShowing = 0;
        _QueuedStaysToDisplay.Clear();
    }

    public void RemoveCard(StayInfo si)
    {
        for(int i = 0; i < _Content.childCount;i++)
        {
            StayCard sc = _Content.GetChild(i).GetComponent<StayCard>();
            if(sc)
            {
                if(sc.GetStayInfo() == si)
                {
                    Destroy(sc.gameObject);
                    _StayCardsShowing--;
                    if (_QueuedStaysToDisplay.Count > 0)
                    {
                        StayInfo siNext = _QueuedStaysToDisplay.Dequeue();
                        AddStayCard(siNext, true);
                    }
                    return;
                }
            }
        }
    }

    private void OnDestroy()
    {
        Helper.RoomAssignmentFailed -= RoomAssignmentFailed;
    }

    private void OnMouseEnterCard(StayCard sc)
    {
        _CurrentCardOver = sc;
    }

    private void OnMouseExitCard(StayCard sc)
    {
        if(sc == _CurrentCardOver)
        {
            _CurrentCardOver = null;
        }
    }

}
