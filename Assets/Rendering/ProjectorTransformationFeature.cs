using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ProjectorTransformationFeature : ScriptableRendererFeature
{
    [Serializable]
    public class ProjectorTransformationSettings
    {
        public bool IsEnabled = true;
        public RenderPassEvent WhenToInsert = RenderPassEvent.AfterRendering;
        public Material MaterialToBlit;
        public Material MaterialPhysicalCal;
        public Material MaterialSoftCal;
    }

    public ProjectorTransformationSettings settings = new ProjectorTransformationSettings();

    private ProjectorTransformationPass _pass;

    public override void Create()
    {
        _pass = new ProjectorTransformationPass(
          "Projector transformation",
          settings.WhenToInsert,
          settings.MaterialToBlit,
          settings.MaterialPhysicalCal,
          settings.MaterialSoftCal
        );
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!settings.IsEnabled) return;
        renderer.EnqueuePass(_pass);
    }
}
