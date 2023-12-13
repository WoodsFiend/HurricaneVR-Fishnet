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
            ScaleHead();
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        ScaleHead();
    }
    private void ScaleHead()
    {
        //Show the head on remote players for FinalIK
        if (headToScale)
        {
            headToScale.localScale = Vector3.one;
        }
    }
}
