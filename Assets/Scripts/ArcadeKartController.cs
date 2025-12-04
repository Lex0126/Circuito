using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArcadeKartController : MonoBehaviour
{
    private Rigidbody rb; // Componente de fisica

    [Header("Movimiento")]
    public float moveSpeed = 5f; // Velocidad de aceleracion.
    public float turnSpeed = 50f; // Velocidad de giro normal.
    public float downForce = 5f; // Fuerza hacia abajo (agarre).

    [Header("Derrape (Drift)")]
    public float driftTurnSpeed = 50f; // Velocidad de giro en derrape.
    public float driftSidewaysDamp = 0.5f; // Freno lateral (anti-derrape).

    // Variables de Input (publicas)
    // Otros scripts escriben aqui
    [HideInInspector]
    public float moveInput;
    [HideInInspector]
    public float turnInput;
    [HideInInspector]
    public bool isDrifting = false;

    // Variables de Estado
    private bool isGrounded = false; // Esta tocando el suelo?

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update() se ha eliminado
    // Otro script gestiona el input

    // Se llama mientras toca un collider
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Se llama al dejar de tocar
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Logica de fisica
    // Lee moveInput y turnInput
    void FixedUpdate()
    {
        if (isGrounded)
        {
            // Aceleracion
            rb.AddForce(transform.forward * moveInput * moveSpeed, ForceMode.Acceleration);

            // Agarre
            rb.AddForce(Vector3.down * downForce, ForceMode.Acceleration);

            // Giro
            float currentTurnSpeed = isDrifting ? driftTurnSpeed : turnSpeed;
            Quaternion turnRotation = Quaternion.Euler(0f, turnInput * currentTurnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);

            // Friccion lateral (si no derrapa)
            if (!isDrifting)
            {
                Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
                localVel.x *= driftSidewaysDamp;
                rb.linearVelocity = transform.TransformDirection(localVel);
            }
        }
    }
}