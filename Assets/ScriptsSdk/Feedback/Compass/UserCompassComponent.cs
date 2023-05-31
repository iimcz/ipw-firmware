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

#if UNITY_EDITOR
    void Update()
    {
        _users[0].UpdatePosition(Input.mousePosition.x / Screen.width);

        if (Input.GetKeyDown(KeyCode.LeftArrow)) _users[0].AnimateGesture("swipe_left");
        else if (Input.GetKeyDown(KeyCode.UpArrow)) _users[0].AnimateGesture("swipe_up");
        else if (Input.GetKeyDown(KeyCode.RightArrow)) _users[0].AnimateGesture("swipe_right");
        else if (Input.GetKeyDown(KeyCode.DownArrow)) _users[0].AnimateGesture("swipe_down");

        if (Input.GetMouseButtonDown(0)) StartCoroutine(_users[0].Notify());
        if (Input.GetMouseButtonDown(1)) UserGesture(new SensorDataMessage { Path = "@nuitrack/skeleton/user/0/gestures/swipe_up" });
    }
#endif

    public void NotifyUser(int userIndex)
    {
        if (userIndex < 0 || userIndex >= MaxUsers)
        {
            Debug.LogWarning($"Received invalid user index {userIndex} for compass notification");
            return;
        }

        StartCoroutine(_users[userIndex].Notify());
    }

    public void UserGesture(SensorDataMessage message)
    {
        var pathParts = message.Path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);
        var gestureType = pathParts[^1];
        var userId = int.Parse(pathParts[3]);

        _users[userId].AnimateGesture(gestureType);
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
