using UnityEngine;

[RequireComponent(typeof(ArcadeKartController))]
public class PlayerInput : MonoBehaviour
{
    private ArcadeKartController kart; // Referencia al cerebro

    void Start()
    {
        // Obtiene el controlador del mismo objeto
        kart = GetComponent<ArcadeKartController>();
    }

    void Update()
    {
        // Esta es la logica original de tu Update()
        // Ahora solo escribe en las variables publicas del kart.
        kart.moveInput = Input.GetAxis("Vertical");
        kart.turnInput = Input.GetAxis("Horizontal");
        kart.isDrifting = Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1");
    }
}