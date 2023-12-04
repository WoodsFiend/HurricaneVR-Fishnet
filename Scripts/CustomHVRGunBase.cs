

using HurricaneVR.Framework.Weapons.Guns;

public class CustomHVRGunBase : HVRGunBase 
{
    protected override void Awake()
    {
        base.Awake();
    }
    //Custom entry point for networked shooting
    public void NetworkShoot()
    {
        Shoot();
    }
}
