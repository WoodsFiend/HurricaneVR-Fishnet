using FishNet.Object;
using HurricaneVR.Framework.Components;

public class NetworkDestructible : NetworkBehaviour
{
    private CustomHVRDestructible hVRDestructible;
    private bool isDestroyed;
    public bool isServer;

    private void Awake()
    {
        hVRDestructible = GetComponent<CustomHVRDestructible>();

        hVRDestructible.BeforeDestroy.AddListener(OnBeforeDestroy);
    }

    private void OnBeforeDestroy()
    {
        if (!isDestroyed)
        {
            if (!isServer)
            {
                RPCDestroy();
            }
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (LocalConnection == Owner)
        {
            isServer = true;
            Destroy(hVRDestructible);
        }
    }
    private void OnDestroy()
    {
        if(hVRDestructible != null) hVRDestructible.BeforeDestroy.RemoveListener(OnBeforeDestroy);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RPCDestroy()
    {
        //Tell observers about destroy
        ObserversDestroy();
        Destroy(gameObject);
    }

    [ObserversRpc]
    private void ObserversDestroy()
    {
        //Destroy the destructible on the observer clients that still have it
        if (hVRDestructible != null && !isDestroyed)
        {
            isDestroyed = true;
            hVRDestructible.Destroy();
        }
    }
}
