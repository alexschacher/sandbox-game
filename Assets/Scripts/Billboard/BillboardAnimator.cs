using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Billboard))]
public class BillboardAnimator : MonoBehaviour
{
    private Billboard billboard;
    private List<Vector2Int> currentAnim;
    private int currentFrame;
    private float animSpeed = 1f;
    private float animTimer;
    private bool paused;

    private void Awake()
    {
        billboard = GetComponent<Billboard>();
        currentAnim = new List<Vector2Int>();
        currentAnim.Add(new Vector2Int(0, 0));
    }
    private void Update()
    {
        if (!paused) Animate();
    }
    private void Animate()
    {
        animTimer += (animSpeed * Time.deltaTime);

        if (animTimer >= 1f)
        {
            animTimer = animTimer % 1;
            currentFrame++;

            if (currentFrame >= currentAnim.Count)
            {
                currentFrame = 0;
            }
            SetBillboardFrame(currentAnim[currentFrame]);
        }
    }
    private void SetBillboardFrame(Vector2Int frame)
    {
        billboard.SetAnimFrame(frame.x, frame.y);
    }
    
    public void StartAnim(Anim anim)
    {
        UnPause();
        animSpeed = anim.GetAnimSpeed();

        List<Vector2Int> newAnim = anim.GetAnim();

        if (newAnim != currentAnim)
        {
            currentAnim = newAnim;
            RestartAnim();
        }
    }
    public void StopAnim()
    {
        Pause();
        RestartAnim();
    }
    public void RestartAnim()
    {
        animTimer = 0f;
        currentFrame = 0;
        SetBillboardFrame(currentAnim[currentFrame]);
    }
    public void Pause() => paused = true;
    public void UnPause() => paused = false;
}