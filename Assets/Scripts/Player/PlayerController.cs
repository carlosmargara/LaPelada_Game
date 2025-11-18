using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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

    [Header("Sensibilidad")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float gamepadSensitivity = 40f;

    [Header("Step Climb Settings")]
    [SerializeField] private float stepHeight = 0.3f;      // Altura máxima del escalón
    [SerializeField] private float stepSmooth = 6f;        // Velocidad de subida
    [SerializeField] private float stepCheckDistance = 0.5f; // Distancia del raycast frontal
    [SerializeField] private Transform stepRayLower;       // Posición del raycast inferior
    [SerializeField] private Transform stepRayUpper;       // Posición del raycast superior

    [Space]
    private StaminaBar staminaBar;
    [Space]
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

    [HideInInspector] public bool IsRunning { get; private set; }

    // ---------- NEW INPUT SYSTEM (PlayerInput) ----------
    private PlayerInput playerInput;           // componente PlayerInput
    private InputAction moveAction;            // action "Move"
    private InputAction lookAction;            // action "Look"
    private InputAction runAction;             // action "Run"
    private InputAction lookBackAction;       // action "MirrarAtras"
    private Vector2 moveInput = Vector2.zero; // valor leído
    private Vector2 lookInput = Vector2.zero; // valor leído

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        camScript = GetComponent<Cinemachine_Headbob_And_Noise>();

        // Obtener el componente PlayerInput (asegurate de tenerlo agregado al GameObject)
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput component no encontrado en el GameObject. Agregalo o usá la clase generada.");
        }
        else if (playerInput.actions != null)
        {
            // Buscar las actions por nombre dentro del asset (suponiendo que se llaman "Move" y "Look")
            moveAction = playerInput.actions.FindAction("Move", true);
            lookAction = playerInput.actions.FindAction("Look", true);
            runAction = playerInput.actions.FindAction("Run", true);
            lookBackAction = playerInput.actions.FindAction("LookBack", true);

            if (moveAction == null) Debug.LogWarning("No se encontró la action 'Move' en el Input Action Asset.");
            if (lookAction == null) Debug.LogWarning("No se encontró la action 'Look' en el Input Action Asset.");
            if (runAction == null) Debug.LogWarning("No se encontró la action 'Run'.");
            if (lookBackAction == null) Debug.LogWarning("No se encontró la action 'LookBack'.");
        }

    }

    private void OnEnable()
    {
        // Habilitamos acciones por si hace falta (PlayerInput normalmente habilita el default map)
        moveAction?.Enable();
        lookAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        lookAction?.Disable();
    }

    public void OnRun(InputValue value)
    {
        IsRunning = value.isPressed;
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        animator = GetComponentInChildren<Animator>();
        currentSpeed = moveSpeed;

        if (rb != null)
            rb.freezeRotation = true;

        staminaBar = StaminaBar.Instance;

        if (MouseSensitivityManager.Instance != null)
            mouseSensitivity = MouseSensitivityManager.Instance.CurrentSensitivity;
    }

    void Update()
    {
        // --- Leer inputs desde las actions (si existen) ---
        moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        lookInput = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;
        IsRunning = runAction != null && runAction.IsPressed();

        camScript.isMoving = moveDirection.magnitude > 0.1f;
        camScript.isRunning = IsRunning && staminaBar.CurrentStamina >= 0.1f;

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

        // Inputs al Animator
        animator.SetFloat("InputX", moveDirection.x, 0.15f, Time.deltaTime);
        animator.SetFloat("InputY", moveDirection.z, 0.15f, Time.deltaTime);
        animator.SetBool("IsRunning", IsRunning && moveDirection.magnitude > 0.1f);

        // --- LLAMADA AL SISTEMA DE PASOS ---
        bool runningCheck = IsRunning && moveDirection.magnitude > 0.1f;
        footstepSystem.HandleFootsteps(Move, runningCheck);

        _MechanicsLookBack();
    }

    void FixedUpdate()
    {
        if (currentMode == ControlMode.FirstPerson)
        {
            HandleMovement_FirstPerson();
            HandleRotation_FirstPerson();
            StepClimb();
        }
        else if (currentMode == ControlMode.Tank)
        {
            HandleMovement_Tank();
            HandleRotation_Tank();
            StepClimb();
        }
    }

    // -------------------------
    // FIRST PERSON MODE
    // -------------------------
    private void HandleMovement_FirstPerson()
    {
        if (IsRunning && staminaBar.CurrentStamina > 0)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            IsRunning = false;
            currentSpeed = moveSpeed;
        }

        float x = moveInput.x;
        float z = moveInput.y;

        moveDirection = (transform.forward * z + transform.right * x).normalized;
        Vector3 moveVelocity = moveDirection * currentSpeed;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;
    }

    private void HandleRotationInput_FirstPerson()
    {
        Vector2 look = lookInput;

        // Detectar si el dispositivo actual es un gamepad o un mouse
        bool isGamepad = Gamepad.current != null && Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0.01f;
        bool isMouse = Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f;

        float sensitivity = mouseSensitivity;

        if (isGamepad)
            sensitivity = gamepadSensitivity;
        else if (isMouse)
            sensitivity = mouseSensitivity;

        // ❌ NO usamos Time.deltaTime acá
        float mouseX = look.x * rotationSpeed * sensitivity;
        float mouseY = look.y * rotationSpeed * sensitivity;

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
        if (IsRunning && staminaBar.CurrentStamina > 0)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            IsRunning = false;
            currentSpeed = moveSpeed;
        }

        float z = moveInput.y; // Adelante/atrás
        moveDirection = transform.forward * z;
        Vector3 moveVelocity = moveDirection * currentSpeed;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;
    }

    private void HandleRotationInput_Tank()
    {
        rotationInput.x = moveInput.x; // Izquierda/Derecha con A/D o stick
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
        if (lookBackAction != null)
        {
            if (lookBackAction.WasPressedThisFrame())
            {
                lookBackYawOffset = 180f;
                bodyPlayer.SetActive(false);
            }
            else if (lookBackAction.WasReleasedThisFrame())
            {
                lookBackYawOffset = 0f;
                bodyPlayer.SetActive(true);
            }
        }
    }

    public void SetMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
    }

    private void StepClimb()
    {
        // 🔸 Solo intentamos subir si el jugador se está moviendo
        if (moveInput.magnitude < 0.1f) return;

        // Rayo bajo (a la altura de los pies)
        if (Physics.Raycast(stepRayLower.position, transform.forward, out RaycastHit lowerHit, stepCheckDistance))
        {
            // Rayo alto (a la altura de la rodilla)
            if (!Physics.Raycast(stepRayUpper.position, transform.forward, stepCheckDistance))
            {
                // Subida suave del Rigidbody
                rb.position += new Vector3(0f, stepSmooth * Time.fixedDeltaTime, 0f);
            }
        }
    }
}
