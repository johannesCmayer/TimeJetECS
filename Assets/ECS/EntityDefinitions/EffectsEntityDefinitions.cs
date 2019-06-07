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

public class EffectsEntityDefinition
{
    public static EntityArchetype explosion;

    static bool isSetup;
    static EntityManager entityManager;

    public static void Setup()
    {
        if (isSetup)
            return;
        isSetup = true;

        entityManager = World.Active.EntityManager;
        
        explosion = entityManager.CreateArchetype(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(Scale)
        );
    }

    public static void SetupExplosion(float3 pos)
    {
        var explosion = entityManager.CreateEntity(EffectsEntityDefinition.explosion);

        entityManager.SetComponentData(explosion, new Translation { Value = pos });
        entityManager.SetComponentData(explosion, new Rotation { Value = quaternion.identity });
        entityManager.SetComponentData(explosion, new Scale { Value = 1 });
        //entityManager.SetComponentData(explosion, new RenderMesh { mesh });
    }
}

