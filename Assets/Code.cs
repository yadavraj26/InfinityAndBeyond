using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Code : MonoBehaviour
{
    public int width, height;
    public GameObject quadToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Instantiate(quadToSpawn, new Vector3(x, 0, y), gameObject.transform.rotation );
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
