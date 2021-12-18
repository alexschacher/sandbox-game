using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiFollowObject : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform UI_element;
    [SerializeField] private Transform UI_element2;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        UI_element.transform.position = cam.WorldToScreenPoint(transform.position + offset);
        UI_element2.transform.position = cam.WorldToScreenPoint(transform.position + offset) + new Vector3(2f, -1f, 0f);
    }

    public void SetText(string text)
    {
        Text textUI = UI_element.GetComponent<Text>();
        if (textUI != null)
        {
            textUI.text = text;
        }
        Text textUI2 = UI_element2.GetComponent<Text>();
        if (textUI2 != null)
        {
            textUI2.text = text;
        }
    }
}
