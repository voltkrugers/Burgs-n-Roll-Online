using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IABird : MonoBehaviour, IKitchenObjParent
{
    public float detectionRadius = 5f;
    public LayerMask objectLayerMask;
    public Transform[] deliveryPoints;
    public Transform ObjPoint;
    [SerializeField] private Transform currentTarget;
    [SerializeField] private KitchenObj currentObject;
    private bool hasObject;
    private float yOffset = 2.5f; 
    private KitchenObj lastDroppedObject = null; 
    private Vector3 randomTarget;
    private float randomTargetTimer = 0f;
    private float randomTargetInterval = 5f;
    [SerializeField] private Transform visualTransform;

    void Update()
    {
        if (!hasObject)
        {
            DetectAndPickUpObject();
        }
        else
        {
            DeliverObject();
        }
    }

    private void DetectAndPickUpObject()
    {

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, objectLayerMask);
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out KitchenObj kitchenObj) )
            {
                currentTarget = kitchenObj.transform;
                MoveToTarget();
                return;
            }
        }
        if (currentTarget == null)
        {
            MoveRandomly();
        }
    }
    private void MoveRandomly()
    {
        if (randomTargetTimer <= 0f)
        {
            SetRandomTarget();
            randomTargetTimer = randomTargetInterval;
        }

        randomTargetTimer -= Time.deltaTime;
    
        float step = 2f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, randomTarget, step);
    }

    private void SetRandomTarget()
    {
        randomTarget = new Vector3(Random.Range(-10, 10), transform.position.y, Random.Range(-10, 2));
    }

    public void PickUpObject(KitchenObj obj)
    {
        obj.transform.SetParent(transform); 
        hasObject = true;
        currentObject = obj;
        currentObject.transform.localPosition = new Vector3(0, -1, 0); // Ajuste la position relative si besoin
        currentTarget = null;
    }

    private void DeliverObject()
    {
        if (hasObject && currentTarget == null)
        {
            int index = Random.Range(0, deliveryPoints.Length);
            currentTarget = deliveryPoints[index];
        }
        MoveToTarget();
    }
    private void MoveToTarget()
    {
        if (currentTarget != null)
        {
            Vector3 adjustedTargetPosition = new Vector3(currentTarget.position.x, currentTarget.position.y + yOffset, currentTarget.position.z);
            float step = 2f * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, adjustedTargetPosition, step);
        
            Vector3 direction = (adjustedTargetPosition - transform.position).normalized; // Calcule la direction
            if (direction.magnitude > 0.1f) // Évite les changements de rotation inutiles
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, step * 5f); // Ajuste le facteur pour une rotation plus rapide si nécessaire
            }

            if (Vector3.Distance(transform.position, adjustedTargetPosition) < 0.5f)
            {
                if (!hasObject && currentTarget.TryGetComponent(out KitchenObj kitchenObj) && kitchenObj != lastDroppedObject)
                {
                    PickUpObject(kitchenObj);
                    lastDroppedObject = null; 
                }
                else if (hasObject && currentTarget.GetComponent<ClearCounter>() != null)
                {
                    DropOffObject(); 
                }
            }
        }
    }

    private void DropOffObject()
    {
        if (currentObject != null && currentTarget != null)
        {
            currentObject.transform.SetParent(currentTarget);
            currentTarget.GetComponent<ClearCounter>().AllowBirdPickup(this);
            lastDroppedObject = currentObject;
            hasObject = false;
            currentObject = null;
            currentTarget = null;
            MoveRandomly();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public Transform GetKitchenObjFollowTransform()
    {
        return ObjPoint;
    }

    public void SetKitchenObject(KitchenObj kitchenObj)
    {
        this.currentObject = kitchenObj;
    }

    public KitchenObj GetKitchenObj()
    {
        return currentObject;
    }

    public void ClearKitchenObject()
    {
        currentObject = null;
    }

    public bool HasKitchenObj()
    {
        return currentObject != null;
    }
}
