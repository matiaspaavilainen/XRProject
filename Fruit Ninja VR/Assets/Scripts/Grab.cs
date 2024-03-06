
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomGrab : MonoBehaviour
{
    // This script should be attached to both controller objects in the scene
    // Make sure to define the input in the editor (LeftHand/Grip and RightHand/Grip recommended respectively)
    CustomGrab otherHand = null;
    public List<Transform> nearObjects = new List<Transform>();
    public Transform grabbedObject = null;
    public InputActionReference action;
    public bool isRightHand = false;
    bool grabbing = false;

    private Vector3 previousPos;
    private Quaternion previousRot;
    private Vector3 grabOffset;

    private void Start()
    {
        action.action.Enable();

        // Set the grab offset based on the hand
        grabOffset = isRightHand ? new Vector3(-0.01f, -0.02f, 0.008f) : new Vector3(0.01f, -0.02f, 0.008f);

        // Find the other hand
        foreach(CustomGrab c in transform.parent.GetComponentsInChildren<CustomGrab>())
        {
            if (c != this)
                otherHand = c;
        }
    }

    void Update()
{
    grabbing = action.action.IsPressed();
    if (grabbing)
    {
        if (!grabbedObject)
            grabbedObject = nearObjects.Count > 0 ? nearObjects[0] : otherHand.grabbedObject;

        if (grabbedObject)
        {
            Vector3 deltaPos = transform.position - previousPos;
            Quaternion deltaRot = transform.rotation * Quaternion.Inverse(previousRot);

            grabbedObject.position -= deltaPos; // Move object back by deltaPos
            grabbedObject.rotation = deltaRot * grabbedObject.rotation; // Apply rotation
            grabbedObject.position = transform.position + transform.TransformDirection(grabOffset); // Move object to controller
        }
    }
    else if (grabbedObject)
        grabbedObject = null;

    previousPos = transform.position;
    previousRot = transform.rotation;
}

    private void OnTriggerEnter(Collider other)
    {
        // Make sure to tag grabbable objects with the "grabbable" tag
        // You also need to make sure to have colliders for the grabbable objects and the controllers
        // Make sure to set the controller colliders as triggers or they will get misplaced
        // You also need to add Rigidbody to the controllers for these functions to be triggered
        // Make sure gravity is disabled though, or your controllers will (virtually) fall to the ground

        Transform t = other.transform;
        if(t && t.tag.ToLower()=="grabbable")
            nearObjects.Add(t);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform t = other.transform;
        if( t && t.tag.ToLower()=="grabbable")
            nearObjects.Remove(t);
    }
}