using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelTest_BulletBase : MonoBehaviour
{
    public float speed = 5f;
    protected Vector3 target;

    public void Initialize(Vector3 targetPosition)
    {
        // target = targetPosition;
        target = (targetPosition - transform.position).normalized;
    }

    protected virtual void Update()
    {
        MoveBullet();
    }

    private void MoveBullet()
    {
        // transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        transform.position += target * speed * Time.deltaTime;
    }

    protected abstract void OnTriggerEnter2D(Collider2D collision);

    protected IEnumerator DestroyBulletAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}