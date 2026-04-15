using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;


[Serializable]
public class Stat 
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new List<StatModifier>();

    private float finalValue;
    private bool needToCalculate = true;


    public float GetValue()
    {

        if (needToCalculate)
        {
            finalValue = GetFinalValue();
            needToCalculate = false;
        }


        return finalValue;
    }


    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd  = new StatModifier(value, source);
        modifiers.Add(modToAdd);
        needToCalculate = true;
    }
    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.Source == source);
        needToCalculate |= true;
    }


    public float GetFinalValue()
    {
        float finalValue = baseValue;

        foreach (var modifier in modifiers)
        {
            finalValue +=modifier.Value;
        }
        return finalValue;
    }

    public void SetBaseValue(float value) => baseValue = value;

}


[Serializable]

public class StatModifier
{
    public float Value;
    public string Source;

    public StatModifier(float value, string source)
    {
        Value = value;
        Source = source;
    }
}

