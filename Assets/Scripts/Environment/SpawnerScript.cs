using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public Generator generatorRef;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SpawnObject(GameObject go, int size)
    { 
        bool spawned = false;
        while (!spawned)
        {
            int spawnHeight = Random.Range(0 + (size * 2), generatorRef.height - (size * 2));
            int spawnWidth = Random.Range(0 + (size * 2), generatorRef.width - (size * 2));
            bool hinderanceFlag=false;
            for (int i = spawnWidth - size; i <= spawnWidth + size; i++)
            {
                for (int j = spawnHeight - size; j <= spawnHeight + size; j++)
                {
                    if(generatorRef.canvas[i,j]==1)
                    {
                        hinderanceFlag = true;
                    }
                }
            }
            if(!hinderanceFlag)
            {
                go = Instantiate(go, new Vector3(spawnWidth, 0, spawnHeight), go.transform.rotation);
                spawned = true;
                if (go.CompareTag("objects"))
                {
                    for (int i = spawnWidth - size; i <= spawnWidth + size; i++)
                    {
                        for (int j = spawnHeight - size; j <= spawnHeight + size; j++)
                        {
                            generatorRef.canvas[i, j] = 1;
                        }
                    }
                }
            }
        }
        return go;

    }
}
