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

    List<Camera> cameras;
    public RenderTexture[] renderTextures;

    Texture2D dummyTex;

    void Start()
    {
        dummyTex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        cameras = new List<Camera>() { forward, left, right, up, down, back };

        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].targetTexture = renderTextures[i];
            blitMaterial.SetTexture($"_RenderTex{i}", renderTextures[i]);
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        
        Graphics.Blit(dummyTex, dest, blitMaterial);
    }
}
