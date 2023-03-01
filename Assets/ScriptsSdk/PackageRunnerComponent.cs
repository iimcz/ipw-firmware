using System;
using System.Threading.Tasks;
using Assets.Extensions;
using emt_sdk.Events.Relay;
using emt_sdk.Packages;
using emt_sdk.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PackageRunnerComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private PackageDescriptor NewPackage = null;

    public void Start()
    {
        InstallRunnerAction();
    }

    public void Update()
    {
        if (NewPackage == null) return;

        switch (NewPackage.Parameters.DisplayType)
        {
            case "video":
                SceneManager.LoadSceneAsync("VideoScene");
                break;
            case "gallery":
                SceneManager.LoadSceneAsync("GalleryScene");
                break;
            case "model":
                SceneManager.LoadSceneAsync("3DObject");
                break;
            case "scene":
                {
                    var relayServer = LevelScopeServices.Instance.GetRequiredService<EventRelayServer>();
                    Task.Run(() => relayServer.Listen());

                    // TODO: We want to wait for the TCP listener to initialize, but this is horrible
                    System.Threading.Thread.Sleep(500);

                    var process = NewPackage.Run();
                    process.WaitForExit();
                    relayServer.TokenSource.Cancel();

                    Logger.ErrorUnity($"Scene package exitted with code '{process.ExitCode}', this probably shouldn't happen");
                    break;
                }
            case "panorama":
                SceneManager.LoadSceneAsync("PanoScene");
                break;
            default:
                Logger.ErrorUnity($"Package display type '{NewPackage.Parameters.DisplayType}' is not implemented");
                throw new NotImplementedException($"Package display type '{NewPackage.Parameters.DisplayType}' is not implemented");
        }
        NewPackage = null;
    }

    public void OnApplicationQuit()
    {
        InstallRunnerAction(true);
    }

    private void InstallRunnerAction(bool uninstall = false)
    {
        var runner = LevelScopeServices.Instance.GetRequiredService<IPackageRunner>();

        if (!(runner is PackageRunnerProxy))
        {
            Logger.ErrorUnity("This component can only work with the PackageRunnerProxy provider for the IPackageRunner service!");
        }

        if (uninstall)
        {
            ((PackageRunnerProxy)runner).PackageRunAction = null;
        }
        else
        {
            ((PackageRunnerProxy)runner).PackageRunAction = RunPackage;
        }
    }

    private void RunPackage(PackageDescriptor package)
    {
        NewPackage = package;

        var packageProvider = LevelScopeServices.Instance.GetRequiredService<IConfigurationProvider<PackageDescriptor>>();
        if (packageProvider is PackageDescriptorProvider)
        {
            ((PackageDescriptorProvider)packageProvider).Configuration = package;
        }
    }
}