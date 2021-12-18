using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing : NetworkBehaviour
{
    // needs to only execute if has authority

    public enum State { NotFishing, WaitingForBite, FishIsBiting, ReelingIn }

    private CharCore core;
    [SerializeField] private GameObject bobberPrefab;
    private GameObject activeBobber;

    private float fishBiteTimer;
    private float fishBiteTimerMin = 3f;
    private float fishBiteTimerMax = 15f;

    private float breakLineTimer;
    private float breakLineTimerMin = 0.5f;
    private float breakLineTimerMax = 2f;

    private float reelingTimer;
    private float reelingTimerMin = 3f;
    private float reelingTimerMax = 4f;
    private int reelTaps;
    private int reelTapsGoal;
    private int reelTapsGoalMin = 8;
    private int reelTapsGoalMax = 20;


    private void Awake()
    {
        core = GetComponent<CharCore>();
    }

    public void Update()
    {
        if (hasAuthority == false) return;

        if (core.GetFishingState() == State.WaitingForBite)
        {
            fishBiteTimer -= Time.deltaTime;
            if (fishBiteTimer < 0)
            {
                FishBite();
            }
        }
        else if (core.GetFishingState() == State.FishIsBiting)
        {
            breakLineTimer -= Time.deltaTime;
            if (breakLineTimer < 0)
            {
                BreakLine();
            }
        }
        else if (core.GetFishingState() == State.ReelingIn)
        {
            reelingTimer -= Time.deltaTime;
            if (reelingTimer < 0)
            {
                BreakLine();
            }
        }
    }

    public void StartFishing(Vector3Int fishingCoords)
    {
        Debug.Log("Start fishing");
        core.SetFishingState(State.WaitingForBite);
        fishBiteTimer = Random.Range(fishBiteTimerMin, fishBiteTimerMax);
        DestroyBobber();
        activeBobber = Instantiate(bobberPrefab, VectorMath.GetPositionFromWorldVoxelCoords(fishingCoords), Quaternion.identity);
        // set fisherman to fishing animation
        // play start fishing sound
    }

    public void StopFishing()
    {
        core.SetFishingState(State.NotFishing);
        DestroyBobber();
        // change fishermans animation to idle
    }

    public void Interact()
    {
        if (core.GetFishingState() == State.FishIsBiting)
        {
            StartReeling();
        }
        else if (core.GetFishingState() == State.ReelingIn)
        {
            ReelTap();
        }
        else
        {
            CancelFishing();
        }
    }

    private void FishBite()
    {
        Debug.Log("Fish Bite!");
        core.SetFishingState(State.FishIsBiting);
        breakLineTimer = Random.Range(breakLineTimerMin, breakLineTimerMax);
        activeBobber.transform.position -= new Vector3(0f, 0.12f, 0f);
    }

    private void CancelFishing()
    {
        Debug.Log("Cancel Fishing");
        // play cancel fishing sound
        StopFishing();
    }

    private void BreakLine()
    {
        Debug.Log("Line broke");
        // play break line sound
        StopFishing();
    }

    private void StartReeling()
    {
        core.SetFishingState(State.ReelingIn);
        reelingTimer = Random.Range(reelingTimerMin, reelingTimerMax);
        reelTapsGoal = Random.Range(reelTapsGoalMin, reelTapsGoalMax);
        reelTaps = 1;
    }

    private void ReelTap()
    {
        reelTaps++;
        if (reelTaps >= reelTapsGoal)
        {
            CatchFish();
        }
    }

    private void CatchFish()
    {
        Debug.Log("Caught Fish!");
        core.SetHeldItem(eID.Fish);
        // play catch fish sound
        StopFishing();
    }

    private void DestroyBobber()
    {
        if (activeBobber != null)
        {
            Destroy(activeBobber);
        }
    }
}