using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(BillboardAnimator))]
public class CharacterAnimator : NetworkBehaviour
{
    public enum AnimState { Stand, Walk, Fish, Reel }

    [SerializeField] private Anim walkAnim;
    [SerializeField] private Anim standAnim;
    [SerializeField] private Anim fishAnim;
    [SerializeField] private Anim reelAnim;

    private CharCore core;
    private BillboardAnimator animator;
    private float animationGrace = 0.15f;
    private float animationGraceTimer;
    [SerializeField] private AnimState animState = AnimState.Stand;

    private void Awake()
    {
        animator = GetComponent<BillboardAnimator>();
        core = GetComponent<CharCore>();
    }
    private void Update()
    {
        if (core.GetFishingState() != Fishing.State.NotFishing)  // This is our "can we move?" check, also used in character movement motor
        {
            AnimateFishing();
        }
        else
        {
            AnimateDefault();
        }
    }
    private void AnimateDefault()
    {
        if (core.GetWalkDir() != Vector2.zero)
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
    }

    private void AnimateFishing()
    {
        if (core.GetFishingState() == Fishing.State.ReelingIn)
        {
            if (animState != AnimState.Reel)
            {
                animator.StartAnim(reelAnim);
                animState = AnimState.Reel;
            }
        }
        else if (animState != AnimState.Fish)
        {
            animator.StartAnim(fishAnim);
            animState = AnimState.Fish;
        }
    }
}