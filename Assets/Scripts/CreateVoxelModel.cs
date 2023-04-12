using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateVoxelModel : MonoBehaviour
{
    public GameObject box;
    public float size;
    public float collisionSize;

    void Start()
    {
        Voxel();
    }
    void Voxel()
    {
        GameObject[] voxels = GameObject.FindGameObjectsWithTag("Voxel");
        foreach (GameObject o in voxels)
        {
            Destroy(o);
        }

        // box.transform.localScale = new Vector3(size, size, size);
        box.transform.localScale = Vector3.one * size;
        for (float y = -10; y < 10; y += size)
        {
            for (float x = -10; x < 10; x += size)
            {
                for (float z = -10; z < 10; z += size)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    if (Physics.CheckSphere(pos, collisionSize))
                    {
                        Instantiate(box, pos, Quaternion.identity, this.transform);
                    }
                }
            }
        }
    }
}
