using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementMotor : MonoBehaviour
{
    private CharacterIntention intention;
    private Vector2 moveDir;
    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        intention = GetComponent<CharacterIntention>();
    }
    private void Update()
    {
        moveDir = intention.GetMoveDir();
        Vector3 translation = new Vector3(moveDir.x, 0f, moveDir.y);
        translation = translation * Time.deltaTime * moveSpeed;
        transform.Translate(translation);
    }
}