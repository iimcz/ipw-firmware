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
                return "Nen� p�ipojen�";
            case ConnectionStateEnum.Connected:
                return "P�ipojeno";
            case ConnectionStateEnum.VersionCheck:
                return "Kontrola verze";
            case ConnectionStateEnum.VerifyRequest:
            case ConnectionStateEnum.VerifyWait:
                return "Ov��ov�n�";
            case ConnectionStateEnum.Verified:
                return "Ov��eno p�ipojen�";
            case ConnectionStateEnum.DescriptorSent:
                return "Odesl�n deskriptor za��zen�";
            case ConnectionStateEnum.PackageInfoReceived:
                return "Bal��ek p�ijat, stahov�n�...";
            default:
                return string.Empty;
        }
    }
}
