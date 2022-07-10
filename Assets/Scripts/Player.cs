using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public float speed = 10f;
    public float rotationSpeed = 2f;
    public GameObject bullet;
    public float shootingTime = 0.5f;

    private Vector3 movement;
    private Vector3 velocity = Vector3.zero;
    private ObjectPool<GameObject> bulletPool;
    private Tail tail;
    private float radius;

    void Start()
    {
        Instance = this;

        tail = GetComponentInChildren<Tail>();
        radius = transform.localScale.x;

        InitializeBulletPool();

        InvokeRepeating("Shoot", shootingTime, shootingTime);
    }

    private void InitializeBulletPool()
    {
        bulletPool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(bullet);
        }, asteroid =>
        {
            asteroid.gameObject.SetActive(true);
        }, asteroid =>
        {
            asteroid.gameObject.SetActive(false);
        }, asteroid =>
        {
            Destroy(asteroid.gameObject);
        },
        false, 7, 10
        );
    }

    void Update()
    {
        Movement();
        CheckDeath();
    }

    private void Movement()
    {
        transform.eulerAngles += Vector3.forward * -Input.GetAxis("Horizontal") * rotationSpeed;

        movement = Vector3.SmoothDamp(movement, transform.up * -Input.GetAxis("Vertical"), ref velocity, 0.3f);
        transform.position = -movement;
        tail.trailDist = movement;
        Unit.extraMovement = movement * speed;
    }

    private void CheckDeath()
    {
        if (GridMechanics.Instance.CheckCollisionOnPosition(transform.position, radius))
        {
            tail.gameObject.SetActive(false);
            GetComponent<SpriteRenderer>().enabled = false;     //Does it count as destroyed ship?
            UIManager.Instance.Lose();
            Time.timeScale = 0f;
        }
    }

    void Shoot()
    {
        var tempBullet = bulletPool.Get();
        tempBullet.transform.eulerAngles = transform.eulerAngles;
        tempBullet.transform.position = transform.position;
    }

    public void ReleaseBullet(GameObject bullet)
    {
        bulletPool.Release(bullet);
    }
}
