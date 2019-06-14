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

public class GatherShootInputSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerTag>().ForEach((ref ShootWeapon shootWeapon) =>
        {
            if (Input.GetButton("Fire1"))
            {
                shootWeapon.shoot = true;
            }
        });
    }
}

public class GatherTranslationInputSystem : ComponentSystem
{
    const float moveSpeedCoef = 6;

    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerTag>().ForEach((ref SteeringInput steerInput, ref MoveSpeed moveSpeed) =>
        {
            moveSpeed = new MoveSpeed
            {
                Value = new float3(Input.GetAxis("ThrustX") * moveSpeedCoef, Input.GetAxis("ThrustY") * moveSpeedCoef, Input.GetAxis("ThrustZ") * moveSpeedCoef)
            };
        });
    }
}

public class SimpleGaterRotationInputSystem : ComponentSystem
{
    const float pitchSpeed = 50;
    const float rollSpeed = 1000;
    const float yawSpeed = 50;

    const float steerPosSensitivity = 18;
    const float steerPosMaxDistFromCenter = 20;

    const float steerPosDeadzone = 1f;

    float2 steerPos = float2.zero;

    protected override void OnCreate()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnUpdate()
    {
        if (Input.GetButtonDown("MouseLock"))
        {
            if (Cursor.lockState == CursorLockMode.None)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }

        float mouseX = 0;
        float mouseY = 0;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }
        
        steerPos += new float2(mouseX, mouseY) * Time.unscaledDeltaTime * steerPosSensitivity;
        if (math.length(steerPos) > steerPosMaxDistFromCenter)
            steerPos = math.normalize(steerPos) * steerPosMaxDistFromCenter;

        UIDataManager.instance.steerPos.transform.localPosition = new Vector3(steerPos.x, steerPos.y, 0) * 10;

        Entities.WithAll<PlayerTag>().ForEach((ref SteeringInput steerInput) =>
        {
            
            if (Cursor.lockState != CursorLockMode.Locked || math.length(steerPos) < steerPosDeadzone)
            {
                steerInput = new SteeringInput
                {
                    pitch = 0,
                    roll = 0,
                    yaw = 0
                };
                return;
            }

            var scaledSteerPos = math.pow(steerPos, 2);
            scaledSteerPos = new float2(
                math.abs(scaledSteerPos.x) - steerPosDeadzone * math.sign(scaledSteerPos.x),
                math.abs(scaledSteerPos.y) - steerPosDeadzone * math.sign(scaledSteerPos.y));

            steerInput = new SteeringInput
            {
                pitch = -steerPos.y * pitchSpeed * Time.unscaledDeltaTime,
                roll = Input.GetAxis("Roll") * rollSpeed * Time.unscaledDeltaTime,
                yaw = steerPos.x * yawSpeed * Time.unscaledDeltaTime
            };
        });
    }
}

[DisableAutoCreation]
public class NoYawAutoRollGaterRotationInputSystem : ComponentSystem
{
    float pitchSpeed = 1600;
    float rollSpeed = 90;
    float moveSpeed = 100;

    float steerPosSensitivity = 9;
    float2 steerPos = float2.zero;

    protected override void OnCreate()
    {
        Cursor.lockState = CursorLockMode.Locked;        
    }

    protected override void OnUpdate()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        
        steerPos += new float2(mouseX, mouseY) * Time.unscaledDeltaTime * steerPosSensitivity;
        bool mouseNotMoved = mouseX == 0 && mouseY == 0;
        
        UIDataManager.instance.steerPos.transform.localPosition = new Vector3(steerPos.x, steerPos.y, 0) * 10;

        Entities.WithAll<PlayerTag>().ForEach((ref SteeringInput steerInput) => {
            if (Input.GetMouseButton(0))
            {
                steerInput = new SteeringInput
                {
                    pitch = 0,
                    roll = 0,
                    yaw = 0
                };
                return;
            }

            float roll = 0;
            float pitch = 0;

            float threshhold = 0.01f + 0.4f * steerPos.y;
            
            
            if (steerPos.y > threshhold && steerPos.x < -threshhold)
            {
                roll = 1 * rollSpeed;
                RotateSteerPos(-1, 1, mouseNotMoved);
            }
            else if (steerPos.y > threshhold && steerPos.x > threshhold)
            {
                roll = -1 * rollSpeed;
                RotateSteerPos(1, 1, mouseNotMoved);
            }
            else if (steerPos.y < -threshhold && steerPos.x < -threshhold)
            {
                roll = -1 * rollSpeed;
                RotateSteerPos(-1, -1, mouseNotMoved);
            }
            else if (steerPos.y < -threshhold && steerPos.x > threshhold)
            {
                roll = 1 * rollSpeed;
                RotateSteerPos(1, -1, mouseNotMoved);
            }

            if (steerPos.x < threshhold * 1.1f && steerPos.x > -threshhold * 1.1f &&
                (steerPos.y > threshhold * 1.1f || steerPos.y < -threshhold * 1.1f))
            {
                pitch = pitchSpeed * math.sign(steerPos.y);

                if (!mouseNotMoved)
                    return;

                float val = 0;
                if (steerPos.y > 0)                
                {
                    val = steerPos.y - (Time.unscaledDeltaTime * pitchSpeed * 0.001f);
                }
                else if (steerPos.y < 0)
                {
                    val = steerPos.y + (Time.unscaledDeltaTime * pitchSpeed * 0.001f);
                }

                steerPos = new float2(steerPos.x, val);
            }

            if (steerPos.x == float.NaN || steerPos.y == float.NaN)
                steerPos = float2.zero;

            steerInput = new SteeringInput
            {
                pitch = -pitch * Time.unscaledDeltaTime,
                roll = roll * 50 * Time.unscaledDeltaTime,
                yaw = 0
            };
        });
    }

    void RotateSteerPos(float dirX, float dirY, bool mouseNotMoved)
    {
        if (!mouseNotMoved)
            return;
        dirX *= 0.01f;
        dirY *= 0.01f;
        float steerLen = math.abs(math.length(steerPos));
        var normSteerPos = math.normalize(steerPos);
        float cos = math.acos(normSteerPos.x);
        float sin = math.asin(normSteerPos.y);
        var coef = Time.unscaledDeltaTime * 1.3f * rollSpeed;
        var newSteerDir = math.normalize(new float2(math.cos(cos + dirX * coef), math.sin(sin + dirY * coef)));

        steerPos = newSteerDir * steerLen;
    }
}