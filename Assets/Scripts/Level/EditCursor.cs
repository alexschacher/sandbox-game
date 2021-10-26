using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EditCursor : MonoBehaviour
{
    [SerializeField] private Level level;
    [SerializeField] private InputActionAsset inputAsset;
    private InputAction scrollAction, mousePositionAction, placeAction, deleteAction, ctrlAction;

    private int levelWidth;
    private int levelHeight; 

    private int cursorHeight;
    private Vector3Int cursorPosition = new Vector3Int(0,0,0), prevCursorPosition;
    [SerializeField] private ID selectedID = ID.Post;

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

        levelWidth = level.GetLevelWidth();
        levelHeight = level.GetLevelHeight();
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
            if (scroll < 0)
            {
                selectedID++;
                if ((int)selectedID > System.Enum.GetNames(typeof(ID)).Length - 1)
                {
                    selectedID = 0;
                }
                HUD.SetSelectedID(selectedID);
            }
            else if (scroll > 0)
            {
                selectedID--;
                if ((int)selectedID < 0)
                {
                    selectedID = (ID)System.Enum.GetNames(typeof(ID)).Length - 1;
                }
                HUD.SetSelectedID(selectedID);
            }
        }
        else
        {
            // Adjust Height
            if (scroll < 0 && transform.position.y < (levelHeight - 1) * 0.5f)
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
        if (App.GetLevel().CheckIfInRange(cursorPosition.x, cursorHeight, cursorPosition.z) == false) return;

        level.Modify(selectedID, cursorPosition.x, cursorPosition.y, cursorPosition.z);
    }

    private void OnDeleteObject()
    {
        if (App.GetLevel().CheckIfInRange(cursorPosition.x, cursorHeight, cursorPosition.z) == false) return;

        level.Modify(ID.Empty, cursorPosition.x, cursorPosition.y, cursorPosition.z);
    }

    private void OnSelectObject()
    {
        if (App.GetLevel().CheckIfInRange(cursorPosition.x, cursorHeight, cursorPosition.z) == false) return;

        selectedID = level.GetID(cursorPosition.x, cursorHeight, cursorPosition.z);

        HUD.SetSelectedID(selectedID);
    }
}