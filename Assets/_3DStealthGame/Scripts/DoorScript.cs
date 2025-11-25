using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public string KeyName;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player == null) return;

        if (player.OwnKey(KeyName))
        {
            Destroy(gameObject);
        }
    }
}
