using HurricaneVR.Framework.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivableDestructible : HVRDestructible
{
    public virtual void Revive()
    {
        // Implement this function in your own RevivableDestructible that reverts the death state
        // The player has already gained back the health to revive at this point
    }
}
