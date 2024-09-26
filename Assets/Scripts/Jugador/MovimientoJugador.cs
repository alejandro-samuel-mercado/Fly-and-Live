using UnityEngine;

namespace CompleteProject
{
    public class MovimientoJugador : MonoBehaviour
    {
        public float velocidad = 6f;
        public float fuerzaSalto= 10f;  // Fuerza del salto
        private Vector3 movement;
        private Animator anim;
        private Rigidbody playerRigidbody;
public Transform groundCheck;
public float groundCheckRadius = 0.3f;
public LayerMask groundLayer;
        private Transform cameraTransform;

        private bool estaEnSuelo;  // Verifica si el jugador está en el suelo
  private bool puedeSaltar ; 

        private float camRayLength = 100f;  // Longitud del rayo de la cámara
      private int floorMask;   

        private void Awake()
        {
            InicializarComponentes();
        }

        private void FixedUpdate()
        {
// Comprobar si el jugador está en el suelo usando un raycast
    estaEnSuelo = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
  if (estaEnSuelo)
            {
                puedeSaltar = true;
            }
    ProcesarMovimiento();
    ActualizarAnimaciones();

    #if !MOBILE_INPUT
    GirarJugador();
    #endif
        }

        private void InicializarComponentes()
        {

            cameraTransform = Camera.main.transform;  // Obtener la referencia a la cámara principal
            anim = GetComponent<Animator>();
            playerRigidbody = GetComponent<Rigidbody>();
        }

        private void ProcesarMovimiento()
        {
           // Obtener entrada de teclado
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            // Ajustar la dirección de movimiento del jugador en base a la rotación de la cámara
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;  // Mantener el movimiento en el plano horizontal
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * v + right * h;

            // Mover el jugador en la dirección deseada
            Vector3 targetVelocity = desiredMoveDirection * velocidad;
            targetVelocity.y = playerRigidbody.velocity.y; // Mantener la velocidad vertical actual

            playerRigidbody.velocity = targetVelocity;
            movement = desiredMoveDirection;

            // Comprobar si se presiona la tecla de salto
            if (Input.GetButtonDown("Jump") && estaEnSuelo && puedeSaltar)
            {
                Saltar();
                puedeSaltar = false;  // Impedir más saltos hasta que el jugador toque el suelo nuevamente
            }
        }

        private void ActualizarAnimaciones()
        {
            bool isWalking = movement.magnitude > 0;
            anim.SetBool("IsWalking", isWalking);
        }

        private void GirarJugador()
        {
    // Solo girar si hay movimiento
    if (movement.magnitude > 0.1f) // Umbral para detectar movimiento significativo
    {
        Quaternion targetRotation = Quaternion.LookRotation(movement);
        playerRigidbody.MoveRotation(targetRotation);
    }
        }

        private void Saltar()
        {  Debug.Log("Salto ejecutado");
            // Aplicar una fuerza hacia arriba para el salto
             playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);  // Restablecer la velocidad vertical antes de aplicar la nueva fuerza
            playerRigidbody.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }

        private void OnCollisionStay(Collision collision)
        {
            // Verificar si el jugador está en el suelo
            if (collision.gameObject.CompareTag("Ground"))
            {
                estaEnSuelo = true;
 puedeSaltar = true; 
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            // El jugador ya no está en el suelo
            if (collision.gameObject.CompareTag("Ground"))
            {
                estaEnSuelo = false;
            }
        }
    }
}

