using System.Threading;
using Cysharp.Threading.Tasks;
using Ib_Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
public class ShowTestUIInfo : Ib_Event<ShowTestUIInfo,string>{}
public class PlayerController : MonoBehaviour
{
    private PlayerMoveInput playerInput;
    private Rigidbody rb;
    private Vector2 moveDirection;
    private CapsuleCollider capsuleCollider;
    
    public Animator animator;
    private float huaChanTime = -1f;

    [Header("设置")]
    [SerializeField] private float moveSpeed = 50f;
    
    [BoxGroup] [SerializeField] private float interactRadius = 5f;
    [BoxGroup] [SerializeField] private LayerMask interactLayer;
    private void Awake()
    {
        playerInput = new PlayerMoveInput();
        TryGetComponent(out rb);
        TryGetComponent(out capsuleCollider);
        if(rb != null) rb.constraints = RigidbodyConstraints.FreezeRotation;
        playerInput.Player.Interact.performed += OnInteract;
    }

    private void OnEnable()
    { 
        playerInput.Enable();
        OnRotatePlayerHub.Register(ChangeDirHandler);
    }
    private void OnDisable()
    {
        playerInput.Disable();
        OnRotatePlayerHub.Deregister(ChangeDirHandler);
    }

    private Dir currentDir = Dir.Forward;
    private void ChangeDirHandler(Dir dir, Vector3 pos) => currentDir = dir;
    private void Update()
    {
        moveDirection = playerInput.Player.Move.ReadValue<Vector2>();
        if(animator)animator.SetFloat("qindao", rb.velocity.x);
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if (rb == null) return;
        if (Time.time - huaChanTime < 0.5f) return;
        Vector3 forceDir = new Vector3(moveDirection.x, 0, moveDirection.y);

        // 根据当前方向旋转力的方向
        float angle = (int)currentDir * 90f;
        forceDir = Quaternion.Euler(0, angle, 0) * forceDir;

        rb.AddForce(forceDir * moveSpeed, ForceMode.Force);
    }
    private Collider[] buffer = new Collider[10];
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (Time.time - huaChanTime > 1.75f)
        {
            animator?.SetTrigger("HuaChan");
            rb.AddForce(Vector3.forward * 150, ForceMode.Impulse);
            // AudioManager.Instance?.PlayOneShot(AudioManager.Instance.audioClips[3],0,1);

            if (capsuleCollider)
            {
                capsuleCollider.center = new Vector3(0, 1, 0);
                capsuleCollider.radius = 1;
                
                Ib_Async.DelayDoSomething(0.8f, () =>
                {
                    capsuleCollider.radius = 0.5f;
                    capsuleCollider.center = new Vector3(0, 0.5f, 0);
                },this.GetCancellationTokenOnDestroy());
            }
            huaChanTime = Time.time;
        }
        // var cnt = Physics.OverlapSphereNonAlloc(transform.position, interactRadius, buffer,interactLayer);
        // for (int i = 0; i < cnt; i++)
        // {
        //     var col = buffer[i];
        //     if (col.TryGetComponent(out UFOController ufoController))
        //     {
        //         if(ufoController.Interact())
        //             ShowTestUIInfo.Invoke("Bingo!");
        //         else 
        //             ShowTestUIInfo.Invoke("No!");
        //     }
        // }
    }
}