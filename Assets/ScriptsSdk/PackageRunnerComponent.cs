using System;
using System.Diagnostics;
using System.Threading;
using Assets.Extensions;
using emt_sdk.Events.Relay;
using emt_sdk.Packages;
using emt_sdk.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PackageRunnerComponent : MonoBehaviour
{
    private const int ExternalSceneKillTime = 5000; 

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private PackageDescriptor NewPackage = null;

    public Process SceneProcess;
    public EventRelayServer RelayServer;

    public void Start()
    {
        InstallRunnerAction();
    }

    protected void Update()
    {
        if (NewPackage == null) return;

        if (SceneProcess != null)
        {
            if (!SceneProcess.HasExited) return;

            RelayServer.TokenSource.Cancel();
            Logger.ErrorUnity($"Scene package exitted with code '{SceneProcess.ExitCode}'");
        }

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
                SceneManager.LoadSceneAsync("3DScene");
                break;
            case "panorama":
                SceneManager.LoadSceneAsync("PanoScene");
                break;
            case "ndi":
                SceneManager.LoadSceneAsync("NdiScene");
                break;
            case "feedback": // Only for testing
                SceneManager.LoadSceneAsync("FeedbackTestingScene");
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

    private void TerminateExternalScene()
    {
        Logger.Info("Closing scene package.");
        SceneProcess.CloseMainWindow();

        // HACK: doing it this way isn't good, but a Unity coroutine won't run if the Unity window isn't focused, so we can't use that.
        Thread.Sleep(ExternalSceneKillTime);
        if (!SceneProcess.HasExited)
        {
            Logger.Warn("Scene package did not close gracefully. Killing scene package!");
            SceneProcess.Kill();
            while (!SceneProcess.HasExited)
            {
                Thread.Sleep(1000);
            }
        }
    }

    private void RunPackage(PackageDescriptor package)
    {
        if (SceneProcess != null && !SceneProcess.HasExited) TerminateExternalScene();
        NewPackage = package;

        var packageProvider = LevelScopeServices.Instance.GetRequiredService<IConfigurationProvider<PackageDescriptor>>();
        if (packageProvider is PackageDescriptorProvider)
        {
            ((PackageDescriptorProvider)packageProvider).Configuration = package;
        }
    }

    public void StartPackage(string packageId)
    {
        LevelScopeServices.Instance.GetRequiredService<Naki3D.Common.Protocol.PackageService.PackageServiceBase>().StartPackage(new Naki3D.Common.Protocol.StartPackageRequest
        {
            PackageId = packageId
        }, null);
    }
}