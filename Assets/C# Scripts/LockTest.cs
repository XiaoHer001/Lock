using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 功能描述：锁定对象测试
/// </summary>
/// <remarks>
/// 注意：测试脚本
/// 依赖：
/// 创建时间：2026年4月23日 20:23:35
/// </remarks>

[ExecuteInEditMode]
public class LockTest : MonoBehaviour
{
    void Update()
    {
        if (GameObject.Find("LockedObject") == null)
        {
            GameObject obj = new GameObject("LockedObject");
            obj.hideFlags = HideFlags.None;
            // obj.hideFlags = HideFlags.NotEditable;
        }
    }
}