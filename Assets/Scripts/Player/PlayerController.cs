using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float rotationSpeed = 6.5f;
     

    [Header("Componentes")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody rb;

    private float currentSpeed;
    private Vector2 rotationInput;
    private float xRotation = 0f;
    private float lookBackYawOffset = 0f;

    private Vector3 moveDirection;
    public Vector3 Move => moveDirection;

    [Space]
    //private PlayerFootsteps playerFootsteps;
    private Cinemachine_Headbob_And_Noise camScript;


    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private Different_types_of_Raycast_and_Tag_steps different_Types_Of_Raycast_And_Tag_Steps;

    [HideInInspector] public bool isRunning = false;
    
    private void Awake()
    {
        //playerFootsteps = GetComponent<PlayerFootsteps>();
        camScript = GetComponent<Cinemachine_Headbob_And_Noise>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = moveSpeed;

        if (rb != null)
            rb.freezeRotation = true;
       
    }

    void Update()
    {
        camScript.isMoving = moveDirection.magnitude > 0.1f;
        camScript.isRunning = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
                      staminaBar.currentStamina >= 0.1;

        HandleRotationInput();
        _MechanicsLookBack();

        
        if(moveDirection.magnitude > 0.1f)
        {
            Debug.Log("Se esta moveindo");
            different_Types_Of_Raycast_And_Tag_Steps.HandleFootsteps(rb, isRunning);            
        }
        
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Verificar si se puede correr
        bool shiftPressed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

        // El StaminaBar se encargará de limitar el correr según la stamina
        if (shiftPressed && staminaBar.currentStamina > 0 )
        {
            isRunning = true;
            currentSpeed = sprintSpeed;
            //playerFootsteps.SetRunning(true);
        }
        else
        {
            isRunning = false;
            currentSpeed = moveSpeed;
            //playerFootsteps.SetRunning(false);
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveDirection = (transform.forward * z + transform.right * x).normalized;
        Vector3 moveVelocity = moveDirection * currentSpeed;
        moveVelocity.y = rb.velocity.y;

        rb.velocity = moveVelocity;
    }

    private void HandleRotationInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        rotationInput = new Vector2(mouseX, mouseY);
    }

    private void HandleRotation()
    {
        transform.Rotate(Vector3.up * rotationInput.x);

        xRotation -= rotationInput.y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        float yaw = lookBackYawOffset; // 0 normalmente, 180 si se mira atrás
        cameraTransform.localRotation = Quaternion.Euler(xRotation, yaw, 0f);
    }
    
    private void _MechanicsLookBack()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1))
        {
            lookBackYawOffset = 180f;
        }
        if (Input.GetKeyUp(KeyCode.E) || Input.GetMouseButtonUp(1))
        {
            lookBackYawOffset = 0f;
        }
    }
}
