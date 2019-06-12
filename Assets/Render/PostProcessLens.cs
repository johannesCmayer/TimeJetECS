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
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;

[System.Serializable]
public enum Lens
{
    FishEye2,
}

[System.Serializable]
[PostProcess(typeof(PostProcessLensRenderer), PostProcessEvent.BeforeStack, "LensProjection")]
public sealed class PostProcessLens : PostProcessEffectSettings
{
    public Lens lens = Lens.FishEye2;

    public FloatParameter zoom = new FloatParameter { value = 2 };
    public FloatParameter imageScale = new FloatParameter { value = 1 };
    public FloatParameter debugGridOpacity = new FloatParameter { value = 0 };

    [Header("Render Texures")]
    public TextureParameter forward = new TextureParameter();
    public TextureParameter left = new TextureParameter();
    public TextureParameter right = new TextureParameter();
    public TextureParameter up = new TextureParameter();
    public TextureParameter down = new TextureParameter();
    public TextureParameter back = new TextureParameter();
}

public sealed class PostProcessLensRenderer : PostProcessEffectRenderer<PostProcessLens>
{
    //public int2[] resolutions = new int2[] {
    //    new int2(512, 512) * 1,
    //    new int2(512, 512) * 2,
    //    new int2(512, 512) * 2,
    //    new int2(512, 512) * 2,
    //    new int2(512, 512) * 2,
    //    new int2(512, 512) * 2,};

    RenderTexture[] renderTextures = new RenderTexture[6];

    RenderTargetIdentifier emptyTargetIdentifier = new RenderTargetIdentifier();

    public override void Init()
    {
        renderTextures = new RenderTexture[] {
            (RenderTexture)settings.forward.value,
            (RenderTexture)settings.left,
            (RenderTexture)settings.right,
            (RenderTexture)settings.up,
            (RenderTexture)settings.down,
            (RenderTexture)settings.back
        };
    }

    public override void Render(PostProcessRenderContext context)
    {
        var lensShader = context.propertySheets.Get(Shader.Find("Lens/Post"));

        for (int i = 0; i < renderTextures.Length; i++)
        {
            if (renderTextures[i] == null)
                Debug.LogError($"All render textures must be set for the '{this}'");
            lensShader.properties.SetTexture($"_RenderTex{i}", renderTextures[i]);
        }
        lensShader.properties.SetFloat("_Zoom", settings.zoom);
        lensShader.properties.SetFloat("_DebugColCoef", settings.debugGridOpacity);
        lensShader.properties.SetFloat("_ImgScale", settings.imageScale);

        context.command.BlitFullscreenTriangle(emptyTargetIdentifier, context.destination, lensShader, 0);
    }
}
