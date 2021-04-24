using UnityEngine;

public class MovementFlipper : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    static readonly Vector3 leftScale = new Vector3(-1, 1, 1);
    static readonly Vector3 rightScale = new Vector3(1, 1, 1);

    void Update()
    {
        transform.localScale = rb.velocity.x < 0 ? leftScale : rightScale;
    }
}