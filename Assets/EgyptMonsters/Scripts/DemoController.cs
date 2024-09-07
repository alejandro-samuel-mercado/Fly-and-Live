using UnityEngine;
using System.Collections;

public class DemoController : MonoBehaviour
{
    private Animator animator;

    public float walkspeed = 5f; // Velocidad de movimiento
        public float descendSpeed = 10f; // Velocidad de descenso    private float horizontal;
    private float vertical;
      private float horizontal;
    private float rotationDegreePerSecond = 1000f;
    private bool isAttacking = false;

  public Camera gamecam; // Referencia a la cámara que sigue al jugador

    public Vector2 camPosition;
    private bool dead;

    public GameObject[] characters;
    public int currentChar = 0;

    public GameObject[] targets;
    public float minAttackDistance;

    public UnityEngine.UI.Text nameText;

    void Start()
    {
        setCharacter(0);
    }

    void FixedUpdate()
    {
            if (animator && !dead)
    {
        // Obtener entradas de movimiento
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Obtenemos la rotación de la cámara
        Vector3 camForward = gamecam.transform.forward;
        Vector3 camRight = gamecam.transform.right;

        // Ignoramos la rotación en el eje Y para evitar que se incline hacia arriba o abajo
        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        // Movemos el personaje relativo a la cámara
        Vector3 stickDirection = (camForward * vertical + camRight * horizontal).normalized;
        float speedOut = stickDirection.sqrMagnitude;

        // Asegurarse de que la dirección no sea cero
        if (stickDirection != Vector3.zero && !isAttacking)
        {
            // Rotar hacia la dirección del movimiento
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(stickDirection, Vector3.up), rotationDegreePerSecond * Time.deltaTime);
        }

        // Aplicar velocidad al Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 newVelocity = stickDirection * speedOut * walkspeed;
        rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);

        // Depuración
        Debug.Log($"stickDirection: {stickDirection}, speedOut: {speedOut}, newVelocity: {newVelocity}");

        // Actualizar el parámetro de velocidad en el Animator
        animator.SetFloat("Speed", speedOut);
    }
    }

    void Update()
    {
        if (!dead)
        {

            // Ataque
            if (Input.GetKeyDown(KeyCode.Space)  && !isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("Attack");
                StartCoroutine(stopAttack(1));
                tryDamageTarget();
            }

            // Recibir daño
            if (Input.GetKeyDown(KeyCode.N) && !isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("Hit");
                StartCoroutine(stopAttack(1));
            }

            // Actualizar el estado de ataque en el Animator
            animator.SetBool("isAttacking", isAttacking);

            // Cambiar personaje
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                setCharacter(-1);
                isAttacking = true;
                StartCoroutine(stopAttack(1f));
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                setCharacter(1);
                isAttacking = true;
                StartCoroutine(stopAttack(1f));
            }

            // Muerte
            if (Input.GetKeyDown(KeyCode.M))
                StartCoroutine(selfdestruct());

            // Salir
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (ContainsParam(animator, "Leave"))
                {
                    animator.SetTrigger("Leave");
                    StartCoroutine(stopAttack(1f));
                }
            }

               Rigidbody rb = GetComponent<Rigidbody>();
if (Input.GetMouseButton(0)) // Clic izquierdo
        {
            rb.velocity = new Vector3(rb.velocity.x, walkspeed, rb.velocity.z);
        }
        else if (Input.GetMouseButton(1)) // Clic derecho
        {
            rb.velocity = new Vector3(rb.velocity.x, -descendSpeed, rb.velocity.z);
        }
        else
        {
            // Detener el movimiento vertical cuando no se presionan clics
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        }
    }

    GameObject target = null;

    public void tryDamageTarget()
    {
        target = null;
        float targetDistance = minAttackDistance + 1;
        foreach (var item in targets)
        {
            float itemDistance = (item.transform.position - transform.position).magnitude;
            if (itemDistance < minAttackDistance)
            {
                if (target == null)
                {
                    target = item;
                    targetDistance = itemDistance;
                }
                else if (itemDistance < targetDistance)
                {
                    target = item;
                    targetDistance = itemDistance;
                }
            }
        }
        if (target != null)
        {
            transform.LookAt(target.transform);
        }
    }

    public void DealDamage(DealDamageComponent comp)
    {
        if (target != null)
        {
            target.GetComponent<Animator>().SetTrigger("Hit");
            var hitFX = Instantiate(comp.hitFX);
            hitFX.transform.position = target.transform.position + new Vector3(0, target.GetComponentInChildren<SkinnedMeshRenderer>().bounds.center.y, 0);
        }
    }

    public IEnumerator stopAttack(float length)
    {
        yield return new WaitForSeconds(length);
        isAttacking = false;
    }

    public IEnumerator selfdestruct()
    {
        animator.SetTrigger("isDead");
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        dead = true;

        yield return new WaitForSeconds(3f);
        while (true)
        {
            if (Input.anyKeyDown)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                yield break;
            }
            else
                yield return null;
        }
    }

    public void setCharacter(int i)
    {
        currentChar += i;

        if (currentChar > characters.Length - 1)
            currentChar = 0;
        if (currentChar < 0)
            currentChar = characters.Length - 1;

        foreach (GameObject child in characters)
        {
            if (child == characters[currentChar])
            {
                child.SetActive(true);
                if (nameText != null)
                    nameText.text = child.name;
            }
            else
            {
                child.SetActive(false);
            }
        }
        animator = GetComponentInChildren<Animator>();
    }

    public bool ContainsParam(Animator _Anim, string _ParamName)
    {
        foreach (AnimatorControllerParameter param in _Anim.parameters)
        {
            if (param.name == _ParamName) return true;
        }
        return false;
    }
}
