
//近づいたときにネームタグを表示するスクリプト（XR対応版）
using UnityEngine;
using UnityEngine. XR;
using UnityEngine.XR.Interaction. Toolkit;
using TMPro;

public class ProximityNameDisplayXR : MonoBehaviour
{
    [Header("TextMeshPro設定")]
    [SerializeField] private TextMeshPro nameText3D;
    [SerializeField] private TextMeshProUGUI nameTextUI; // Canvas使用時
    
    [Header("距離設定")]
    [SerializeField] private float displayDistance = 4f;
    
    [Header("フェード設定")]
    [SerializeField] private bool useFade = true;
    [SerializeField] private float fadeSpeed = 5f;
    
    [Header("ビルボード設定")]
    [SerializeField] private bool useBillboard = true;
    
    private Transform xrOriginTransform;
    private Camera xrCamera;
    private float currentAlpha = 0f;
    
    void Start()
    {
        // XR Origin（プレイヤー）を検索
        FindXROrigin();
        
        // 初期状態では非表示
        SetTextAlpha(0f);
    }
    
    private void FindXROrigin()
    {
        // 方法1: XR Originを直接検索
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (xrOrigin != null)
        {
            xrOriginTransform = xrOrigin.Camera.transform;
            xrCamera = xrOrigin.Camera;
            return;
        }
        
//         // 方法2: 旧式のXR Rigを検索（古いバージョン用）
// #pragma warning disable CS0618
//         var xrRig = FindObjectOfType<XRRig>();
//         if (xrRig != null)
//         {
//             xrOriginTransform = xrRig. cameraGameObject.transform;
//             xrCamera = xrRig. cameraGameObject. GetComponent<Camera>();
//             return;
//         }
#pragma warning restore CS0618
        
        // 方法3: フォールバックとしてMain Cameraを使用
        if (Camera.main != null)
        {
            xrOriginTransform = Camera.main.transform;
            xrCamera = Camera.main;
            Debug.LogWarning("XR Origin not found. Using Main Camera as fallback.");
        }
    }
    
    void Update()
    {
        if (xrOriginTransform == null) return;
        
        UpdateNameVisibility();
        
        if (useBillboard)
        {
            UpdateBillboard();
        }
    }
    
    private void UpdateNameVisibility()
    {
        // HMD（ヘッドセット）の位置とHumanoidとの距離を計算
        float distance = Vector3. Distance(transform.position, xrOriginTransform. position);
        
        // 目標のアルファ値を決定
        float targetAlpha = distance <= displayDistance ? 1f : 0f;
        
        if (useFade)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }
        else
        {
            currentAlpha = targetAlpha;
        }
        
        SetTextAlpha(currentAlpha);
    }
    
    private void UpdateBillboard()
    {
        if (xrCamera == null) return;
        
        // テキストを常にHMDの方向に向ける
        Transform textTransform = nameText3D != null ? nameText3D.transform : 
                                  nameTextUI != null ? nameTextUI. transform : null;
        
        if (textTransform != null)
        {
            textTransform.LookAt(
                textTransform.position + xrCamera.transform. rotation * Vector3.forward,
                xrCamera.transform. rotation * Vector3. up
            );
        }
    }
    
    private void SetTextAlpha(float alpha)
    {
        if (nameText3D != null)
        {
            Color color = nameText3D.color;
            color.a = alpha;
            nameText3D.color = color;
        }
        
        if (nameTextUI != null)
        {
            Color color = nameTextUI. color;
            color.a = alpha;
            nameTextUI.color = color;
        }
    }
    
    // 公開メソッド
    public void SetDisplayDistance(float distance)
    {
        displayDistance = distance;
    }
    
    public void SetPlayerTransform(Transform player)
    {
        xrOriginTransform = player;
    }
}