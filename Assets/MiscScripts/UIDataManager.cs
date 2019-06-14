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
using UnityEngine.UI;

public class UIDataManager : MonoBehaviour
{
    public static UIDataManager instance;

    public GameObject steerPos;
    public Slider timscaleIndicator;

    void Awake()
    {
        instance = this;
    }
}
