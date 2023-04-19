using System.Collections;
using System.Threading.Tasks;
using emt_sdk.Events.Relay;
using emt_sdk.Packages;
using emt_sdk.Settings;
using UnityEngine;

public class SceneHandlerComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public PackageRunnerComponent RunnerComponent;

    private void Start()
    {
        StartCoroutine(DelayApply());
    }

    public IEnumerator DelayApply()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        var NewPackage = LevelScopeServices.Instance.GetRequiredService<IConfigurationProvider<PackageDescriptor>>().Configuration;
        RunnerComponent.RelayServer = LevelScopeServices.Instance.GetRequiredService<EventRelayServer>();
        Task.Run(() => RunnerComponent.RelayServer.Listen());
        yield return new WaitForSecondsRealtime(0.5f);
        Logger.Info($"Running package '{NewPackage.Metadata.Title}'");
        RunnerComponent.SceneProcess = NewPackage.Run();
    }
}