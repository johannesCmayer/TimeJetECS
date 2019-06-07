﻿using System.Collections;
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

public class TurnTowardsTargetSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        endSimulationEntityCommandBufferSystem = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var eq = GetEntityQuery(
            ComponentType.ReadOnly(typeof(Translation)),
            ComponentType.ReadOnly(typeof(Rotation)),
            ComponentType.ReadOnly(typeof(RotationSpeed)),
            ComponentType.ReadOnly(typeof(TargetSelection)),
            ComponentType.ReadOnly(typeof(TurnTowardsTarget)));
        var turnTowardTargetArray = eq.ToComponentDataArray<TargetSelection>(Allocator.TempJob);
        var targetIsValidArray = new NativeArray<bool>(turnTowardTargetArray.Length, Allocator.TempJob);
        var targetTranslations = new NativeArray<Translation>(turnTowardTargetArray.Length, Allocator.TempJob);
        for (int i = 0; i < turnTowardTargetArray.Length; i++)
        {
            var target = turnTowardTargetArray[i].target;
            targetIsValidArray[i] = World.Active.EntityManager.Exists(target) && World.Active.EntityManager.HasComponent<Translation>(target);
            if(targetIsValidArray[i])
                targetTranslations[i] = World.Active.EntityManager.GetComponentData<Translation>(target);
        }
        turnTowardTargetArray.Dispose();

        var ValidateTargets = new RemoveComponentJob
        {
            targetExists = targetIsValidArray,
            commandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        var returnDep = ValidateTargets.Schedule(this, inputDeps);

        RotateJob RotateTowardsTargets = new RotateJob
        {
            dt = Time.deltaTime,
            targetTranslations = targetTranslations,
            targetExists = targetIsValidArray
        };
         returnDep = JobHandle.CombineDependencies(RotateTowardsTargets.Schedule(this, inputDeps), returnDep);

        var deallocateJob = new DeallocateJob<bool>
        {
            toDeallocate = targetIsValidArray
        };
        returnDep = deallocateJob.Schedule(returnDep);

        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(returnDep);

        return returnDep;
    }

    [RequireComponentTag(typeof(TurnTowardsTarget), typeof(TargetSelection), typeof(Rotation), typeof(RotationSpeed))]
    struct RemoveComponentJob : IJobForEachWithEntity<Translation>
    {
        public EntityCommandBuffer.Concurrent commandBuffer;
        [ReadOnly] public NativeArray<bool> targetExists;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation trans)
        {
            if (!targetExists[index])
            {
                commandBuffer.RemoveComponent<TargetSelection>(index, entity);
            }
        }
    }

    [RequireComponentTag(typeof(TurnTowardsTarget), typeof(TargetSelection))]
    [BurstCompile]
    struct RotateJob : IJobForEachWithEntity<Translation, Rotation, RotationSpeed>
    {
        public float dt;
        [ReadOnly] public NativeArray<bool> targetExists;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> targetTranslations;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, ref Rotation rotation, [ReadOnly] ref RotationSpeed rotationSpeed)
        {
            if (!targetExists[index])
            {
                return;
            }

            var targetDir = math.normalize(targetTranslations[index].Value - translation.Value);
            var forward = rotation.Value.forward();
            var offset = (targetDir - forward) * rotationSpeed.Value * dt;
            var newForward = forward + offset;

            rotation.Value = quaternion.LookRotation(newForward, rotation.Value.up());
        }
    }
}