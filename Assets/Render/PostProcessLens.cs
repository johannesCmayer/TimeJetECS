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
    [Header("Render Texure Inputs")]
    public TextureParameter forward = new TextureParameter();
    public TextureParameter left = new TextureParameter();
    public TextureParameter right = new TextureParameter();
    public TextureParameter up = new TextureParameter();
    public TextureParameter down = new TextureParameter();
    public TextureParameter back = new TextureParameter();

    [Header("Parameters")]
    public FloatParameter zoom = new FloatParameter { value = 2 };
    public FloatParameter lenseStretchX = new FloatParameter { value = 0 };
    public FloatParameter lenseStretchY = new FloatParameter { value = 0 };

    [Header("Debug")]
    public BoolParameter showDebugGrid = new BoolParameter { value = false };
    public FloatParameter debugGridOpacity = new FloatParameter { value = 0.2f };
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
    

    internal Texture2D debugGridTexture = new Texture2D(14, 14);

    RenderTexture[] renderTextures = new RenderTexture[6];

    RenderTargetIdentifier emptyTargetIdentifier = new RenderTargetIdentifier();

    public override void Init()
    {
        for (int i = 0; i < debugGridTexture.width; i++)
        {
            for (int j = 0; j < debugGridTexture.height; j++)
            {
                Color col = new Color(1, 1, 1, 1);
                if ((i+1) % 3 == 0 || (j+1) % 3 == 0)
                    col = new Color(0, 0, 0, 0);
                debugGridTexture.SetPixel(i, j, col);
            }
        }
        debugGridTexture.Apply();
        debugGridTexture.filterMode = FilterMode.Point;

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
        lensShader.properties.SetFloat("_LenseStretchX", settings.lenseStretchX);
        lensShader.properties.SetFloat("_LenseStretchY", settings.lenseStretchY);
        lensShader.properties.SetFloat("_AspectRatio", (float)Screen.width / (float)Screen.height);
        lensShader.properties.SetFloat("_DebugColCoef", settings.showDebugGrid.value ? settings.debugGridOpacity : 0f);
        lensShader.properties.SetTexture("_DebugProjectionTexAlpha", debugGridTexture);

        lensShader.properties.SetFloat("_Time", Time.realtimeSinceStartup);

        context.command.BlitFullscreenTriangle(emptyTargetIdentifier, context.destination, lensShader, 0);
    }
}
