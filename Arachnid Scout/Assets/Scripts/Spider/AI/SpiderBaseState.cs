using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpiderBaseState 
{
    public abstract void EnterState(SpiderAI spider);

    public abstract void UpdateState(SpiderAI spider);
}
