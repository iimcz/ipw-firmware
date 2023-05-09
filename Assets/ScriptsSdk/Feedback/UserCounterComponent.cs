using Naki3D.Common.Protocol;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UserCounterComponent : MonoBehaviour
{
    class CountedUser
    {
        public float LastActivity;
        public GameObject GameObject;
    }

    [Min(1)]
    public int MaxUsers = 6;

    [Min(0.01f)]
    public float InactivityDelay = 5.0f;

    public GameObject UserPrefab;
    public List<Color> UserColors = new() { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.cyan };

    private List<CountedUser> _users = new();
    private Regex _userRegex = new(@"nuitrack\/skeleton\/user\/(\d+)\/joint_torso\/confidence", RegexOptions.Compiled);

    void Start()
    {
        for (int i = 0; i < MaxUsers; i++)
        {
            var user = Instantiate(UserPrefab, transform);
            user.SetActive(false);
            user.name = $"User {i}";

            var userImage = user.GetComponent<Image>();
            userImage.color = UserColors[i % UserColors.Count];

            _users.Add(new CountedUser
            {
                GameObject = user,
                LastActivity = 0f
            });
        }
    }

    void Update()
    {
        for (int i = 0; i < MaxUsers; i++)
        {
            if (!_users[i].GameObject.activeSelf) continue;

            _users[i].LastActivity += Time.deltaTime;
            if (_users[i].LastActivity > InactivityDelay) _users[i].GameObject.SetActive(false);
        }
    }

    public void UserUpdate(SensorDataMessage message)
    {
        var match = _userRegex.Match(message.Path);
        if (!match.Success) return;

        var userIndex = int.Parse(match.Captures[0].Value);
        if (userIndex < 0 || userIndex >= MaxUsers)
        {
            Debug.LogWarning($"Received invalid user index {userIndex} for user count UI");
            return;
        }

        _users[userIndex].LastActivity = 0.0f;
        if (!_users[userIndex].GameObject.activeSelf) _users[userIndex].GameObject.SetActive(true);
    }
}
