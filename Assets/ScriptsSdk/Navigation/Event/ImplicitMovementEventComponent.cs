using Naki3D.Common.Protocol;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ImplicitMovementEventComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    [SerializeField]
    private RelayClientComponent _relayClient;

    [SerializeField]
    private NodeNavigatorComponent _nodeNavigator;

    private void Awake()
    {
        var navigationNodes = FindObjectsOfType<NavigationNodeComponent>();
        Logger.Info($"Found {navigationNodes.Length} navigation nodes, adding implicit movement events");

        foreach (var node in navigationNodes)
        {
            node.OnActivate.AddListener(() => SendMovementEvent("NodeActivate", node));
            node.OnEnter.AddListener(() => SendMovementEvent("NodeEnter", node));
            node.OnExit.AddListener(() => SendMovementEvent("NodeExit", node));
        }

        var lookNodes = FindObjectsOfType<LookNodeComponent>();
        Logger.Info($"Found {lookNodes.Length} look nodes, adding implicit activation events");

        foreach (var lookNode in lookNodes)
        {
            lookNode.OnActivate.AddListener(() => SendMovementEvent("NodeActivate", lookNode));
        }

        _nodeNavigator.OnTransitionStart.AddListener(node => SendMovementEvent("NavigatorTransitionStart", node));
        _nodeNavigator.OnTransitionEnd.AddListener(node => SendMovementEvent("NavigatorTransitionEnd", node));
    }

    private void SendMovementEvent(string movementEvent, MonoBehaviour node)
    {
        var message = new SensorMessage
        {
            SensorId = node.gameObject.name,
            Event = new EventData
            {
                Name = movementEvent
            }
        };

        _relayClient.BroadcastEvent(message);
    }
}
