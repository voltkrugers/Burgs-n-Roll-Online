using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterController : MonoBehaviour, IKitchenObjParent
{
    public static CharacterController Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged; 
    
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    
    [SerializeField]private float speed = 9f;
    [SerializeField]private GameInput _gameInput ;
    [SerializeField]private LayerMask countersLayerMask ;
    [SerializeField] private Transform kitchenObjHoldPoint;
    
    private bool isWalking;
    private bool isStunned;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObj kitchenObj;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Error: Multiple Instances");
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _gameInput.OnInteractAction += OnInteractAction;
        _gameInput.OnSecondInteractAction += OnSecondInteractAction;
    }

    void Update()
    {
        if (!isStunned)
        {
            HandleMovement();
            HandleInteraction();
        }
    }

    private void OnSecondInteractAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.SecondInteract(this);
        }
    }

    private void OnInteractAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    public bool GetIsWalking()
    {
        return isWalking;
    }
    public void StunCharacter(float stunDuration)
    {
        StartCoroutine(StunCoroutine(stunDuration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = _gameInput.GetMouvementVector();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float interactDistance = 2f;

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
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
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = speed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2F;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
        if (!isStunned)
        {
            if (!canMove)
            {
                Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
                canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirX;
                }
                else
                {
                    Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                    canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
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
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
        }
        

    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjFollowTransform()
    {
        return kitchenObjHoldPoint;
    }

    public void SetKitchenObject(KitchenObj kitchenObj)
    {
        this.kitchenObj = kitchenObj;
    }

    public KitchenObj GetKitchenObj()
    {
        return kitchenObj;
    }

    public void ClearKitchenObject()
    {
        kitchenObj = null;
    }

    public bool HasKitchenObj()
    {
        return kitchenObj != null;
    }
}