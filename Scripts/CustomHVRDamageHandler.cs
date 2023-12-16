using HurricaneVR.Framework.Components;
using UnityEngine;
using UnityEngine.Events;

public class CustomHVRDamageHandler : HVRDamageHandler
{
    public float MaxLife = 100f;
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
            CheckForRevive();
            EnforceLifeLimit();
        }
    }

    //Clients should call this to update the health from damage
    public virtual void TakeNetworkDamage(float damage, Vector3 hitPoint, Vector3 direction)
    {
        //Debug.Log("Network Damage");
        base.TakeDamage(damage);
        ClientDamageTaken.Invoke(damage, hitPoint, direction);
        CheckForRevive();
        EnforceLifeLimit();
    }


    private void CheckForRevive()
    {
        if (Desctructible.Destroyed && Life > 0)
        {
            var playerdDeathDestructible = Desctructible as PlayerDeathDestructible;
            if (playerdDeathDestructible)
            {
                playerdDeathDestructible.Revive();
            }
        }
    }
    private void EnforceLifeLimit()
    {
        if (Life < 0)
        {
            Life = 0;
        }
        else if (Life > MaxLife)
        {
            Life = MaxLife;
        }
    }

    public override void HandleDamageProvider(HVRDamageProvider damageProvider, Vector3 hitPoint, Vector3 direction)
    {
        if (networkDamageHandler.IsServer)
        {
            base.HandleDamageProvider(damageProvider, hitPoint, direction);
            ServerDamageTaken.Invoke(damageProvider.Damage, hitPoint, direction);
        }
    }

    public void HandleDamageProvider(HVRDamageProvider damageProvider, Vector3 hitPoint, Vector3 direction, float damageMultiplier)
    {
        if (networkDamageHandler.IsServer)
        {
            //base.HandleDamageProvider(damageProvider, hitPoint, direction);
            TakeDamage(damageProvider.Damage * damageMultiplier);
            if (Rigidbody)
            {
                Rigidbody.AddForceAtPosition(direction.normalized * damageProvider.Force, hitPoint, ForceMode.Impulse);
            }
            ServerDamageTaken.Invoke(damageProvider.Damage * damageMultiplier, hitPoint, direction);
        }
    }
}