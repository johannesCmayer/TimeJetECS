using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using R = UnityEngine.Random;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class RenderTest : MonoBehaviour
{
    public Material blitMaterial;

    [Header("Cameras")]
    public Camera forward;
    public Camera left;
    public Camera right;
    public Camera up;
    public Camera down;
    public Camera back;

    int2[] resolutions = new int2[] {
        new int2(512, 512) * 1,
        new int2(512, 512) * 2,
        new int2(512, 512) * 2,
        new int2(512, 512) * 2,
        new int2(512, 512) * 2,
        new int2(512, 512) * 2,};

    Camera[] cameras;
    RenderTexture[] renderTextures = new RenderTexture[6];

    Texture2D dummyTex;

    void Start()
    {
        dummyTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        cameras = new Camera[] { forward, left, right, up, down, back };

        for (int i = 0; i < cameras.Length; i++)
        {
            renderTextures[i] = new RenderTexture(resolutions[i].x, resolutions[i].y, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
            renderTextures[i].filterMode = FilterMode.Point;
            if (!renderTextures[i].IsCreated())
                renderTextures[i].Create();
            cameras[i].targetTexture = renderTextures[i];
            blitMaterial.SetTexture($"_RenderTex{i}", renderTextures[i]);
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(dummyTex, dest, blitMaterial);
    }
}
