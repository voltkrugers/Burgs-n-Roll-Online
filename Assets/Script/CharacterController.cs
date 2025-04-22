using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterController : NetworkBehaviour, IKitchenObjParent
{

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPickedSomething;

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }
    public static CharacterController LocalInstance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged; 
    public event EventHandler OnPickedSomething; 
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    
    [SerializeField]private float speed = 9f;
    [SerializeField]private LayerMask countersLayerMask ;
    [SerializeField]private LayerMask CollisionsLayerMask ;
    [SerializeField] private Transform kitchenObjHoldPoint;
    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private PlayerVisual playerVisual;
    
    
    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObj kitchenObj;
    

    private void Start()
    {
        GameInput.Instance.OnInteractAction += OnInteractAction;
        GameInput.Instance.OnSecondInteractAction += OnSecondInteractAction;
        PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = spawnPositionList[KitchenGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        
        OnAnyPlayerSpawned?.Invoke(this,EventArgs.Empty);
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnclientDisconnectCallback;
        }
    }

    private void NetworkManager_OnclientDisconnectCallback(ulong ClientId)
    {
        if (ClientId == OwnerClientId && HasKitchenObj())
        {
            KitchenObj.DestroyKitchenObject(GetKitchenObj());
        }
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        HandleMovement();
        HandleInteraction();
    }
    
    private void OnSecondInteractAction(object sender, EventArgs e)
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter != null)
        {
            selectedCounter.SecondInteract(this);
        }
    }
    private void OnInteractAction(object sender, EventArgs e)
    {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;
        if (selectedCounter!=null)
        {
            selectedCounter.Interact(this);
        }
    }
    
    public bool GetIsWalking()
    {
        return isWalking;
    }

    private void HandleInteraction()
    {
        Vector2 inputVector = GameInput.Instance.GetMouvementVector();
        Vector3 moveDir = new Vector3(inputVector.x, 0f,inputVector.y );
        float interactDistance = 2f;

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        if (Physics.Raycast(transform.position,lastInteractDir,out RaycastHit raycastHit,interactDistance,countersLayerMask))
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
        Vector2 inputVector = GameInput.Instance.GetMouvementVector();
        Vector3 moveDir = new Vector3(inputVector.x, 0f,inputVector.y );

        float moveDistance = speed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2F;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius , moveDir,Quaternion.identity, moveDistance,CollisionsLayerMask);
        
        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);
            canMove = moveDir.x!=0 &&!Physics.BoxCast(transform.position, Vector3.one * playerRadius , moveDirX,Quaternion.identity, moveDistance,CollisionsLayerMask);
            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);
                canMove = moveDir.z!=0 && !Physics.BoxCast(transform.position, Vector3.one * playerRadius , moveDirZ,Quaternion.identity, moveDistance,CollisionsLayerMask);
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

        if (kitchenObj!=null)
        {
            OnPickedSomething?.Invoke(this,EventArgs.Empty);
            OnAnyPickedSomething?.Invoke(this,EventArgs.Empty);
        }
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

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
