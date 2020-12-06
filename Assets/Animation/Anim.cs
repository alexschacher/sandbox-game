using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Anim : ScriptableObject
{
    [SerializeField] private float animSpeed;
    [SerializeField] private List<Vector2Int> anim;

    public float GetAnimSpeed() => animSpeed;
    public List<Vector2Int> GetAnim() => anim;
}