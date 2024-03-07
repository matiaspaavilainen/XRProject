using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandController : MonoBehaviour
{
    XRController controller;
    public Hand hand;

    void Start()
    {
        controller = GetComponent<XRController>();
    }

    void Update()
    {
        if (controller != null && hand != null)
        {
            InputDevice device = controller.inputDevice;

            if (device.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                hand.SetGrip(gripValue);
            }

            if (device.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                hand.SetTrigger(triggerValue);
            }
        }
    }
}