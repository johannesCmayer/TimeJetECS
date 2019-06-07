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
    public static EntityArchetype explosionArchetype;
    public static EntityArchetype trailArchetype;

    static bool isSetup;
    static EntityManager entityManager;
    
    public static void Setup()
    {
        if (isSetup)
            return;
        isSetup = true;

        entityManager = World.Active.EntityManager;

        ExplosionArchetypeSetup();
        TrailArchetypeSetup();
    }

    static void ExplosionArchetypeSetup()
    {
        explosionArchetype = entityManager.CreateArchetype(
            typeof(ExplosionTag),

            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),

            typeof(DestroyAfterTime)
        );
    }

    public static void SetupExplosion(EntityCommandBuffer commandBuffer, float3 pos)
    {
        var explosion = commandBuffer.CreateEntity(explosionArchetype);

        commandBuffer.SetComponent(explosion, new Translation { Value = pos });
        commandBuffer.SetComponent(explosion, new Rotation { Value = quaternion.identity });
        commandBuffer.SetComponent(explosion, new Scale { Value = 1 });
        commandBuffer.SetSharedComponent(explosion, new RenderMesh {
            mesh = GlobalData.instance.explosionMesh,
            material = GlobalData.instance.explosionMaterial
        });
        commandBuffer.SetComponent(explosion, new DestroyAfterTime { timeToDestruction = 0.5f });
    }

    public static void SetupExplosion(EntityCommandBuffer.Concurrent commandBuffer, int idx, float3 pos)
    {
        var explosion = commandBuffer.CreateEntity(idx, explosionArchetype);

        commandBuffer.SetComponent(idx, explosion, new Translation { Value = pos });
        commandBuffer.SetComponent(idx, explosion, new Rotation { Value = quaternion.identity });
        commandBuffer.SetComponent(idx, explosion, new Scale { Value = 1 });
        commandBuffer.SetSharedComponent(idx, explosion, new RenderMesh
        {
            mesh = GlobalData.instance.explosionMesh,
            material = GlobalData.instance.explosionMaterial
        });
        commandBuffer.SetComponent(idx, explosion, new DestroyAfterTime { timeToDestruction = 0.5f });
    }

    static void TrailArchetypeSetup()
    {
        trailArchetype = entityManager.CreateArchetype(
            typeof(RenderMesh),
            typeof(LocalToWorld),
            typeof(Translation),
            typeof(Rotation),
            typeof(Scale),

            typeof(DestroyAfterTime)
        );
    }

    public static void SetupTrail(EntityCommandBuffer commandBuffer, float3 pos, float duration = 2)
    {
        var explosion = commandBuffer.CreateEntity(trailArchetype);
        SetupTrail(commandBuffer, explosion, pos, duration);
    }

    public static void SetupTrail(Entity trail, float3 pos, float duration = 2)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer { ShouldPlayback = true, MinimumChunkSize = 1000 };
        SetupTrail(commandBuffer, trail, pos, duration);
        commandBuffer.Playback(entityManager);
    }

    public static void SetupTrail(EntityCommandBuffer commandBuffer, Entity trail, float3 pos, float duration = 2)
    {
        commandBuffer.SetComponent(trail, new Translation { Value = pos });
        commandBuffer.SetComponent(trail, new Rotation { Value = quaternion.identity });
        commandBuffer.SetComponent(trail, new Scale { Value = 0.2f });
        commandBuffer.SetSharedComponent(trail, new RenderMesh
        {
            mesh = GlobalData.instance.missileTrailMesh,
            material = GlobalData.instance.missileTrailMaterial
        });
        commandBuffer.SetComponent(trail, new DestroyAfterTime { timeToDestruction = duration });
    }

    public static void SetupTrailConcurrent(EntityCommandBuffer.Concurrent commandBuffer, int idx, float3 pos, float duration = 2)
    {
        var explosion = commandBuffer.CreateEntity(idx, trailArchetype);
        commandBuffer.SetComponent(idx, explosion, new Translation { Value = pos });
        commandBuffer.SetComponent(idx, explosion, new Rotation { Value = quaternion.identity });
        commandBuffer.SetComponent(idx, explosion, new Scale { Value = 1 });
        commandBuffer.SetSharedComponent(idx, explosion, new RenderMesh
        {
            mesh = GlobalData.instance.missileTrailMesh,
            material = GlobalData.instance.missileTrailMaterial
        });
        commandBuffer.SetComponent(idx, explosion, new DestroyAfterTime { timeToDestruction = duration });
    }
}

