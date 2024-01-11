using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteComponentsManager : NetworkBehaviour
{
    [SerializeField]
    private List<Component> remoteRemoveComponents;
    [SerializeField]
    private List<GameObject> remoteRemoveGameObjects;

    public override void OnStartClient()
    {
        base.OnStartClient();
        // The local client doesn't change any components
        if (Owner.IsLocalClient)
        {
            return;
        }
        // The remote clients should remove all the components and
        // game objects that are included in the lists
        foreach (var comp in remoteRemoveComponents)
        {
            Destroy(comp);
        }
        foreach (var go in remoteRemoveGameObjects)
        {
            Destroy(go);
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (Owner.IsLocalClient)
        {
            return;
        }
        // The remote clients should remove all the components and
        // game objects that are included in the lists
        foreach (var comp in remoteRemoveComponents)
        {
            Destroy(comp);
        }
        foreach (var go in remoteRemoveGameObjects)
        {
            Destroy(go);
        }
    }
}
