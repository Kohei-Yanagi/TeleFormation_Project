using UnityEngine;
using TMPro;

public class NameLabelActivator : MonoBehaviour
{
    public Transform player;            // プレイヤー（カメラ）
    public Transform target;            // 名前を表示する対象
    public TextMeshPro textMesh;        // 表示する TextMeshPro
    public float triggerDistance = 2.0f; // プレイヤーが近づく距離
    public float heightOffset = 0.2f;   // 頭上に少し余白

    void Start()
    {
        if (player == null)
            player = Camera.main.transform;

        if (target == null)
            target = transform.parent;

        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        textMesh.text = target.name;
        textMesh.gameObject.SetActive(false); // 初期は非表示
    }

    void LateUpdate()
    {
        float distance = Vector3.Distance(player.position, target.position);

        // ① 距離判定
        if (distance <= triggerDistance)
            textMesh.gameObject.SetActive(true);
        else
            textMesh.gameObject.SetActive(false);

        // ② 頭上に配置
        float height = 1f;
        var col = target.GetComponent<Collider>();
        if (col != null)
            height = col.bounds.size.y;

        transform.position = target.position + Vector3.up * (height + heightOffset);

        // ③ カメラ方向へ向ける
        transform.LookAt(player);
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
}
