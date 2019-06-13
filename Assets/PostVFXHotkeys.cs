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

[ExecuteInEditMode]
public class PostVFXHotkeys : MonoBehaviour
{
    PostProcessVolume myVol;
    PostProcessLens lens;

    void Start()
    {
        myVol = GetComponent<PostProcessVolume>();
        myVol.profile.TryGetSettings<PostProcessLens>(out lens);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            lens.showDebugGrid.value = !lens.showDebugGrid.value;
        if (Input.GetKey(KeyCode.F2))
            lens.zoom.value += 0.2f;
        if (Input.GetKey(KeyCode.F3))
            lens.zoom.value -= 0.2f;
    }
}
