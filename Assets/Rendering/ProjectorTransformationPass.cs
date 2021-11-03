using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ProjectorTransformationPass : ScriptableRenderPass
{
    private string _profilerTag;
    private Material _materialToBlit;
    private RenderTargetHandle _tempTexture;

    public const int MAX_DISPLAYS = 8; // Unity limit
    public static ProjectorTransformationData[] ScreenData = new ProjectorTransformationData[MAX_DISPLAYS];
    public static float[] Saturation = new float[MAX_DISPLAYS];
    public static float[] Brightness = new float[MAX_DISPLAYS];
    public static float[] Contrast = new float[MAX_DISPLAYS];
    public static bool[] FlipCurve = new bool[MAX_DISPLAYS];
    public static float[] CrossOver = new float[MAX_DISPLAYS];
    public static bool EnableCurve = true;

    public ProjectorTransformationPass(string profilerTag, RenderPassEvent renderPassEvent, Material materialToBlit)
    {
        this.renderPassEvent = renderPassEvent;
        _profilerTag = profilerTag;
        _materialToBlit = materialToBlit;

        for (int i = 0; i < MAX_DISPLAYS; i++) ScreenData[i] = new ProjectorTransformationData();
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        cmd.GetTemporaryRT(_tempTexture.id, cameraTextureDescriptor);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        Camera camera = renderingData.cameraData.camera;
        CommandBuffer cmd = CommandBufferPool.Get(_profilerTag);
        var displayNumber = renderingData.cameraData.camera.targetDisplay;

        cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
        cmd.Blit(renderingData.cameraData.targetTexture, _tempTexture.Identifier());
        cmd.SetGlobalTexture(Shader.PropertyToID("tex"), _tempTexture.Identifier());
        cmd.SetGlobalFloat(Shader.PropertyToID("contrast"), Contrast[displayNumber]);
        cmd.SetGlobalFloat(Shader.PropertyToID("brightness"), Brightness[displayNumber]);
        cmd.SetGlobalFloat(Shader.PropertyToID("saturation"), Saturation[displayNumber]);
        cmd.SetGlobalFloat(Shader.PropertyToID("flipCurve"), FlipCurve[displayNumber] ? 1.0f : 0.0f);
        cmd.SetGlobalFloat(Shader.PropertyToID("enableCurve"), EnableCurve ? 1.0f : 0.0f);
        cmd.SetGlobalFloat(Shader.PropertyToID("crossOver"), CrossOver[displayNumber]);
        cmd.ClearRenderTarget(false, true, Color.black);

        var mesh = ScreenData[displayNumber].ScreenMesh;
        if (mesh == null) // Editor doesn't always call the constructor
        {
            mesh = MeshUtils.CreateTransform(new Vector3[] { new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0) });
            ScreenData[displayNumber].ScreenMesh = mesh;
        }

        cmd.DrawMesh(mesh, Matrix4x4.identity, _materialToBlit, 0, 0);
        cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(_tempTexture.id);
    }
}
