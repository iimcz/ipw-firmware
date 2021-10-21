using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ProjectorTransformationPass : ScriptableRenderPass
{
    private string _profilerTag;
    private Material _materialToBlit;
    private RenderTargetHandle _tempTexture;

    public const int MAX_DISPLAYS = 8; // Unity limit
    public static Mesh[] ScreenMeshes = new Mesh[MAX_DISPLAYS];
    public static float[] Saturation = new float[MAX_DISPLAYS];
    public static float[] Brightness = new float[MAX_DISPLAYS];
    public static float[] Contrast = new float[MAX_DISPLAYS];
    public static bool[] FlipCurve = new bool[MAX_DISPLAYS];
    public static float[] CrossOver = new float[MAX_DISPLAYS];
    public static bool EnableCurve = true;

    public static Mesh CreateTransform(Vector3[] v)
    {
        var mesh = new Mesh
        {
            vertices = v,
            triangles = new int[] { 0, 1, 2, 0, 2, 3 }
        };

        var shiftedPositions = new Vector2[] { Vector2.zero, new Vector2(0, v[1].y - v[0].y), new Vector2(v[2].x - v[1].x, v[2].y - v[3].y), new Vector2(v[3].x - v[0].x, 0) };
        mesh.uv = shiftedPositions;

        var widthsHeights = new Vector2[4];
        widthsHeights[0].x = widthsHeights[3].x = shiftedPositions[3].x;
        widthsHeights[1].x = widthsHeights[2].x = shiftedPositions[2].x;
        widthsHeights[0].y = widthsHeights[1].y = shiftedPositions[1].y;
        widthsHeights[2].y = widthsHeights[3].y = shiftedPositions[2].y;
        mesh.uv2 = widthsHeights;

        mesh.UploadMeshData(false);
        return mesh;
    }

    public ProjectorTransformationPass(string profilerTag, RenderPassEvent renderPassEvent, Material materialToBlit)
    {
        this.renderPassEvent = renderPassEvent;
        _profilerTag = profilerTag;
        _materialToBlit = materialToBlit;

        ScreenMeshes[0] = CreateTransform(new Vector3[] { new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0) });
        Saturation[0] = 1f;
        Brightness[0] = 0f;
        Contrast[0] = 1f;
        CrossOver[0] = 0.05f;

        for (int i = 1; i < MAX_DISPLAYS; i++)
        {
            ScreenMeshes[i] = Object.Instantiate(ScreenMeshes[0]);
            Saturation[i] = Saturation[0];
            Brightness[i] = Brightness[0];
            Contrast[i] = Contrast[0];
        }
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

        var mesh = ScreenMeshes[displayNumber];
        if (mesh == null) // Editor doesn't always call the constructor
        {
            mesh = CreateTransform(new Vector3[] { new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0) });
            ScreenMeshes[displayNumber] = mesh;
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
