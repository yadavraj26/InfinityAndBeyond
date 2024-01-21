using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletPower;
    public float lifeTime=2.0f;

    private float elapsed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed>lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Protoss":
                collision.gameObject.GetComponent<Protoss>().UpdateHealth(-(bulletPower / 5));
                break;
            case "Zerg":
                collision.gameObject.GetComponent<ZergStarship>().UpdateHealth(-(bulletPower / 5));
                break;
            case "Terran":
                collision.gameObject.GetComponent<Terran>().UpdateHealth(-(bulletPower / 5));
                break;
            case "objects":
                Planet pl = collision.gameObject.GetComponent<Planet>();
                if(pl!=null)
                    pl.UpdateHealth(-(bulletPower / 5));
                break;
            default: Debug.Log("hit no one"); break;

        }
        Debug.Log("hit no one");
        Destroy(gameObject);
    }
}
