using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleProfilePanel : MonoBehaviour
{
    public GameObject profilePanel;
    private RectTransform panelRect;
    private float panelLeft;
    private float panelWidth;

    void Start()
    {
        panelRect = profilePanel.GetComponent<RectTransform>();
        panelLeft = panelRect.rect.x;
        panelWidth = panelRect.rect.width;
        // panelRect.offsetMax = new Vector2(-720,0);
        Debug.Log("Left:" + panelLeft);
        Debug.Log("Width:" + panelWidth);
        // panelRect.position.Set(-panelWidth, 0, 0);
        panelLeft = panelRect.rect.x;
        Debug.Log("Desp.Left:" + panelLeft);
        Debug.Log("Desp.Width:" + panelWidth);
    }

    void Update()
    {
    }

    public void Toggle()
    {
        profilePanel.SetActive(!profilePanel.activeSelf);
    }
}
