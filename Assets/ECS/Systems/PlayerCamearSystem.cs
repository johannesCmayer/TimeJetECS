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

//[UpdateAfter(typeof(MovementSystem))]
public class PlayerCamearSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerTag>().ForEach((ref Translation translation, ref Rotation rotation) => {
            var forward = rotation.Value.forward();
            var up = rotation.Value.up();
            var left = rotation.Value.left();
            // var camOffset = up + forward * -3 + left;
            var camOffset = float3.zero;

            var camTrans = Camera.main.transform;
            camTrans.position = translation.Value +  camOffset;
            camTrans.rotation = rotation.Value;
        });
    }
}
