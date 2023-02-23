using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using emt_sdk.Communication;
using TMPro;
using UnityEngine;

public class ConnectionUiComponent : MonoBehaviour
{
    public ExhibitConnectionComponent ExhibitConnection;
    public TextMeshProUGUI IPWInfo;
    
    public string LocalIps = string.Join("\n", GetAllLocalIPv4());
    
    void Update()
    {
        IPWInfo.text = $"Hostname: \t {ExhibitConnection.Hostname}\n" +
                       $"Server: \t {ExhibitConnection.Settings.Communication.ContentHostname}:{ExhibitConnection.Settings.Communication.ContentPort}\n" +
                    //    $"Verze serveru: \t {ExhibitConnectionComponent.Connection?.ServerVersion}\n" +
                    //    $"Stav: {TranslateConnectionState(ExhibitConnectionComponent.Connection?.ConnectionState)}\n" +
                       $"IP:\n{LocalIps}";
    }

    // private string TranslateConnectionState(ConnectionStateEnum? state)
    // {
    //     switch (state)
    //     {
    //         case ConnectionStateEnum.Disconnected:
    //             return "Není připojení";
    //         case ConnectionStateEnum.Connected:
    //             return "Připojeno";
    //         case ConnectionStateEnum.VersionCheck:
    //             return "Kontrola verze";
    //         case ConnectionStateEnum.VerifyRequest:
    //         case ConnectionStateEnum.VerifyWait:
    //             return "Ověřování";
    //         case ConnectionStateEnum.Verified:
    //             return "Ověřeno připojení";
    //         case ConnectionStateEnum.DescriptorSent:
    //             return "Odeslán deskriptor zařízení";
    //         case ConnectionStateEnum.PackageInfoReceived:
    //             return "Balíček přijat, stahování...";
    //         default:
    //             return string.Empty;
    //     }
    // }
    
    private static string[] GetAllLocalIPv4()
    {
        List<string> ipAddrList = new List<string>();
        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.OperationalStatus == OperationalStatus.Up && item.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork || ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        ipAddrList.Add($"{ip.Address}\n\t{item.GetPhysicalAddress()}");
                    }
                }
            }
        }
        return ipAddrList.ToArray();
    }
}
