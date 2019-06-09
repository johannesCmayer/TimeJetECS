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

public class UtilityEntityDefinitions
{
    public static EntityArchetype meshChild;
    static bool isSetup;
    static EntityManager entityManager;

    public static void Setup()
    {
        Debug.Log($"setup {typeof(UnitEnityDefinitions)}");
        if (isSetup)
            return;
        isSetup = true;

        entityManager = World.Active.EntityManager;
        meshChild = entityManager.CreateArchetype(
            typeof(RenderMesh),

            typeof(LocalToWorld),
            typeof(LocalToParent),
            typeof(Parent),

            typeof(Translation),
            typeof(Rotation),
            typeof(Scale)
        );
    }

    
}

