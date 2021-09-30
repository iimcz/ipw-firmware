using System;
using UnityEngine;

[Serializable]
public class TransitionInfo
{
    public enum MovementTypeEnum
    {
        AnimationCurve,
        ConstantSpeed,
        ConstantTime,
        Teleport,
        Vignette // This needs some thought
    }

    public NavigationNodeComponent NextNode;
    public MovementTypeEnum MovementType = MovementTypeEnum.ConstantTime;

    public float TravelTime = 1.0f;
    public float Speed = 2.0f;
    public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
}
