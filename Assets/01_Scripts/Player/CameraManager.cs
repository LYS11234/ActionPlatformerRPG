using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Vector3 targetPosition;
    [SerializeField]
    private BoxCollider2D bound;
    [SerializeField]
    private Vector3 minBound;
    [SerializeField] 
    private Vector3 maxBound;

    private float halfWidth;
    private float halfHeight;

    private Camera thisCam;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        thisCam = GetComponent<Camera>();
        maxBound = bound.bounds.max;
        minBound = bound.bounds.min;
        halfHeight = thisCam.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;
    }

    private void Update()
    {
        if(target.IsUnityNull())
        {
            return;
        }
        MoveCam();
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
        targetPosition = target.position;
        targetPosition.z = transform.position.z;
    }

    public void SetView(Vector3 _targetPos)
    {
        targetPosition = _targetPos;
        targetPosition.z = transform.position.z;
    }

    private void MoveCam()
    {
        targetPosition = target.position;
        targetPosition.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed);

        float clampX = Mathf.Clamp(transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);
        float clampY = Mathf.Clamp(transform.position.y, minBound.y + halfHeight, maxBound.y - halfHeight);

        transform.position = new Vector3(clampX, clampY, transform.position.z);
    }
}
