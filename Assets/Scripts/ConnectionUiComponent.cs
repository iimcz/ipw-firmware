using emt_sdk.Communication;
using TMPro;
using UnityEngine;

public class ConnectionUiComponent : MonoBehaviour
{
    public ExhibitConnectionComponent ExhibitConnection;
    public TextMeshProUGUI IPWInfo;
    
    void Update()
    {
        IPWInfo.text = $"Hostname: \t {ExhibitConnection.Hostname}\n" +
                       $"Server: \t {ExhibitConnection.Settings.Communication.ContentHostname}:{ExhibitConnection.Settings.Communication.ContentPort}\n" +
                       $"Verze serveru: \t {ExhibitConnection.Connection.ServerVersion}\n" +
                       $"Stav: {TranslateConnectionState(ExhibitConnection.Connection?.ConnectionState)}";
    }

    private string TranslateConnectionState(ConnectionStateEnum? state)
    {
        switch (state)
        {
            case ConnectionStateEnum.Disconnected:
                return "Není pøipojení";
            case ConnectionStateEnum.Connected:
                return "Pøipojeno";
            case ConnectionStateEnum.VersionCheck:
                return "Kontrola verze";
            case ConnectionStateEnum.VerifyRequest:
            case ConnectionStateEnum.VerifyWait:
                return "Ovìøování";
            case ConnectionStateEnum.Verified:
                return "Ovìøeno pøipojení";
            case ConnectionStateEnum.DescriptorSent:
                return "Odeslán deskriptor zaøízení";
            case ConnectionStateEnum.PackageInfoReceived:
                return "Balíèek pøijat, stahování...";
            default:
                return string.Empty;
        }
    }
}
