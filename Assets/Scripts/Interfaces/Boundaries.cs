using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Boundaries
{
    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;

    public Boundaries(float minX, float maxX, float minZ, float maxZ)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minZ = minZ;
        this.maxZ = maxZ;
    }

    public Boundaries()
    {
        this.minX = 0;
        this.maxX = 0;
        this.minZ = 0;
        this.maxZ = 0;
    }

    public void GetBoundaries(GameObject gameObject)
    {
        Collider selfCollider = gameObject.GetComponent<Collider>();

        Collider deskCollider = gameObject.GetComponent<Collider>();

        this.minX = deskCollider.bounds.min.x;
        this.maxX = deskCollider.bounds.max.x;
        this.minZ = deskCollider.bounds.min.z;
        this.maxZ = deskCollider.bounds.max.z;
    }

    public Vector2 GetRandomPoint()
    {
        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);

        return new Vector2(randomX, randomZ);
    }

    public float GetMinX()
    {
        return minX;
    }

    public float GetMaxX()
    {
        return maxX;
    }

    public float GetMinZ()
    {
        return minZ;
    }

    public float GetMaxZ()
    {
        return maxZ;
    }
}
