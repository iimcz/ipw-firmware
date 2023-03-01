using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavigationNodeComponent))]
[CanEditMultipleObjects]
public class NavigationNodeEditor : Editor
{
    SerializedProperty nodeType;
    SerializedProperty nextNodes;
    SerializedProperty lookList;
    
    SerializedProperty enableNavigationLines;
    SerializedProperty navigationLineMaterial;

    SerializedProperty onActivate;
    SerializedProperty onEnter;
    SerializedProperty onExit;

    void OnEnable()
    {
        nodeType = serializedObject.FindProperty("NavigationNodeType");
        nextNodes = serializedObject.FindProperty("NextNodes");
        lookList = serializedObject.FindProperty("LookList");
        
        enableNavigationLines = serializedObject.FindProperty("EnableNavigationLines");
        navigationLineMaterial = serializedObject.FindProperty("NavigationLineMaterial");

        onActivate = serializedObject.FindProperty("OnActivate");
        onEnter = serializedObject.FindProperty("OnEnter");
        onExit = serializedObject.FindProperty("OnExit");
    }

    public override void OnInspectorGUI()
    {
        var node = target as NavigationNodeComponent;

        serializedObject.Update();

        GUILayout.Label("Spawn navigation node");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Empty"))
        {
            node.NextNodes.Add(new TransitionInfo());
        }

        if (GUILayout.Button("One way"))
        {
            var obj = new GameObject("Navigation Node");
            obj.transform.localPosition = node.transform.position;

            var objComp = obj.AddComponent<NavigationNodeComponent>();
            AddTransition(node, objComp);

            node.LookList.Add(objComp);
            objComp.LookList.Add(node);
            Selection.activeGameObject = obj;
        }

        if (GUILayout.Button("Two way"))
        {
            var obj = new GameObject("Navigation Node");
            obj.transform.localPosition = node.transform.position;

            var objComp = obj.AddComponent<NavigationNodeComponent>();
            AddTransition(node, objComp);
            AddTransition(objComp, node);

            node.LookList.Add(objComp);
            objComp.LookList.Add(node);
            Selection.activeGameObject = obj;
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Spawn look node"))
        {
            var obj = new GameObject("Look Node");
            obj.transform.parent = node.transform;
            obj.transform.localPosition = Vector3.zero;

            var objComp = obj.AddComponent<LookNodeComponent>();
            objComp.ParentNode = node;

            node.LookList.Add(objComp);
            Selection.activeGameObject = obj;
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(nodeType);
        EditorGUILayout.PropertyField(nextNodes, true);
        EditorGUILayout.PropertyField(lookList);
        
        EditorGUILayout.Space();
        var oldWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 140f;
        EditorGUILayout.PropertyField(enableNavigationLines);
        if (enableNavigationLines.boolValue) EditorGUILayout.PropertyField(navigationLineMaterial, true);
        EditorGUIUtility.labelWidth = oldWidth;

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(onActivate);
        EditorGUILayout.PropertyField(onEnter);
        EditorGUILayout.PropertyField(onExit);

        if (node.NextNodes == null || node.NextNodes.Count == 0 || node.NextNodes.All(n => n == null))
        {
            EditorGUILayout.HelpBox("Node has no outgoing paths, user will be stuck in this point", MessageType.Warning);
        }
        else if (node.NextNodes.Any(n => n == null))
        {
            EditorGUILayout.HelpBox("Node has an invalid null path", MessageType.Error);
        }

        if (node.NextNodes.Any(n => n.NextNode == node))
        {
            EditorGUILayout.HelpBox("Cannot be set as a next node to itself", MessageType.Error);
        }

        if (node.NavigationNodeType == NavigationNodeComponent.NavigationNodeTypeEnum.PathPoint && node.NextNodes?.Count > 1)
        {
            EditorGUILayout.HelpBox("Too many possible nodes", MessageType.Error);
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
    public static void DrawHandles(NavigationNodeComponent t, GizmoType gizmoType)
    {
        if (t.NavigationNodeType == NavigationNodeComponent.NavigationNodeTypeEnum.StopPoint) Gizmos.color = Color.blue;
        else Gizmos.color = Color.red;

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(t.transform.position, Quaternion.identity, new Vector3(1, 0.1f, 1));
        Gizmos.DrawSphere(Vector3.zero, 0.5f);
        Gizmos.matrix = oldMatrix;

        Gizmos.color = Color.white;
        var position = t.transform.position;

        if (t.NextNodes == null) return;
        foreach (var node in t.NextNodes)
        {
            if (node == null || node.NextNode == null) continue;

            var nodePosition = node.NextNode.transform.position;
            var angle = Quaternion.LookRotation(nodePosition - position, Vector3.up);

            Gizmos.DrawLine(position, nodePosition);

            // This could be probably replaced by a gizmo but it doesn't really matter, all we lose is occlusion and hit detection
            Handles.ArrowHandleCap(0, position, angle, 1f, EventType.Repaint);
        }
    }

    private static void AddTransition(NavigationNodeComponent parent, NavigationNodeComponent target)
    {
        parent.NextNodes.Add(new TransitionInfo
        {
            NextNode = target
        });
    }
}
