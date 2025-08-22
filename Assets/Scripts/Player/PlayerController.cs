using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlMode { FirstPerson, Tank }

public class PlayerController : MonoBehaviour
{
    [Header("Modo de control")]
    public ControlMode currentMode = ControlMode.FirstPerson;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 6.5f; //Sensibilidad del mouse
    [SerializeField] private float tankTurnSpeed = 90f; // Grados por segundo en modo tanque

    [Header("Componentes")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject bodyPlayer;

    [Space]
    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private DiffetentTypes_footSteps_with_FmodEvent footstepSystem; // sistema con Fmod

    private Rigidbody rb;
    private Cinemachine_Headbob_And_Noise camScript;
    private Animator animator;

    private float currentSpeed;
    private Vector2 rotationInput;
    private float xRotation = 0f;
    private float lookBackYawOffset = 0f;
    private Vector3 moveDirection;
    public Vector3 Move => moveDirection;

    [HideInInspector] public bool isRunning = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        camScript = GetComponent<Cinemachine_Headbob_And_Noise>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        animator = GetComponentInChildren<Animator>();
        currentSpeed = moveSpeed;

        if (rb != null)
            rb.freezeRotation = true;
    }

    void Update()
    {
        camScript.isMoving = moveDirection.magnitude > 0.1f;
        camScript.isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
                              staminaBar.CurrentStamina >= 0.1f;

        // Detección de modo
        if (currentMode == ControlMode.FirstPerson)
        {
            HandleRotationInput_FirstPerson();
            _MechanicsLookBack();
        }
        else if (currentMode == ControlMode.Tank)
        {
            HandleRotationInput_Tank();
        }

        // Inputs crudos al Animator
        animator.SetFloat("InputX", moveDirection.x, 0.15f, Time.deltaTime);
        animator.SetFloat("InputY", moveDirection.z, 0.15f, Time.deltaTime);

        bool runningCheck = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && moveDirection.magnitude > 0.1f;
        animator.SetBool("IsRunning", runningCheck);

        if (moveDirection.magnitude > 0.1f)
        {
            footstepSystem.HandleFootsteps(Move, runningCheck);
        }
    }

    void FixedUpdate()
    {
        if (currentMode == ControlMode.FirstPerson)
        {
            HandleMovement_FirstPerson();
            HandleRotation_FirstPerson();
        }
        else if (currentMode == ControlMode.Tank)
        {
            HandleMovement_Tank();
            HandleRotation_Tank();
        }
    }

    // -------------------------
    // FIRST PERSON MODE
    // -------------------------
    private void HandleMovement_FirstPerson()
    {
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shiftPressed && staminaBar.CurrentStamina > 0)
        {
            isRunning = true;
            currentSpeed = sprintSpeed;
        }
        else
        {
            isRunning = false;
            currentSpeed = moveSpeed;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveDirection = (transform.forward * z + transform.right * x).normalized;
        Vector3 moveVelocity = moveDirection * currentSpeed;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;
    }

    private void HandleRotationInput_FirstPerson()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        rotationInput = new Vector2(mouseX, mouseY);
    }

    private void HandleRotation_FirstPerson()
    {
        transform.Rotate(Vector3.up * rotationInput.x);

        xRotation -= rotationInput.y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        float yaw = lookBackYawOffset;
        cameraTransform.localRotation = Quaternion.Euler(xRotation, yaw, 0f);
    }

    // -------------------------
    // TANK MODE
    // -------------------------
    private void HandleMovement_Tank()
    {
        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shiftPressed && staminaBar.CurrentStamina > 0)
        {
            isRunning = true;
            currentSpeed = sprintSpeed;
        }
        else
        {
            isRunning = false;
            currentSpeed = moveSpeed;
        }

        float z = Input.GetAxis("Vertical"); // Adelante/atrás
        moveDirection = transform.forward * z;
        Vector3 moveVelocity = moveDirection * currentSpeed;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;
    }

    private void HandleRotationInput_Tank()
    {
        rotationInput.x = Input.GetAxis("Horizontal"); // Izquierda/Derecha con A/D o stick
    }

    private void HandleRotation_Tank()
    {
        transform.Rotate(Vector3.up * rotationInput.x * tankTurnSpeed * Time.fixedDeltaTime);
    }

    // -------------------------
    // UTILIDADES
    // -------------------------
    private void _MechanicsLookBack()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1))
        {
            lookBackYawOffset = 180f;
            bodyPlayer.SetActive(false);
        }
        if (Input.GetKeyUp(KeyCode.E) || Input.GetMouseButtonUp(1))
        {
            lookBackYawOffset = 0f;
            bodyPlayer.SetActive(true);
        }
    }
}
