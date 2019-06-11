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

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateBefore(typeof(RenderMeshSystemV2))]
public class PlayerCamearSystem : ComponentSystem
{
    public GameObject playerCamRoot;

    protected override void OnCreate()
    {
        playerCamRoot = GameObject.FindGameObjectWithTag("PlayerCameraRoot");
    }

    protected override void OnUpdate()
    {
        playerCamRoot = GameObject.FindGameObjectWithTag("PlayerCameraRoot");

        Entities.WithAll<PlayerTag>().ForEach((ref Translation translation, ref Rotation rotation) => {
            var forward = rotation.Value.forward();
            var up = rotation.Value.up();
            var left = rotation.Value.right();
            //var camOffset = up + forward * -3 + left * 0;
            var camOffset = float3.zero;

            var camTrans = playerCamRoot.transform;
            camTrans.rotation = rotation.Value;
            camTrans.position = translation.Value +  camOffset;
        });
    }
}
