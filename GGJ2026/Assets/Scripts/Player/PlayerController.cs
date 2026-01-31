using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private Rigidbody rb;
    private Vector2 moveDirection;

    [Header("设置")]
    [SerializeField] private float moveSpeed = 50f;
    
    private void Awake()
    {
        playerInput = new PlayerInput();
        TryGetComponent(out rb);
        
        // 确保父物体物理旋转是锁定的
        if(rb != null) rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void OnEnable() => playerInput.Enable();
    private void OnDisable() => playerInput.Disable();

    private void Update()
    {
        moveDirection = playerInput.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if (rb == null) return;
        Vector3 forceDir = new Vector3(moveDirection.x, 0, moveDirection.y);
        rb.AddForce(forceDir * moveSpeed, ForceMode.Force);
    }
}