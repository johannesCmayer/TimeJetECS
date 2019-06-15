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

public enum ECSLayerID
{
    NotSet,
    Ship,
    Projectiles,
    Missiles,
}

public static class ECSLayerExtensions
{
    public static ECSLayerID[] GetCollidingLayers(this ECSLayerID layer)
    {
        switch (layer)
        {
            case ECSLayerID.NotSet:
                throw new System.Exception("The layer of the entity is not set.");
            case ECSLayerID.Ship:
                return new ECSLayerID[]
                {
                    ECSLayerID.Projectiles,
                    ECSLayerID.Missiles
                };
            case ECSLayerID.Projectiles:
                return new ECSLayerID[]
                {
                };
            case ECSLayerID.Missiles:
                return new ECSLayerID[]
                {
                    ECSLayerID.Projectiles
                };
            default:
                throw new System.NotImplementedException();
        }
    }
}
