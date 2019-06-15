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

public class DestroyCollidingSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem simulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        simulationEntityCommandBufferSystem = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var q = GetEntityQuery(
            ComponentType.ReadOnly<SphereCollider>(),
            ComponentType.ReadOnly<Translation>(),
            ComponentType.ReadOnly<ECSLayer>()
        );
        NativeArray<Entity> entitiesWithColliders = q.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> entityColliderTranslations = q.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<SphereCollider> entityColliders = q.ToComponentDataArray<SphereCollider>(Allocator.TempJob);
        NativeArray<ECSLayer> entityLayers = q.ToComponentDataArray<ECSLayer>(Allocator.TempJob);

        var job = new DestroyJob
        {
            entitiesWithColliders = entitiesWithColliders,
            entityColliderTranslations = entityColliderTranslations,
            entityColliders = entityColliders,
            entityLayers = entityLayers,
            commandBuffer = simulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
    };
        inputDeps = job.Schedule(this, inputDeps);

        simulationEntityCommandBufferSystem.AddJobHandleForProducer(inputDeps);

        return inputDeps;
    }

    //[BurstCompile]
    struct DestroyJob : IJobForEachWithEntity<SphereCollider, Translation, ECSLayer>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> entitiesWithColliders;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> entityColliderTranslations;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<SphereCollider> entityColliders;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<ECSLayer> entityLayers;

        public void Execute(Entity entity, int index, [ReadOnly] ref SphereCollider collider, [ReadOnly] ref Translation translation, [ReadOnly] ref ECSLayer eCSLayer)
        {
            for (int i = 0; i < entityColliderTranslations.Length; i++)
            {
                bool layerCollisionSet = false;
                var collidingLayers = eCSLayer.layerID.GetCollidingLayers();
                for (int j = 0; j < collidingLayers.Length; j++)
                {
                    if (collidingLayers[j] == entityLayers[i].layerID)
                        layerCollisionSet = true;
                }

                if (entity == entitiesWithColliders[i] || !layerCollisionSet)
                    continue;

                if (math.distance(translation.Value, entityColliderTranslations[i].Value) < collider.size + entityColliders[i].size)
                {
                    commandBuffer.DestroyEntity(index, entity);
                    commandBuffer.DestroyEntity(index, entitiesWithColliders[i]);
                    EffectsEntityDefinition.SetupExplosion(commandBuffer, index, translation.Value);
                }
            }
        }
    }
}
