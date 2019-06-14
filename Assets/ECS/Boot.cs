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

public class Boot : MonoBehaviour
{
    EntityManager entityManager;

    void Start()
    {
        entityManager = World.Active.EntityManager;
        var gd = GlobalData.instance;

        var player = entityManager.Instantiate(UnitEnityDefinitions.playerPrefab);
        entityManager.SetComponentData(player, new Translation { Value = new float3(0, 0, -30) });

        var enemyPlanes = UnitEnityDefinitions.SetupPlanes(UnitEnityDefinitions.enemyPlaneArechetype, 2, gd.EnemyPlaneMesh, gd.enemyPlaneMaterial);

        for (int i = 0; i < enemyPlanes.Length; i++)
        {
            entityManager.SetComponentData(enemyPlanes[i], new TargetSelection { target = player });
            entityManager.SetComponentData(enemyPlanes[i], new RotationSpeed {Value = 3});
            entityManager.SetComponentData(enemyPlanes[i], new MoveSpeed { Value = new float3(0,0,1) });

            //var spawnPos = new float3(0, 0, 40 + i * 2);
            //UnitEnityDefinitions.SetupMissile(spawnPos, quaternion.Euler(0, 180, 0), new Velocity { Value = float3.zero }, enemyPlanes[i]);
        }
    }
}