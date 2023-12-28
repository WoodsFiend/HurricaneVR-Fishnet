

using HurricaneVR.Framework.Weapons.Guns;
using UnityEngine;

public class CustomHVRGunBase : HVRGunBase 
{
    [SerializeField]
    private bool ejectCasingOnHandleEjected = false;
    [SerializeField]
    private bool enableChamberedOnHandleEjected = false;

    protected override void Awake()
    {
        base.Awake();
    }
    //Custom entry point for networked shooting
    public void NetworkShoot()
    {
        Shoot();
    }
    protected override void OnCockingHandleEjected()
    {
        base.OnCockingHandleEjected();
        if (ejectCasingOnHandleEjected)
        {
            if (ChamberedCasing && ChamberedCasing.activeSelf)
            {
                EjectCasing();
            }
        }
        if (enableChamberedOnHandleEjected)
        {
            if (ChamberedRound && !ChamberedRound.activeSelf)
            {
                if (Ammo && Ammo.HasAmmo)
                {
                    ChamberedRound.SetActive(true);
                }
            }
        }
    }
}
