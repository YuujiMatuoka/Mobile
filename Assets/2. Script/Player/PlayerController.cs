using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 15f;
    public float bounceFactor = 0.8f;

    private Rigidbody2D rb;
    private Vector2 lastFrameVelocity;
    private bool isCollidingWithLimit = false;
    public MouseInputMovement mouseInput;


    private void Awake()
    {
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Configurar el Rigidbody
        rb.gravityScale = 0f;
        rb.drag = 0.1f;
        rb.angularDrag = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Suscribirse al evento de fuerza aplicada
        MouseInputMovement.OnForceApplied += ApplyMouseForce;
    }

    void OnDestroy()
    {
        // Desuscribirse del evento
        MouseInputMovement.OnForceApplied -= ApplyMouseForce;
    }

    void FixedUpdate()
    {
        // Guardar la velocidad antes de posibles colisiones
        lastFrameVelocity = rb.velocity;

        // Limitar velocidad máxima
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void ApplyMouseForce(Vector2 force)
    {
        // Aplicar la fuerza recibida del mouse
        rb.velocity = Vector2.zero; // Resetear velocidad anterior
        rb.AddForce(force, ForceMode2D.Impulse);

        Debug.Log("Fuerza de mouse aplicada: " + force);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameObject.SetActive(false);
        }


        if (collision.gameObject.layer == LayerMask.NameToLayer("Limit"))
        {
            isCollidingWithLimit = true;
            HandleLimitCollision(collision);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Limit"))
        {
            isCollidingWithLimit = false;
        }
    }

    void HandleLimitCollision(Collision2D collision)
    {
        if (collision.contactCount > 0)
        {
            ContactPoint2D contact = collision.contacts[0];

            // Calcular el rebote basado en la velocidad anterior a la colisión
            Vector2 bounceVelocity = Vector2.Reflect(lastFrameVelocity, contact.normal);

            // Aplicar factor de rebote
            bounceVelocity = bounceVelocity * bounceFactor;

            // Aplicar la nueva velocidad solo si es significativa
            if (bounceVelocity.magnitude > 0.1f)
            {
                rb.velocity = bounceVelocity;
                Debug.Log("Rebote aplicado. Velocidad: " + bounceVelocity);
            }
            else
            {
                // Si la velocidad de rebote es muy pequeña, detener el objeto
                rb.velocity = Vector2.zero;
                Debug.Log("Velocidad demasiado baja, deteniendo objeto");
            }
        }
    }

    void OnGUI()
    {
        // Mostrar información de debug
        GUI.Label(new Rect(10, 100, 300, 20), "Velocidad: " + rb.velocity.ToString("F2"));
        GUI.Label(new Rect(10, 120, 300, 20), "En colisión: " + isCollidingWithLimit);
        GUI.Label(new Rect(10, 140, 300, 20), "Última velocidad: " + lastFrameVelocity.magnitude.ToString("F2"));
    }
}
