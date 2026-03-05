using UnityEngine;
using UnityEngine.InputSystem;


public class TeleportByAButton : MonoBehaviour
{
    [Header("XR Interaction")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    public UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationProvider teleportationProvider;

    private InputAction teleportAction;

    void Awake()
    {
        // PICO 向け：右手コントローラーの Aボタン（またはボタン南）
        teleportAction = new InputAction(
            name: "TeleportAButton",
            type: InputActionType.Button,
            binding: "<XRController>{RightHand}/primaryButton" // PICOのAボタンに対応
        );

        
        // binding: "<XRController>{RightHand}/primaryButton"
    }

    void OnEnable()
    {
        teleportAction.Enable();
    }

    void OnDisable()
    {
        teleportAction.Disable();
    }

    void Update()
    {
        if (teleportAction.WasPressedThisFrame())
        {
            TryTeleport();
        }
    }

    private void TryTeleport()
    {
        if (rayInteractor != null &&
            teleportationProvider != null &&
            rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            var teleportRequest = new UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportRequest
            {
                destinationPosition = hit.point,
            };

            teleportationProvider.QueueTeleportRequest(teleportRequest);
        }
    }
}

