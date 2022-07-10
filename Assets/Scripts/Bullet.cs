using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;
    public float speed = 15f;
    private IEnumerator dying;
    private float radius;

    private void Start()
    {
        radius = transform.localScale.x / 2;
    }

    private void OnEnable()
    {
        dying = Die();
        StartCoroutine(dying);
    }

    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        if (GridMechanics.Instance.CheckCollisionOnPosition(transform.position, radius))
        {
            UIManager.Instance.IncreaseScore(10);
            StopCoroutine(dying);
            Player.Instance.ReleaseBullet(this.gameObject);
        }
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(lifetime);
        Player.Instance.ReleaseBullet(this.gameObject);
    }
}
