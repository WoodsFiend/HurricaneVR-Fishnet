using HurricaneVR.Framework.Components;
using UnityEngine;
using UnityEngine.Events;

public class CustomHVRDamageHandler : HVRDamageHandler
{
    public UnityEvent<float, Vector3, Vector3> ServerDamageTaken = new UnityEvent<float, Vector3, Vector3>();
    public UnityEvent<float, Vector3, Vector3> ClientDamageTaken = new UnityEvent<float, Vector3, Vector3>();
    protected NetworkDamageHandler networkDamageHandler;

    private void Awake()
    {
        networkDamageHandler = GetComponent<NetworkDamageHandler>();
    }

    public override void TakeDamage(float damage)
    {
        //Only the server sends damage taken event
        if (networkDamageHandler.isServer)
        {
            //Debug.Log("Server Damage");
            base.TakeDamage(damage);
        }
    }
    //Clients should call this to update the health from damage
    public virtual void TakeNetworkDamage(float damage, Vector3 hitPoint, Vector3 direction)
    {
        //Debug.Log("Network Damage");
        base.TakeDamage(damage);
        ClientDamageTaken.Invoke(damage, hitPoint, direction);
    }

    public override void HandleDamageProvider(HVRDamageProvider damageProvider, Vector3 hitPoint, Vector3 direction)
    {
        if (networkDamageHandler.isServer)
        {
            base.HandleDamageProvider(damageProvider, hitPoint, direction);
            ServerDamageTaken.Invoke(damageProvider.Damage, hitPoint, direction);
        }
    }

}