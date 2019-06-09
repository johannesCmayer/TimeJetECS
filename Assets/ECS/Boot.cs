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
        print($"eman {entityManager}");
        var gd = GlobalData.instance;
        print($"global data {gd}");

        var player = UnitEnityDefinitions.SetupPlanes(UnitEnityDefinitions.playerPlaneArechetype, 1, gd.friendlyPlaneMesh, gd.friendlyPlaneMaterial)[0];
        //var enemyPlanes = UnitEnityDefinitions.SetupPlanes(UnitEnityDefinitions.enemyPlaneArechetype, 100, gd.EnemyPlaneMesh, gd.enemyPlaneMaterial);

        //for (int i = 0; i < enemyPlanes.Length; i++)
        //{
        //    var spawnPos = new float3(R.Range(-100, 100), R.Range(-100, 100), R.Range(-100, 100)) + new float3(0, 0, 300);
        //    UnitEnityDefinitions.SetupMissile(spawnPos, quaternion.identity, new Velocity { Value = float3.zero }, enemyPlanes[i]);
        //}
    }
}