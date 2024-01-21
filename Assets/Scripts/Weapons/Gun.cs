using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public int bulletSpeed;

    //private int gunPower;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Fire(int gunPower)
    {
        GameObject bulletClone = Instantiate(bullet, transform.position, transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        bulletClone.GetComponent<Bullet>().bulletPower = gunPower;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
