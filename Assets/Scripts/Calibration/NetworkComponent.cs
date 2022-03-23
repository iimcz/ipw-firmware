using emt_sdk.Settings;
using TMPro;
using UnityEngine;

public class NetworkComponent : MonoBehaviour
{
    public bool ShowWarning = false;
    public bool ShowVerification = false;

    [SerializeField]
    private TMP_InputField _hostname;

    [SerializeField]
    private GameObject _warning;
    
    [SerializeField]
    private GameObject _verification;

    [SerializeField]
    private ExhibitConnectionComponent _connection;
    
    private CommunicationSettings _communication;

    private void Start()
    {
        _communication = _connection.Settings.Communication;
    }
    
    private void Update()
    {
        _warning.SetActive(ShowWarning);
        _verification.SetActive(ShowVerification);
        
        _communication.ContentHostname = _hostname.text;
        
        _hostname.Select();
        _hostname.ActivateInputField();
    }
}
