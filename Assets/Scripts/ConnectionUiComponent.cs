using System;
using TMPro;
using UnityEngine;

public class ConnectionUiComponent : MonoBehaviour
{
    public ExhibitConnectionComponent ExhibitConnection;
    public TextMeshProUGUI IPWInfo;
    
    void Update()
    {
        IPWInfo.text = $"Hostname \t {ExhibitConnection.Hostname}\n" +
                       $"Target \t {ExhibitConnection.TargetServer}:{ExhibitConnection.Port}\n" +
                       $"Server Ver \t {ExhibitConnection.Connection}\n" +
                       $"Connection: {ExhibitConnection.Connection?.IsConnected ?? false}\n" +
                       $"Progress: {ExhibitConnection.Connection?.ConnectionState}";
    }
}
