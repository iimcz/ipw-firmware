using System;
using TMPro;
using UnityEngine;

public class ConnectionUiComponent : MonoBehaviour
{
    public ExhibitConnectionComponent ExhibitConnection;

    public TextMeshProUGUI IPWInfo;
    public TextMeshProUGUI ServerInfo;

    private Version _version;

    void Start()
    {
        _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
    }

    // Update is called once per frame
    void Update()
    {
        IPWInfo.text = $"IPW Ver. {_version}\nHostname: {ExhibitConnection.Hostname}\nTarget: {ExhibitConnection.TargetServer}";
        ServerInfo.text = $"Server Ver. \nConnection: {ExhibitConnection.Connection?.IsConnected ?? false}\nProgress:";
    }
}
