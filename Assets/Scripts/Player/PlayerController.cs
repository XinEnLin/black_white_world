using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public UnityEvent OnShutter;

    private Rigidbody2D rb;
    private Vector2 facingDirection = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            OnShutter?.Invoke();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v);

        if (input.sqrMagnitude > 0f)
        {
            facingDirection = input.normalized;
            rb.linearVelocity = facingDirection * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public Vector2 FacingDirection => facingDirection;
}