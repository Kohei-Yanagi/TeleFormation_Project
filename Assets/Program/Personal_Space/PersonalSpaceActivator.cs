using UnityEngine;

public class PersonalSpaceActivator : MonoBehaviour
{
    public GameObject personalSpacePrefab;
    private GameObject currentPersonalSpace;

    // 地面との最大距離
    public float maxRaycastDistance = 5f;

    // 位置オフセット
    public Vector3 personalSpaceOffset = Vector3.zero;

    // ✅ 回転オフセット（度数法で設定できるようにする）
    public Vector3 rotationOffset = Vector3.zero;

    public void ShowPersonalSpace()
    {
        if (currentPersonalSpace == null)
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance))
            {
                Vector3 spawnPos = hit.point + personalSpaceOffset;

                // ✅ Prefab の回転にオフセットを加算
                Quaternion spawnRot = personalSpacePrefab.transform.rotation * Quaternion.Euler(rotationOffset);

                currentPersonalSpace = Instantiate(personalSpacePrefab, spawnPos, spawnRot);
            }
            else
            {
                Debug.LogWarning("床が見つかりませんでした。パーソナルスペースは生成されません。");
            }
        }
    }

    public void HidePersonalSpace()
    {
        if (currentPersonalSpace != null)
        {
            Destroy(currentPersonalSpace);
            currentPersonalSpace = null;
        }
    }

    // 表示/非表示トグル
    public void TogglePersonalSpace()
    {
        if (currentPersonalSpace != null)
        {
            HidePersonalSpace();
        }
        else
        {
            ShowPersonalSpace();
        }
    }
}
