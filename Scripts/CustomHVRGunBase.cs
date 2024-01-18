

using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Weapons;
using HurricaneVR.Framework.Weapons.Guns;
using UnityEngine;

public class CustomHVRGunBase : HVRGunBase 
{
    [SerializeField]
    private bool ejectCasingOnHandleEjected = false;
    [SerializeField]
    private bool enableChamberedOnHandleEjected = false;
    [SerializeField]
    private bool hasChamberedAmmo = false;

    //This should be controlled by the ammo being shot
    [SerializeField]
    private int NumberOfPellets = 1;
    [SerializeField]
    private float ShotRadius = 0.05f;
    [SerializeField]
    private float BurstCooldown = 0.2f;
    protected override void Awake()
    {
        base.Awake();
        if (hasChamberedAmmo)
        {
            Ammo = GetComponent<HVRShotgunMagazine>();
        }
    }
    //Custom entry point for networked shooting
    public void NetworkShoot()
    {
        Shoot();
    }

    public override void TriggerPulled()
    {
        if (FireType == GunFireType.ThreeRoundBurst && Time.time - TimeOfLastShot < BurstCooldown)
            return;
        base.TriggerPulled();
    }


    protected override void OnFire(Vector3 direction)
    {
        if (NumberOfPellets == 1)
        {
            base.OnFire(direction);
        }
        else
        {
            for (int i = 0; i < NumberOfPellets; i++)
            {
                var xy = Random.insideUnitCircle * ShotRadius;
                var newDirection = direction + transform.TransformDirection(xy);
                FireBullet(newDirection);
            }
            FireHaptics();
        }
    }

    protected override void OnAmmoSocketed(HVRGrabberBase grabber, HVRGrabbable grabbable)
    {
        if (hasChamberedAmmo)
        {
            AmmoSocketedHaptics();
        }
        else
        {
            base.OnAmmoSocketed(grabber, grabbable);
        }
    }
    protected override void OnAmmoSocketReleased(HVRGrabberBase grabber, HVRGrabbable grabbable)
    {
        if (!hasChamberedAmmo)
        {
            base.OnAmmoSocketReleased(grabber, grabbable);
        }
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
