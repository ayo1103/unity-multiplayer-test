using Unity.Netcode;

namespace Ayo
{
    public class CameraFollow : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                gameObject.SetActive(false);
            }
        }
    }
}