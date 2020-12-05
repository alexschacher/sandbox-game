using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTorch : MonoBehaviour
{
    [SerializeField] private GameObject torch;

    private void OnGrab()
    {
        torch.SetActive(!torch.activeSelf);
    }
}
