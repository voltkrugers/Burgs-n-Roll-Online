using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]private float speed = 9f;
    [SerializeField]private GameInput _gameInput ;
    private bool isWalking;
    void Update()
    {
        Vector2 inputVector = _gameInput.GetMouvementVector();
        Vector3 moveDir = new Vector3(inputVector.x, 0f,inputVector.y );
        transform.position += moveDir * Time.deltaTime * speed;

        isWalking = moveDir != Vector3.zero;

        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime*rotationSpeed);
    }

    public bool GetIsWalking()
    {
        return isWalking;
    }
}
