using HurricaneVR.Framework.Core;
using UnityEngine;
using UnityEngine.Events;

namespace HurricaneVR.Framework.Components
{
    /// <summary>
    /// Helper component that spawns a prefab game object when the Destroy function is called.
    /// If the spawned game object has a rigidbodies then they will have force added to them based on the
    /// fields provided.
    /// </summary>
    public class CustomHVRDestructible : HVRDestructible
    {
        public UnityEvent BeforeDestroy = new UnityEvent();
        private NetworkDestructible networkDestructible;
        private void Awake()
        {
            networkDestructible = GetComponent<NetworkDestructible>();
        }
        public override void Destroy()
        {
            //Debug.Log("Destroy Called");
            if (Destroyed) return;
            //Add an event to fire network RPC
            BeforeDestroy.Invoke();
            
            if (DestroyedVersion)
            {
                var destroyed = Instantiate(DestroyedVersion, transform.position, transform.rotation);

                foreach (var rigidBody in destroyed.GetComponentsInChildren<Rigidbody>())
                {
                    var v = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                    rigidBody.AddForce(v * ExplosionPower, ForceMode.VelocityChange);

                    if (RemoveDebris)
                    {
                        var delay = Random.Range(RemoveDebrisTimerLower, RemoveDebrisTimerUpper);
                        if (delay < .1f)
                        {
                            delay = 3f;
                        }

                        var timer = rigidBody.gameObject.AddComponent<HVRDestroyTimer>();
                        timer.StartTimer(delay);
                    }

                    if (IgnorePlayerCollision)
                    {
                        var colliders = rigidBody.gameObject.GetComponentsInChildren<Collider>();
                        HVRManager.Instance?.IgnorePlayerCollision(colliders);
                    }

                    rigidBody.transform.parent = null;
                }

                if (RemoveDebris)
                {
                    var timer = destroyed.gameObject.AddComponent<HVRDestroyTimer>();
                    var delay = RemoveDebrisTimerUpper;
                    if (delay <= .1f)
                        delay = 3f;
                    timer.StartTimer(delay);
                }
            }

            Destroyed = true;
            if (networkDestructible && networkDestructible.isServer)
            {
                //Delayed destroy on the server
                Destroy(gameObject, RemoveDebrisTimerUpper);
            }
            else
            {
                //Immediate destroy on clients
                Destroy(gameObject);
            }
        }
    }
}