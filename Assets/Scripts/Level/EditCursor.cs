﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EditCursor : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputAsset;
    private InputAction scrollAction, mousePositionAction, placeAction, deleteAction, ctrlAction;

    private int cursorHeight;
    private Vector3Int cursorPosition = new Vector3Int(0,0,0), prevCursorPosition;

    [SerializeField] private vID selectedVID = vID.Post;
    [SerializeField] private eID selectedEID = eID.Apple;
    private bool selectionModeIsVoxel = true;

    private void Awake()
    {
        InputActionMap inputActions = inputAsset.FindActionMap("Play");

        placeAction = inputActions.FindAction("PlaceObject");
        deleteAction = inputActions.FindAction("DeleteObject");
        ctrlAction = inputActions.FindAction("Ctrl");

        scrollAction = inputActions.FindAction("Scroll");
        scrollAction.performed += Scroll;
        scrollAction.Enable();

        mousePositionAction = inputActions.FindAction("MousePosition");
        mousePositionAction.Enable();
    }

    private void Start()
    {
        UpdateCursorPosition();
    }

    private void Update()
    {
        UpdateCursorPosition();
        CheckForNewCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePositionAction.ReadValue<Vector2>());
        Plane plane = new Plane(Vector3.up, -cursorHeight * 0.5f);
        float distanceToPlane;
        plane.Raycast(mouseRay, out distanceToPlane);

        prevCursorPosition = cursorPosition;
        cursorPosition = new Vector3Int(
            Mathf.RoundToInt(mouseRay.GetPoint(distanceToPlane).x),
            cursorHeight,
            Mathf.RoundToInt(mouseRay.GetPoint(distanceToPlane).z));

        transform.position = new Vector3(cursorPosition.x, cursorPosition.y * 0.5f, cursorPosition.z);
    }

    private Vector3 GetUnroundedCursorPosition()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePositionAction.ReadValue<Vector2>());
        Plane plane = new Plane(Vector3.up, -cursorHeight * 0.5f);
        float distanceToPlane;
        plane.Raycast(mouseRay, out distanceToPlane);

        return new Vector3(
            mouseRay.GetPoint(distanceToPlane).x,
            cursorHeight * 0.5f,
            mouseRay.GetPoint(distanceToPlane).z);
    }

    private void CheckForNewCursorPosition()
    {
        if (cursorPosition != prevCursorPosition)
        {
            if (placeAction.ReadValue<float>() > 0.5f)
            {
                OnPlaceObject();
            }
            else if (deleteAction.ReadValue<float>() > 0.5f)
            {
                OnDeleteObject();
            }
        }
    }

    private void Scroll(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<float>();

        if (ctrlAction.ReadValue<float>() > 0.5f)
        {
            // Scroll through IDs
            if (scroll > 0)
            {
                if (selectionModeIsVoxel)
                {
                    selectedVID++;
                    if ((int)selectedVID > System.Enum.GetNames(typeof(vID)).Length - 1)
                    {
                        selectedEID = 0;
                        selectionModeIsVoxel = false;
                        HUD.SetSelectedID(selectedEID.ToString());
                    }
                    else
                    {
                        HUD.SetSelectedID(selectedVID.ToString());
                    }
                }
                else
                {
                    selectedEID++;
                    if ((int)selectedEID > System.Enum.GetNames(typeof(eID)).Length - 1)
                    {
                        selectedVID = 0;
                        selectionModeIsVoxel = true;
                        HUD.SetSelectedID(selectedVID.ToString());
                    }
                    else
                    {
                        HUD.SetSelectedID(selectedEID.ToString());
                    }
                }
            }
            else if (scroll < 0)
            {
                if (selectionModeIsVoxel)
                {
                    selectedVID--;
                    if ((int)selectedVID > 65000)
                    {
                        selectedEID = (eID)System.Enum.GetNames(typeof(eID)).Length - 1;
                        selectionModeIsVoxel = false;
                        HUD.SetSelectedID(selectedEID.ToString());
                    }
                    else
                    {
                        HUD.SetSelectedID(selectedVID.ToString());
                    }
                }
                else
                {
                    selectedEID--;
                    if ((int)selectedEID > 65000)
                    {
                        selectedVID = (vID)System.Enum.GetNames(typeof(vID)).Length - 1;
                        selectionModeIsVoxel = true;
                        HUD.SetSelectedID(selectedVID.ToString());
                    }
                    else
                    {
                        HUD.SetSelectedID(selectedEID.ToString());
                    }
                }
            }
        }
        else
        {
            // Adjust Height
            if (scroll < 0 && transform.position.y < (Chunk.height - 1) * 0.5f)
            {
                cursorHeight++;
            }
            if (scroll > 0 && transform.position.y > 0)
            {
                cursorHeight--;
            }
        }
    }

    private void OnPlaceObject()
    {
        if (LevelHandler.CheckIfInRange(cursorPosition.x, cursorPosition.y, cursorPosition.z) == false) return;

        if (selectionModeIsVoxel)
        {
            LevelHandler.ModifyVoxel(selectedVID, cursorPosition.x, cursorPosition.y, cursorPosition.z);
        }
        else
        {
            LevelHandler.SpawnEntity(selectedEID, GetUnroundedCursorPosition());
        }
    }

    private void OnDeleteObject()
    {
        if (LevelHandler.CheckIfInRange(cursorPosition.x, cursorPosition.y, cursorPosition.z) == false) return;

        LevelHandler.ModifyVoxel(vID.Empty, cursorPosition.x, cursorPosition.y, cursorPosition.z);
    }

    private void OnSelectObject()
    {
        if (LevelHandler.CheckIfInRange(cursorPosition.x, cursorPosition.y, cursorPosition.z) == false) return;

        selectionModeIsVoxel = true;
        selectedVID = LevelHandler.GetVoxelIdAtPosition(cursorPosition.x, cursorHeight, cursorPosition.z);

        HUD.SetSelectedID(selectedVID.ToString());
    }
}