using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(BillboardAnimator))]
public class CharacterAnimator : NetworkBehaviour
{
    public enum AnimState { Stand, Walk }

    [SerializeField] private Anim walkAnim;
    [SerializeField] private Anim standAnim;

    private BillboardAnimator animator;
    private Vector3 currentPosition;
    private Vector3 prevPosition;
    private float animationGrace = 0.15f;
    private float animationGraceTimer;
    [SerializeField] private AnimState animState = AnimState.Stand;

    private void Awake()
    {
        animator = GetComponent<BillboardAnimator>();
    }
    private void Update()
    {
        currentPosition = transform.position;

        if (currentPosition != prevPosition)
        {
            if (animState != AnimState.Walk)
            {
                animator.StartAnim(walkAnim);
                animState = AnimState.Walk;
            }
        }
        else
        {
            if (animState != AnimState.Stand)
            {
                if (!hasAuthority)
                {
                    animationGraceTimer += Time.deltaTime;
                    if (animationGraceTimer > animationGrace)
                    {
                        animationGraceTimer = 0f;
                        animator.StartAnim(standAnim);
                        animState = AnimState.Stand;
                    }
                }
                else
                {
                    animator.StartAnim(standAnim);
                    animState = AnimState.Stand;
                }
            }
        }
        prevPosition = currentPosition;
    }
}