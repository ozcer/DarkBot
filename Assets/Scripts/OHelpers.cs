using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OHelpers
{
    public static Vector2 Change(this Vector2 org, object x = null, object y = null, object z = null)
    {
        return new Vector2(
            (x == null ? org.x: (float) x), 
            (y == null ? org.y: (float) y));
    }
    
    public static Vector3 Change(this Vector3 org, object x = null, object y = null, object z = null)
    {
        return new Vector3(
            (x == null ? org.x: (float) x), 
            (y == null ? org.y: (float) y),
            (z == null ? org.z: (float) z));
    }
    
    public static Vector3 DChange(this Vector3 org, object x = null, object y = null, object z = null)
    {
        return new Vector3(
            (x == null ? org.x: org.x + (float) x), 
            (y == null ? org.y: org.y + (float) y),
            (z == null ? org.z: org.z +(float) z));
    }

    public static T GetRandom<T>(this List<T> og)
    {
        return og[Random.Range(0, og.Count)];
    }

    public static Bounds CombinedBounds(this Transform og)
    {
        Bounds combinedBounds = new Bounds(og.position, Vector3.one);
        Renderer[] colliders = og.GetComponentsInChildren<Renderer>();
        foreach(Renderer collider in colliders) {
            combinedBounds.Encapsulate(collider.bounds);
        }

        return combinedBounds;
    }

    public static bool IsWithin(this int self, int left, int right)
    {
        return self >= left && self <= right;
    }
    
}