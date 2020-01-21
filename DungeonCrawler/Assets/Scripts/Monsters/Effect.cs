using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PossibleEffects
{
    Poisoned,
    OnFire,
    SlowedDown
}

public class Effect
{
    PossibleEffects effectType;

    const float poisonTimer = 5;
    const float fireTimer = 2;
    const float slowTimer = 3;

    float timer;
    bool isActive;
    public Effect(PossibleEffects type)
    {
        effectType = type;
        timer = 0.0f;
        isActive = false;
    }

    public void StartEffect()
    {
        switch(effectType)
        {
            case PossibleEffects.Poisoned:
                timer = poisonTimer;
                break;
            case PossibleEffects.OnFire:
                timer = fireTimer;
                break;
            case PossibleEffects.SlowedDown:
                timer = slowTimer;
                break;
            default:
                break;
        }
        isActive = true;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void Update(float deltaTime)
    {
        int oldSecond = Mathf.FloorToInt(timer);
        timer -= deltaTime;
        if(oldSecond != Mathf.FloorToInt(timer))
        {
            // TODO damage player
        }

        if(timer <= 0)
        {
            isActive = false;
        }
    }
}
