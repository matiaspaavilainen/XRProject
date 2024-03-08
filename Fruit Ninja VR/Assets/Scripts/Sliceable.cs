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
        gameObject.tag = "sliceable";
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject sword = collision.gameObject;
        Sword swordScript = sword.GetComponent<Sword>();

        // Check if the object that hit this one is the sword
        if (sword.CompareTag("blade"))
        {
            // Player must swing the sword
            if (swordScript.Velocity.magnitude > 1f)
            {
                Vector3 contactPoint = collision.contacts[0].point;
                SliceObject(contactPoint, swordScript);
            }

        } else if (sword.CompareTag("ground"))
        {
            startGame.strikes++;
            startGame.UpdateScoreText();
            Destroy(gameObject, 0f);
        }
    }

    void SliceObject(Vector3 contactPoint, Sword swordScript)
    {
        startGame.points++;
        startGame.UpdateScoreText();

        // Get the position and direction of the slice
        Vector3 position = contactPoint;
        Vector3 direction = Vector3.Cross(swordScript.Velocity.normalized, Vector3.up);

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

            upperHalf.tag = "sliceable";
            lowerHalf.tag = "sliceable";

            // Add rigidbodies to the halves so they fall under gravity
            Rigidbody upperRb = upperHalf.AddComponent<Rigidbody>();
            Rigidbody lowerRb = lowerHalf.AddComponent<Rigidbody>();

            // Add mesh colliders to the halves
            upperHalf.AddComponent<MeshCollider>().convex = true;
            lowerHalf.AddComponent<MeshCollider>().convex = true;
            float force = 1f;

            upperRb.AddForce(direction * force, ForceMode.Impulse);
            lowerRb.AddForce(-direction * force, ForceMode.Impulse);

            // Destroy the original object
            Destroy(gameObject);
        }
    }
}