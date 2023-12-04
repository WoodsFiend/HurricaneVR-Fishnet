using FishNet.Connection;
using FishNet.Object;
using HurricaneVR.Framework.Weapons.Guns;
using System;

//Client Authoritative gun solution
//For Server Authoritative the server would
//need to manage gun cocking and ammo count
//and the client would just need to pass its trigger state
public class NetworkGun : NetworkBehaviour
{
    private CustomHVRGunBase hVRGunBase;

    private void Awake()
    {
        hVRGunBase = GetComponent<CustomHVRGunBase>();
        hVRGunBase.Fired.AddListener(OnFired);
    }
    private void OnDestroy()
    {
        if (hVRGunBase) hVRGunBase.Fired.RemoveListener(OnFired);
    }
    private void OnFired()
    {
        if (Owner.IsLocalClient)
        {
            RPCShoot();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        //The server can always shoot upon request of a client
        hVRGunBase.RequiresAmmo = false;
        hVRGunBase.RequiresChamberedBullet = false;
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        base.OnOwnershipClient(prevOwner);
        //Only the owner need to track shooting requirements
        if (Owner.IsLocalClient)
        {
            hVRGunBase.RequiresAmmo = true;
            hVRGunBase.RequiresChamberedBullet = true;
        }
        else
        {
            //The observers always shoot upon request of the server
            hVRGunBase.RequiresAmmo = false;
            hVRGunBase.RequiresChamberedBullet = false;
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void RPCShoot()
    {
        hVRGunBase.NetworkShoot();
        ObserversShoot();
    }
    [ObserversRpc(ExcludeOwner = true)]
    private void ObserversShoot()
    {
        hVRGunBase.NetworkShoot();
    }
}
