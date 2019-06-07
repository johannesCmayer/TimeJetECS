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

public class Setup : MonoBehaviour
{
    public static System.Action AwakeHook = delegate { };
    public static System.Action StartHook = delegate { };

    private void Awake()
    {
        UnitEnityDefinitions.Setup();
        EffectsEntityDefinition.Setup();
        AwakeHook();
    }

    void Start()
    {
        StartHook();
    }
}
