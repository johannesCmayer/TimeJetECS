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
            ComponentType.ReadOnly<Translation>()
        );
        NativeArray<Entity> entitiesWithColliders = q.ToEntityArray(Allocator.TempJob);
        NativeArray<Translation> entityColliderTranslations = q.ToComponentDataArray<Translation>(Allocator.TempJob);
        NativeArray<SphereCollider> entityColliders = q.ToComponentDataArray<SphereCollider>(Allocator.TempJob);

        var job = new DestroyJob
        {
            entitiesWithColliders = entitiesWithColliders,
            entityColliderTranslations = entityColliderTranslations,
            entityColliders = entityColliders,
            commandBuffer = simulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
    };
        inputDeps = job.Schedule(this, inputDeps);

        simulationEntityCommandBufferSystem.AddJobHandleForProducer(inputDeps);

        return inputDeps;
    }

    [BurstCompile]
    struct DestroyJob : IJobForEachWithEntity<SphereCollider, Translation>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> entitiesWithColliders;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> entityColliderTranslations;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<SphereCollider> entityColliders;
        

        public void Execute(Entity entity, int index, [ReadOnly] ref SphereCollider collider, [ReadOnly] ref Translation tarnslation)
        {
            for (int i = 0; i < entityColliderTranslations.Length; i++)
            {
                if (entity == entitiesWithColliders[i])
                    continue;

                if (math.distance(tarnslation.Value, entityColliderTranslations[i].Value) < collider.size + entityColliders[i].size)
                {
                    commandBuffer.DestroyEntity(index, entity);
                    commandBuffer.DestroyEntity(index, entitiesWithColliders[i]);
                }
            }
        }
    }
}
