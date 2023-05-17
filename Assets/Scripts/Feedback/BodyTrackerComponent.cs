using Assets.ScriptsSdk.Extensions;
using Naki3D.Common.Protocol;
using System.Text.RegularExpressions;
using UnityEngine;

public class BodyTrackerComponent : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lineRenderer;

    public int UserIndex = 1;
    public float DistanceScale = 10f;

    [SerializeField]
    private Transform Chest;

    [SerializeField]
    private Transform LeftHand, RightHand;

    [SerializeField]
    private Transform LeftElbow, RightElbow;

    [SerializeField]
    private Transform LeftShoulder, RightShoulder;

    private Vector3 _chestPosition;
    private Regex _userMoveRegex = new(@"nuitrack\/skeleton\/user\/(\d+)\/(.+)\/position\/normalized", RegexOptions.Compiled);

    void Start()
    {
        UpdateSkeletonLine();
    }

    public void UpdateSkeletonJoint(SensorDataMessage message)
    {
        var match = _userMoveRegex.Match(message.Path);
        if (!match.Success) return;

        var userIndex = int.Parse(match.Groups[1].Value);
        if (UserIndex != userIndex) return;

        var jointPosition = message.Vector3.ToUnityVector();
        var relativeToChest = (jointPosition - _chestPosition) * DistanceScale;

        // TODO: Check if these actually match
        var bodyPart = match.Groups[2].Value;
        switch (bodyPart)
        {
            case "chest":
                _chestPosition = jointPosition; // Chest always stays at 0,0, only serves as reference point
                // We also don't recalculate other joints, nuitrack should send those soon enough
                break;
            case "left_hand":
                LeftHand.localPosition = relativeToChest;
                break;
            case "right_hand":
                RightHand.localPosition = relativeToChest;
                break;
            case "left_elbow":
                LeftElbow.localPosition = relativeToChest;
                break;
            case "right_elbow":
                RightElbow.localPosition = relativeToChest;
                break;
            case "left_shoulder":
                LeftShoulder.localPosition = relativeToChest;
                break;
            case "right_shoulder":
                RightShoulder.localPosition = relativeToChest;
                break;
        }

        UpdateSkeletonLine();
    }

    private void UpdateSkeletonLine()
    {
        _lineRenderer.SetPositions(new[]
        {
            LeftHand.position,
            LeftElbow.position,
            LeftShoulder.position,
            Chest.position,
            RightShoulder.position,
            RightElbow.position,
            RightHand.position,
        });
    }
}
