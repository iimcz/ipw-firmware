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
    private Material[] _materials = new Material[7];

    private Regex _userMoveRegex = new(@"nuitrack\/skeleton\/user\/(\d+)\/(.+)\/position\/normalized", RegexOptions.Compiled);
    private Regex _userConfidenceRegex = new(@"nuitrack\/skeleton\/user\/(\d+)\/(.+)\/confidence", RegexOptions.Compiled);


    void Start()
    {
        UpdateSkeletonLine();

        _materials[0] = Chest.GetComponent<MeshRenderer>().material;
        _materials[1] = LeftHand.GetComponent<MeshRenderer>().material;
        _materials[2] = RightHand.GetComponent<MeshRenderer>().material;
        _materials[3] = LeftElbow.GetComponent<MeshRenderer>().material;
        _materials[4] = RightElbow.GetComponent<MeshRenderer>().material;
        _materials[5] = LeftShoulder.GetComponent<MeshRenderer>().material;
        _materials[6] = RightShoulder.GetComponent<MeshRenderer>().material;
    }

    private void UpdateJointPosition(Match match, Vector3 jointPosition)
    {
        var userIndex = int.Parse(match.Groups[1].Value);
        if (UserIndex != userIndex) return;

        jointPosition.z = 0;
        jointPosition.y = 1.0f - jointPosition.y;
        jointPosition.x = 1.0f - jointPosition.x;
        var relativeToChest = (jointPosition - _chestPosition) * DistanceScale;

        var bodyPart = match.Groups[2].Value;
        switch (bodyPart)
        {
            case "torso":
                _chestPosition = jointPosition; // Chest always stays at 0,0, only serves as reference point
                // We also don't recalculate other joints, nuitrack should send those soon enough
                break;
            case "lefthand":
                LeftHand.localPosition = relativeToChest;
                break;
            case "righthand":
                RightHand.localPosition = relativeToChest;
                break;
            case "leftelbow":
                LeftElbow.localPosition = relativeToChest;
                break;
            case "rightelbow":
                RightElbow.localPosition = relativeToChest;
                break;
            case "leftshoulder":
                LeftShoulder.localPosition = relativeToChest;
                break;
            case "rightshoulder":
                RightShoulder.localPosition = relativeToChest;
                break;
        }

        UpdateSkeletonLine();
    }

    private void UpdateJointConfidence(Match match, float confidence)
    {
        var userIndex = int.Parse(match.Groups[1].Value);
        if (UserIndex != userIndex) return;

        var bodyPart = match.Groups[2].Value;
        var color = Color.Lerp(Color.red, Color.cyan, confidence);
        switch (bodyPart)
        {
            case "torso":
                _materials[0].color = color;
                break;
            case "lefthand":
                _materials[1].color = color;
                break;
            case "righthand":
                _materials[2].color = color;
                break;
            case "leftelbow":
                _materials[3].color = color;
                break;
            case "rightelbow":
                _materials[4].color = color;
                break;
            case "leftshoulder":
                _materials[5].color = color;
                break;
            case "rightshoulder":
                _materials[6].color = color;
                break;
        }
    }

    public void UpdateSkeletonJoint(SensorDataMessage message)
    {
        var match = _userMoveRegex.Match(message.Path);
        if (match.Success) UpdateJointPosition(match, message.Vector3.ToUnityVector());

        match = _userConfidenceRegex.Match(message.Path);
        if (match.Success) UpdateJointConfidence(match, message.Float);
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
