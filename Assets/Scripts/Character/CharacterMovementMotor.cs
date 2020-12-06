using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FaceDir))]
public class CharacterMovementMotor : MonoBehaviour
{
    private CharacterIntention intention;
    private FaceDir faceDir;

    [SerializeField] private Vector2 moveDir;
    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        intention = GetComponent<CharacterIntention>();
        faceDir = GetComponent<FaceDir>();
    }
    private void Update()
    {
        moveDir = intention.GetMoveDir();

        Vector3 translation = new Vector3(moveDir.x, 0f, moveDir.y);
        translation = translation * Time.deltaTime * moveSpeed;
        transform.Translate(translation);

        faceDir.SetFaceDir(moveDir);
    }
}