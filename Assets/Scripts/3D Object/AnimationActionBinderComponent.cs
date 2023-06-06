using emt_sdk.Events;
using emt_sdk.Events.Effect;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationActionBinderComponent : MonoBehaviour
{
    private class BlendShapeInfo
    {
        public static BlendShapeInfo[] FromSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            var count = skinnedMeshRenderer.sharedMesh.blendShapeCount;
            var info = new BlendShapeInfo[count];

            for (int i = 0; i < count; i++)
            {
                info[i] = new BlendShapeInfo(skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i), i, skinnedMeshRenderer);
            }

            return info;
        }

        private BlendShapeInfo(string name, int index, SkinnedMeshRenderer skinnedMeshRenderer)
        {
            Name = name;
            Index = index;
            SkinnedMeshRenderer = skinnedMeshRenderer;
        }

        public string Name { get; }
        public int Index { get; }
        public SkinnedMeshRenderer SkinnedMeshRenderer { get; }

        public float Value
        {
            get => SkinnedMeshRenderer.GetBlendShapeWeight(Index);
            set => SkinnedMeshRenderer.SetBlendShapeWeight(Index, value);
        }
    }

    public OrbitComponent Orbit { get; set; }
    public CameraHandMovement CameraHandMovement { get; set; }

    [SerializeField]
    private Animation _animation;

    private EventManager _eventManager;
    private Dictionary<string, BlendShapeInfo> _blends;

    void Start()
    {
        _eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
        _eventManager.OnEffectCalled += OnEventReceived;

        _animation = GetComponent<Animation>();

        FindBlendShapes();
    }

    void OnDestroy()
    {
        _eventManager.OnEffectCalled -= OnEventReceived;
    }

    private void OnEventReceived(EffectCall e)
    {
        if (_animation != null && e.Name == "animate")
        {
            StartCoroutine(StartAnimation(e.String));
        }
        else if (e.Name.StartsWith("blendShape"))
        {
            var blendShapeName = e.Name.Split('/').Last();
            if (!_blends.TryGetValue(blendShapeName, out var blendShape)) return;
            blendShape.Value = e.Float;
        }
    }

    private void FindBlendShapes()
    {
        var skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        _blends = skinnedMeshes.SelectMany(sm => BlendShapeInfo.FromSkinnedMeshRenderer(sm)).ToDictionary(i => i.Name);
    }

    private IEnumerator StartAnimation(string animationName)
    {
        if (Orbit.Resetting) yield break;

        yield return Orbit.ResetToDefaultPosition();
        CameraHandMovement.UnZoom();

        yield return new WaitForSecondsRealtime(0.5f);
        _animation.Play(animationName);
    }
}
