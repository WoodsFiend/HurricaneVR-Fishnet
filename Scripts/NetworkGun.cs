using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using HurricaneVR.Framework.Weapons.Guns;
using System;

//Client Authoritative gun solution
//For Server Authoritative the server would
//need to manage gun cocking and ammo count
//and the client would just need to pass its trigger state
public class NetworkGun : NetworkBehaviour
{
    private CustomHVRGunBase hVRGunBase;

    [SyncVar(WritePermissions = WritePermission.ServerOnly)]
    public bool isChambered;

    private void Awake()
    {
        hVRGunBase = GetComponent<CustomHVRGunBase>();
        hVRGunBase.Fired.AddListener(OnFired);
        hVRGunBase.CockingHandle.ChamberRound.AddListener(OnChamberRound);
    }

    private void OnDestroy()
    {
        if (hVRGunBase)
        {
            hVRGunBase.Fired.RemoveListener(OnFired);
            hVRGunBase.CockingHandle.ChamberRound.RemoveListener(OnChamberRound);
        }
    }
    private void OnChamberRound()
    {
        if (Owner.IsLocalClient)
        {
            var chambered = hVRGunBase.Ammo && hVRGunBase.Ammo.HasAmmo;
            RPCChambered(chambered);
        }
    }

    private void OnFired()
    {
        if (Owner.IsLocalClient)
        {
            RPCShoot();
            //The gun chambers after firing and has run out of ammo
            if(hVRGunBase.ChambersAfterFiring && hVRGunBase.Ammo && !hVRGunBase.Ammo.HasAmmo)
            {
                RPCChambered(false);
            }
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        //The server can always shoot upon request of a client
        hVRGunBase.RequiresAmmo = false;
        hVRGunBase.RequiresChamberedBullet = false;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        //I am not the owner and the gun is chambered on the server
        if (!Owner.IsLocalClient && isChambered)
        {
            hVRGunBase.IsBulletChambered = true;
        }
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
    [ServerRpc(RequireOwnership = true)]
    private void RPCChambered(bool chambered)
    {
        isChambered = chambered;
        ObserversChambered();
    }
    [ObserversRpc(ExcludeOwner = true)]
    private void ObserversChambered()
    {
        hVRGunBase.IsBulletChambered = isChambered;
    }
}
