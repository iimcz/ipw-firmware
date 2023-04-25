using System;
using System.Threading.Tasks;
using emt_sdk.Communication.Discovery;
using emt_sdk.Communication.Exhibit;
using emt_sdk.Events;
using emt_sdk.Events.Local;
using emt_sdk.Packages;
using emt_sdk.Settings;
using UnityEngine;

public class ServiceStarterComponent : MonoBehaviour
{
    public bool StartGlobalServices = false;

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public void Awake()
    {
        if (StartGlobalServices)
        {
            var sensorManager = GlobalServices.Instance.GetRequiredService<ISensorManager>();
            Task.Run(() => sensorManager.Start());

            var discovery = GlobalServices.Instance.GetRequiredService<IDiscoveryService>();
            if (!discovery.IsBroadcasting) discovery.StartBroadcast();

            var grpc = GlobalServices.Instance.GetRequiredService<GrpcServer>();
            grpc.Start();
        }
        else
        {
            var eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
            eventManager.ConnectSensor();
            if (!eventManager.ConnectedRemote)
            {
                eventManager.ConnectRemote();
            }

            var package = LevelScopeServices.Instance.GetRequiredService<IConfigurationProvider<PackageDescriptor>>().Configuration;
            eventManager.Actions.Clear();
            eventManager.Actions.AddRange(package.Inputs);

            // TODO: global event logging

            // TODO: does this need to be a delayed coroutine?
        }
    }

    public void OnApplicationQuit()
    {
        if (StartGlobalServices)
        {
            var sensorManager = GlobalServices.Instance.GetRequiredService<ISensorManager>();
            sensorManager.Stop();

            var discovery = GlobalServices.Instance.GetRequiredService<IDiscoveryService>();
            if (discovery.IsBroadcasting) discovery.StopBroadcast();

            var grpc = GlobalServices.Instance.GetRequiredService<GrpcServer>();
            grpc.ShutdownAsync().Wait(TimeSpan.FromSeconds(5)); // TODO: handle this better?
        }
    }
}