using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform player;
    private Vector3 initialRotationOffset;

    private Vector3 currentPositionOffset;
    private Quaternion currentRotationOffset;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        // transform.SetParent(player);
        transform.SetLocalPositionAndRotation(new Vector3(0f, 2.5f, 1.1f), Quaternion.Euler(0, 0, 0f));
        // currentPositionOffset = new Vector3(0f, 9f, -23f);
        // currentPositionOffset = new Vector3(0f, 3f, 0.9f);
        // currentPositionOffset = new Vector3(0f, 2f, 0.35f);
        // Debug.Log(transform.rotation);
        // currentRotationOffset = new Quaternion(0f, -1f, 0.13f, 0f);
        // currentRotationOffset = new Quaternion(0.13f, 0f, 0f, 1f);
        // currentRotationOffset = Quaternion.Euler(new Vector3(0f,2f,0.35f));
        
        currentPositionOffset = new Vector3(0f, 2.5f, 1.1f);
        currentRotationOffset = Quaternion.Euler(new Vector3(0f,0f,0f));

    }

    void LateUpdate()
    {
        if (player != null)
        {
            currentRotationOffset = Quaternion.Euler(initialRotationOffset + player.eulerAngles);
            Vector3 desiredPosition = player.position + currentRotationOffset * currentPositionOffset;

            transform.position = desiredPosition;
            transform.LookAt(player.position+ player.forward*5);
        }
    }
}