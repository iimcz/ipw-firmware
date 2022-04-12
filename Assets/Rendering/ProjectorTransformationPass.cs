using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ProjectorTransformationPass : ScriptableRenderPass
{
    private string _profilerTag;
    private Material _materialToBlit;
    private Material _materialPhysicalAlignment;
    private Material _materialSoftwareCalibration;
    private RenderTargetHandle _tempTexture;

    public const int MAX_DISPLAYS = 8; // Unity limit
    public static ProjectorTransformationData[] ScreenData = new ProjectorTransformationData[MAX_DISPLAYS];
    public static float[] Saturation = new float[MAX_DISPLAYS];
    public static Color[] Brightness = new Color[MAX_DISPLAYS];
    public static float[] Contrast = new float[MAX_DISPLAYS];
    public static bool[] FlipCurve = new bool[MAX_DISPLAYS];
    public static float[] CrossOver = new float[MAX_DISPLAYS];
    public static bool Vertical = true;

    public static bool PhysicalAlignment = false;

    // SW calibration settings
    public static bool SoftwareCalibration = false;
    public static bool EnableColorRamp = false;
    public static bool EnableGeometricCorrection = true;
    public static bool EnableGammaCorrection = true;
    public static bool EnableBrightnessCorrection = true;
    public static bool EnableContrastSaturation = true;
    public static bool EnableBlending = true;

    public ProjectorTransformationPass(string profilerTag, RenderPassEvent renderPassEvent, Material materialToBlit, Material materialPhysicalAlignment, Material materialSoftwareCalibration)
    {
        this.renderPassEvent = renderPassEvent;
        _profilerTag = profilerTag;
        _materialToBlit = materialToBlit;
        _materialPhysicalAlignment = materialPhysicalAlignment;
        _materialSoftwareCalibration = materialSoftwareCalibration;

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

        if (PhysicalAlignment)
        {
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _materialPhysicalAlignment);
            cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
        }
        else
        {
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.Blit(renderingData.cameraData.targetTexture, _tempTexture.Identifier());
            cmd.SetGlobalTexture(Shader.PropertyToID("tex"), _tempTexture.Identifier());
            cmd.SetGlobalFloat(Shader.PropertyToID("contrast"), Contrast[displayNumber]);
            cmd.SetGlobalColor(Shader.PropertyToID("brightness"), Brightness[displayNumber]);
            cmd.SetGlobalFloat(Shader.PropertyToID("saturation"), Saturation[displayNumber]);
            cmd.SetGlobalFloat(Shader.PropertyToID("flipCurve"), FlipCurve[displayNumber] ? 1.0f : 0.0f);
            cmd.SetGlobalFloat(Shader.PropertyToID("crossOver"), CrossOver[0]);
            cmd.SetGlobalFloat(Shader.PropertyToID("vertical"), Vertical ? 1.0f : 0.0f);
            cmd.ClearRenderTarget(false, true, Color.black);

            var mesh = ScreenData[displayNumber].ScreenMesh;
            if (mesh == null) // Editor doesn't always call the constructor
            {
                mesh = MeshUtils.CreateTransform(new Vector3[] { new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0) });
                ScreenData[displayNumber].ScreenMesh = mesh;
            }

            if (SoftwareCalibration)
            {
                cmd.SetGlobalFloat(Shader.PropertyToID("enableColorRamp"), EnableColorRamp ? 1.0f : 0.0f);
                cmd.SetGlobalFloat(Shader.PropertyToID("enableGeometricCorrection"), EnableGeometricCorrection ? 1.0f : 0.0f);
                cmd.SetGlobalFloat(Shader.PropertyToID("enableGammaCorrection"), EnableGammaCorrection ? 1.0f : 0.0f);
                cmd.SetGlobalFloat(Shader.PropertyToID("enableBrightnessCorrection"), EnableBrightnessCorrection ? 1.0f : 0.0f);
                cmd.SetGlobalFloat(Shader.PropertyToID("enableContrastSaturation"), EnableContrastSaturation ? 1.0f : 0.0f);
                cmd.SetGlobalFloat(Shader.PropertyToID("enableBlending"), EnableBlending ? 1.0f : 0.0f);

                cmd.DrawMesh(mesh, Matrix4x4.identity, _materialSoftwareCalibration, 0, 0);
            }
            else
            {
                cmd.DrawMesh(mesh, Matrix4x4.identity, _materialToBlit, 0, 0);
            }
            cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(_tempTexture.id);
    }
}
