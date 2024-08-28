using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTileManager : MonoBehaviour
{
    [SerializeField] private GameObject _RoomTileRow;
    [SerializeField] private RoomTile _RoomTile;

    private const int ROWS = 3;
    private const int TILES_PER_ROW = 3;

    private List<RoomTile> _CreatedTiles;
    private RoomTile _CurrentRoomTileOver = null;

    // Start is called before the first frame update
    void Start()
    {
        CreateRooms();   
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonUp(0) && Helper.ActiveStayCard)
        {
            Debug.Log("CALLED");
            RoomTile closestTile = GetClosestRoomTileToMouse();
            if(closestTile)
            {
                closestTile.CheckInRoom(Helper.ActiveStayCard.GetStayInfo());
                Destroy(Helper.ActiveStayCard.gameObject);
                Helper.ActiveStayCard = null;
            }
            else
            {
                //Go back to original spot
                Helper.RoomAssignmentFailed?.Invoke();
                Debug.LogWarning("No room tile close enough!");
            }
        }
    }

    RoomTile GetClosestRoomTileToMouse()
    {
        float closest = float.MaxValue;
        RoomTile temp = null;
        foreach(RoomTile rt in _CreatedTiles)
        {
            float currentClosest = Vector2.Distance(rt.transform.position, Input.mousePosition);
            if(currentClosest <= GameConstants.TILE_TO_STAY_ACCEPTANCE_RANGE && currentClosest < closest)
            {
                temp = rt;
                closest = currentClosest;
            }           
        }
        return temp;
    }

    private void CreateRooms()
    {
        _CreatedTiles = new List<RoomTile>();
        for(int i = 0; i < ROWS;i++)
        {
            GameObject roomTileRow = Instantiate(_RoomTileRow, this.transform);
            for(int j = 0; j < TILES_PER_ROW;j++)
            {
                string roomTileID = string.Format("Room_{0}{1}",i,j);
                RoomTile rt = Instantiate(_RoomTile, roomTileRow.transform);
                rt.Setup(roomTileID,OnMouseEnterCard,OnMouseExitCard);
                rt.gameObject.name = roomTileID;
                
                _CreatedTiles.Add(rt);
            }
        }
    }

    private void OnRoomTileClicked(RoomTile rt)
    {
        if(HotelManager.Instance?.SelectedStay != null)
        {
            if(rt.GetRoomStatus() == RoomStatus.DIRTY || rt.GetRoomStatus() == RoomStatus.OCCUPIED)
            {
                return;
            }

            rt.CheckInRoom(HotelManager.Instance?.SelectedStay);
        }
    }
    private void OnMouseEnterCard(RoomTile rt)
    {
        _CurrentRoomTileOver = rt;
    }

    private void OnMouseExitCard(RoomTile rt)
    {
        if (rt == _CurrentRoomTileOver)
        {
            _CurrentRoomTileOver = null;
        }
    }

}
