using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class TEST_script : MonoBehaviour
{
    int a; // aというint型の変数を宣言

    void Start()
    {
        a = 10; // 変数aの中身を10に設定
        Debug.Log("Start: " + a); // コンソールウインドウに変数aの持っている数値を表示
    }

    void Update()
    {
        // 入力されているすべてのデバイスをチェック
        foreach (var device in InputSystem.devices)
        {
            // 各デバイスにbuttonSouth（Aボタン相当）があるか探す
            if (device.TryGetChildControl<ButtonControl>("primaryButton") is ButtonControl aButton)
            {
                if (aButton.wasPressedThisFrame)
                {
                    Debug.Log("Aボタン（buttonSouth）が押されました [デバイス名: " + device.name + "]");
                }
            }
        }
    }
}
