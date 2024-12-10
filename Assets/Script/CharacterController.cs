using System;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    
    public static CharacterController Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged; 
    
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }
    
    [SerializeField]private float speed = 9f;
    [SerializeField]private GameInput _gameInput ;
    [SerializeField]private LayerMask countersLayerMask ;
    private bool isWalking;
    private Vector3 lastInteractDir;
    private ClearCounter selectedCounter;

    private void Awake()
    {
        if (Instance !=null)
        {
            Debug.LogError("error Instance");
        }
        Instance = this;
    }

    private void Start()
    {
        _gameInput.OnInteractAction += OnInteractAction;
    }

    void Update()
    {
       HandleMovement();
       HandleInteraction();
    }
    
    private void OnInteractAction(object sender, EventArgs e)
    {
        Vector2 inputVector = _gameInput.GetMouvementVector();
        Vector3 moveDir = new Vector3(inputVector.x, 0f,inputVector.y );
        float interactDistance = 2f;

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        if (Physics.Raycast(transform.position,lastInteractDir,out RaycastHit raycastHit,interactDistance,countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                clearCounter.Interact();
            }
        }
    }
    
    public bool GetIsWalking()
    {
        return isWalking;
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = _gameInput.GetMouvementVector();
        Vector3 moveDir = new Vector3(inputVector.x, 0f,inputVector.y );
        float interactDistance = 2f;

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        if (Physics.Raycast(transform.position,lastInteractDir,out RaycastHit raycastHit,interactDistance,countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if (clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = _gameInput.GetMouvementVector();
        Vector3 moveDir = new Vector3(inputVector.x, 0f,inputVector.y );

        float moveDistance = speed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2F;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }
        if (canMove)
        {
            transform.position += moveDir * Time.deltaTime * speed;
        }
        
        isWalking = moveDir != Vector3.zero;

        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime*rotationSpeed);
    }

    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
        
    }
}
