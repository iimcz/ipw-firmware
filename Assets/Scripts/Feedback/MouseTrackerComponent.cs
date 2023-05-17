using Naki3D.Common.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseTrackerComponent : MonoBehaviour
{
    public enum JointType
    {
        Chest,
        LeftHand,
        RightHand,
        LeftElbow,
        RightElbow,
        LeftShoulder,
        RightShoulder,
    }

    public JointType ActiveJoint = JointType.Chest;
    public int UserId = 1;

    public UnityEvent<SensorDataMessage> Target;

    private string[] joints = new[]
    {
        "chest",
        "left_hand",
        "right_hand",
        "left_elbow",
        "right_elbow",
        "left_shoulder",
        "right_shoulder",
    };

    void Update()
    {
        if (!Input.GetMouseButton(0)) return;

        Target.Invoke(new SensorDataMessage
        {
            Vector3 = new Vector3Data
            {
                X = Input.mousePosition.x / Screen.width,
                Y = Input.mousePosition.y / Screen.height,
                Z = 0
            },
            Path = $"nuitrack/skeleton/user/{UserId}/{joints[(int)ActiveJoint]}/position/normalized"
        });
    }
}
