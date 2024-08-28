using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject _StayPanel;
    [SerializeField] private GameObject _RequestsPanel;
    [SerializeField] private GameObject _StaffPanel;

    private GameObject _ActivePanel = null;

    private void Awake()
    {
        OnClickStays();
        _RequestsPanel.SetActive(false);
        _StaffPanel.SetActive(false);
    }

    public void OnClickStays()
    {
        _ActivePanel?.SetActive(false);
        _StayPanel.SetActive(true);
        _ActivePanel = _StayPanel;
    }
    public void OnClickRequests()
    {
        _ActivePanel?.SetActive(false);
        _RequestsPanel.SetActive(true);
        _ActivePanel = _RequestsPanel;
    }
    public void OnClickStaff()
    {
        _ActivePanel?.SetActive(false);
        _StaffPanel.SetActive(true);
        _ActivePanel = _StaffPanel;
    }

}
