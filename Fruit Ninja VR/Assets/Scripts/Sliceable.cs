using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class Sliceable : MonoBehaviour
{
    public Material crossSectionMaterial; // Material to apply to the sliced surface
    private StartGame startGame;

    private void Start()
    {
        startGame = GameObject.FindAnyObjectByType<StartGame>();
        gameObject.layer = LayerMask.NameToLayer("whole");
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject cube = collision.gameObject;

        // Check if the object that hit this one is one of the sword edges
        if (cube.CompareTag("grabbable"))
        {
            // Call a method to slice this object
            SliceObject(cube);

        } else if (cube.CompareTag("ground"))
        {
            startGame.strikes++;
            Destroy(gameObject, 1f);
        }
    }

    void SliceObject(GameObject sword)
    {
        startGame.points++;
        startGame.UpdateScoreText();

        // Get the position and direction of the slice
        Vector3 position = sword.transform.position;
        Vector3 direction = sword.transform.right;

        // Use the Slicer class to slice the object
        SlicedHull hull = gameObject.Slice(position, direction, crossSectionMaterial);

        if (hull != null)
        {
            // Create the upper and lower halves of the sliced object
            GameObject upperHalf = hull.CreateUpperHull(gameObject, crossSectionMaterial);
            GameObject lowerHalf = hull.CreateLowerHull(gameObject, crossSectionMaterial);

            // Assign the sliced parts to their layer
            upperHalf.layer = LayerMask.NameToLayer("sliced");
            lowerHalf.layer = LayerMask.NameToLayer("sliced");

            // Add rigidbodies to the halves so they fall under gravity
            Rigidbody upperRb = upperHalf.AddComponent<Rigidbody>();
            Rigidbody lowerRb = lowerHalf.AddComponent<Rigidbody>();

            // Add mesh colliders to the halves
            upperHalf.AddComponent<MeshCollider>().convex = true;
            lowerHalf.AddComponent<MeshCollider>().convex = true;
            float force = 0.5f;

            upperRb.AddForce(direction * force, ForceMode.Impulse);
            lowerRb.AddForce(-direction * force, ForceMode.Impulse);

            // Destroy the original object
            Destroy(gameObject);

            Destroy(upperHalf, 2f);
            Destroy(lowerHalf, 2f);
        }
    }
}