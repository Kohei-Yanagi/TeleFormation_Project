using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class RayTriggerPersonalSpace : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    public InputHelpers.Button triggerButton = InputHelpers.Button.Trigger;
    public float activationThreshold = 0.1f;

    private bool triggerHeld = false;
    private InputDevice inputDevice;

    void Start()
    {
        // どちらの手にアタッチされているかを自動で取得（右手前提。左手の場合は変更）
        var characteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(characteristics, devices);

        if (devices.Count > 0)
        {
            inputDevice = devices[0];
        }
        else
        {
            Debug.LogWarning("No input device found for right hand controller.");
        }
    }

    void Update()
    {
        if (!inputDevice.isValid)
            return;

        InputHelpers.IsPressed(inputDevice, triggerButton, out bool isPressed, activationThreshold);

        if (isPressed && !triggerHeld)
        {
            triggerHeld = true;

            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                var target = hit.transform.GetComponent<PersonalSpaceActivator>();
                if (target != null)
                    target.TogglePersonalSpace();
            }
        }
        else if (!isPressed && triggerHeld)
        {
            triggerHeld = false;
        }
    }
}
