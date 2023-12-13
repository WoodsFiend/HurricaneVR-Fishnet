using FishNet.Object;
using UnityEngine;

public class NetworkDamageHandler : NetworkBehaviour
{
    private CustomHVRDamageHandler hVRDamageHandler;

    public bool isServer;

    private void Awake()
    {
        hVRDamageHandler = GetComponent<CustomHVRDamageHandler>();
        hVRDamageHandler.ServerDamageTaken.AddListener(OnDamageTaken);
    }
    private void OnDestroy()
    {
        if (hVRDamageHandler) hVRDamageHandler.ServerDamageTaken.RemoveListener(OnDamageTaken);
    }

    private void OnDamageTaken(float damage, Vector3 hitPoint, Vector3 direction)
    {
        RPCDamageTaken(damage, hitPoint, direction);
    }

    //The server should be the only one sending damage and is already updated
    [ObserversRpc(ExcludeServer = true)]
    private void RPCDamageTaken(float damage, Vector3 hitPoint, Vector3 direction)
    {
        hVRDamageHandler.TakeNetworkDamage(damage, hitPoint, direction);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        //The server will send the damage to itself and update all the clients
        isServer = true;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        //Client damage handlers should never trigger destruction, it will be handled in NetworkDestructible
        hVRDamageHandler.Desctructible = null;
    }
}
