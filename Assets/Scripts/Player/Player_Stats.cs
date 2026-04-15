using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : Entity_Stats
{

    private List<string> activeBuff = new List<string>();


    protected override void Awake()
    {
        base.Awake();
    }

}
