using FishNet.Object;
using UnityEngine;

public class NetworkHead : NetworkBehaviour
{
    [SerializeField]
    private Transform headToScale;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!Owner.IsLocalClient)
        {
            //Show the head on remote players for FinalIK
            if (headToScale)
            {
                headToScale.localScale = Vector3.one;
            }
        }
    }
}
