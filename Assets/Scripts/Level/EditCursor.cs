using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditCursor : MonoBehaviour
{
    [SerializeField] private Level level;

    [SerializeField] private InputActionAsset inputAsset;
    private InputAction scrollAction, mousePositionAction;

    private int levelWidth;
    private int levelHeight; 

    private int cursorHeight;
    private Vector3Int cursorPosition;
    private ID selectedID = ID.Ground;

    private void Awake()
    {
        InputActionMap inputActions = inputAsset.FindActionMap("Play");

        scrollAction = inputActions.FindAction("Scroll");
        scrollAction.performed += Scroll;
        scrollAction.Enable();

        mousePositionAction = inputActions.FindAction("MousePosition");
        mousePositionAction.Enable();

        levelWidth = level.GetLevelWidth();
        levelHeight = level.GetLevelHeight();
    }

    private void Scroll(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<float>();

        if (scroll < 0 && transform.position.y < (levelHeight - 1) * 0.5f)
        {
            cursorHeight++;
        }
        if (scroll > 0 && transform.position.y > 0)
        {
            cursorHeight--;
        }
    }

    private void Update()
    {
        UpdateCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePositionAction.ReadValue<Vector2>());
        Plane plane = new Plane(Vector3.up, -cursorHeight * 0.5f);
        float distanceToPlane;
        plane.Raycast(mouseRay, out distanceToPlane);

       cursorPosition = new Vector3Int(
            Mathf.RoundToInt(mouseRay.GetPoint(distanceToPlane).x),
            cursorHeight,
            Mathf.RoundToInt(mouseRay.GetPoint(distanceToPlane).z));

        transform.position = new Vector3(cursorPosition.x, cursorPosition.y * 0.5f, cursorPosition.z);
    }

    private void OnPlaceObject()
    {
        if (IfWithinChunk() == false) return;

        level.Modify(selectedID, cursorPosition.x, cursorPosition.y, cursorPosition.z);
    }

    private void OnDeleteObject()
    {
        if (IfWithinChunk() == false) return;

        level.Modify(ID.Empty, cursorPosition.x, cursorPosition.y, cursorPosition.z);
    }

    private void OnSelectObject()
    {
        if (IfWithinChunk() == false) return;

        selectedID = level.GetCellID(cursorPosition.x, cursorHeight, cursorPosition.z);

        Debug.Log("Selected " + selectedID.ToString());
    }

    private bool IfWithinChunk()
    {
        if (cursorPosition.x >= 0 &&
            cursorPosition.z >= 0 &&
            cursorPosition.x < levelWidth &&
            cursorPosition.z < levelWidth)
        {
            return true;
        }
        return false;
    }
}