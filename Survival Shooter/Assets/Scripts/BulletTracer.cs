using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    Vector3 target;

    [SerializeField] float speed = 40f;

    public void Init(Vector3 targetPosition)
    {
        target = targetPosition;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}