using UnityEngine;


[CreateAssetMenu(menuName ="Curve/MoveCurve",fileName = "MoveCurve")] 
public class MoveCurveSO : ScriptableObject
{
    public AnimationCurve velocityCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float duration = 1f;
}
