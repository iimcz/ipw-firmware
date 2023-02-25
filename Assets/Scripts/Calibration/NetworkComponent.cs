using emt_sdk.Settings;
using emt_sdk.Settings.EMT;
using Microsoft.Extensions.DependencyInjection;
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

    private IConfigurationProvider<EMTSetting> _configProvider;

    private void Start()
    {
        _configProvider = GlobalServices.Instance.GetRequiredService<IConfigurationProvider<EMTSetting>>();
    }
    
    private void Update()
    {
        _warning.SetActive(ShowWarning);
        _verification.SetActive(ShowVerification);

        _configProvider.Configuration.Communication.ContentHostname = _hostname.text;

        _hostname.Select();
        _hostname.ActivateInputField();
    }
}
