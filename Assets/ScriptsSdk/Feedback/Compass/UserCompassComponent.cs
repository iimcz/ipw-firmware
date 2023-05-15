using Naki3D.Common.Protocol;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class UserCompassComponent : MonoBehaviour
{
    [Min(1)]
    public int MaxUsers = 6;

    [Min(0.01f)]
    public float InactivityDelay = 5.0f;

    public GameObject UserPrefab;
    public List<Color> UserColors = new() { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.cyan };

    private List<UserDotComponent> _users = new();
    private Regex _userMoveRegex = new(@"nuitrack\/skeleton\/user\/(\d+)\/torso\/position\/normalized", RegexOptions.Compiled);
    private Regex _userConfidenceRegex = new(@"nuitrack\/skeleton\/user\/(\d+)\/torso\/confidence", RegexOptions.Compiled);

    void Start()
    {
        for (int i = 0; i < MaxUsers; i++)
        {
            var user = Instantiate(UserPrefab, transform);
            user.SetActive(false);
            user.name = $"User {i}";

            var userDot = user.GetComponent<UserDotComponent>();
            userDot.SetColor(UserColors[i % UserColors.Count]);
            userDot.InactivityDelay = InactivityDelay;

            _users.Add(userDot);
        }
    }

    public void NotifyUser(int userIndex)
    {
        if (userIndex < 0 || userIndex >= MaxUsers)
        {
            Debug.LogWarning($"Received invalid user index {userIndex} for compass notification");
            return;
        }

        StartCoroutine(_users[userIndex].Notify());
    }

    public void UserConfidence(SensorDataMessage message)
    {
        var match = _userConfidenceRegex.Match(message.Path);
        if (!match.Success) return;

        var userIndex = int.Parse(match.Groups[1].Value);
        if (userIndex < 0 || userIndex >= MaxUsers)
        {
            Debug.LogWarning($"Received invalid user index {userIndex} for compass confidence");
            return;
        }

        _users[userIndex].UpdateConfidence(message.Float);
    }

    public void UserMove(SensorDataMessage message)
    {
        var match = _userMoveRegex.Match(message.Path);
        if (!match.Success) return;

        var userIndex = int.Parse(match.Groups[1].Value);
        if (userIndex < 0 || userIndex >= MaxUsers)
        {
            Debug.LogWarning($"Received invalid user index {userIndex} for compass position");
            return;
        }

        _users[userIndex].UpdatePosition(1.0f - message.Vector3.X);
    }
}
