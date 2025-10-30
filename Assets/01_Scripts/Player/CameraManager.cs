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
    [SerializeField]
    private Transform tf;

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
        targetPosition.z = tf.position.z;
    }

    public void SetView(Vector3 _targetPos)
    {
        targetPosition = _targetPos;
        targetPosition.z = tf.position.z;
    }

    private void MoveCam()
    {
        targetPosition = target.position;
        targetPosition.z = tf.position.z;
        tf.position = Vector3.Lerp(tf.position, targetPosition, moveSpeed);

        float clampX = Mathf.Clamp(tf.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);
        float clampY = Mathf.Clamp(tf.position.y, minBound.y + halfHeight, maxBound.y - halfHeight);

        tf.position = new Vector3(clampX, clampY, tf.position.z);
    }
}
