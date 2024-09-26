using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;                // El jugador al que sigue la cámara
    public Vector3 offset = new Vector3(0, 2, -4);  // Distancia entre la cámara y el jugador
    public float rotationSpeed = 100f;      // Velocidad de rotación de la cámara con el ratón
    public float followSpeed = 10f;         // Velocidad con la que la cámara sigue al jugador

    private float yaw = 0f;                 // Rotación horizontal (eje Y)
    private float pitch = 0f;               // Rotación vertical (eje X)

    void Start()
    {
        // Inicializamos la rotación para que siga la dirección inicial del jugador
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        // Obtener la entrada del ratón para rotar la cámara alrededor del jugador
        yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // Limitar el ángulo de la cámara verticalmente (evitar girar hacia arriba/abajo demasiado)
        pitch = Mathf.Clamp(pitch, -45f, 45f);

        // Crear la rotación de la cámara basada en los valores de pitch y yaw
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Calcular la nueva posición de la cámara usando el offset y la rotación
        Vector3 desiredPosition = player.position + rotation * offset;

        // Aplicar un movimiento suave para seguir al jugador (lerp para suavidad)
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Hacer que la cámara siempre mire al jugador
        transform.LookAt(player.position + Vector3.up * offset.y);
    }
}


