using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 功能描述：物体对象锁定工具
/// </summary>
/// <remarks>
/// 注意：该脚本放在 Editor 文件夹中
/// 依赖：
/// 创建时间：2026年4月23日 20:26:03
/// </remarks>

public class EditableControlMenu
{
    // =========================
    // 锁定选中对象（包含子物体）
    // =========================
    [MenuItem("Tools/Lock Selected %#l")] // Ctrl+Shift+L
    static void LockSelected()
    {
        var selection = Selection.gameObjects;

        if (selection.Length == 0)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }

        int count = 0;

        foreach (var obj in selection)
        {
            // 避免锁 Prefab Root（重要）
            if (PrefabUtility.IsAnyPrefabInstanceRoot(obj))
            {
                Debug.LogWarning($"Skip Prefab Root: {obj.name}");
                continue;
            }

            count += ApplyToHierarchy(obj, true);
        }

        Debug.Log($"Locked {count} objects.");
    }

    // =========================
    // 解锁选中对象（包含子物体）
    // =========================
    [MenuItem("Tools/Unlock Selected %#u")] // Ctrl+Shift+U
    static void UnlockSelected()
    {
        var selection = Selection.gameObjects;

        if (selection.Length == 0)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }

        int count = 0;

        foreach (var obj in selection)
        {
            count += ApplyToHierarchy(obj, false);
        }

        Debug.Log($"Unlocked {count} objects.");
    }

    // =========================
    // 一键解锁所有对象（救命按钮）
    // =========================
    [MenuItem("Tools/Force Unlock ALL")]
    static void UnlockAll()
    {
        var all = Object.FindObjectsOfType<GameObject>(true);
        int count = 0;

        foreach (var obj in all)
        {
            if (obj.hideFlags != HideFlags.None)
            {
                Undo.RecordObject(obj, "Force Unlock");
                obj.hideFlags = HideFlags.None;
                EditorUtility.SetDirty(obj);
                count++;
            }
        }

        Debug.Log($"Force Unlocked {count} objects (ALL).");
    }

    // =========================
    // 核心逻辑：递归处理层级
    // =========================
    static int ApplyToHierarchy(GameObject root, bool lockState)
    {
        int count = 0;

        var transforms = root.GetComponentsInChildren<Transform>(true);

        foreach (var t in transforms)
        {
            var obj = t.gameObject;

            // 已经是目标状态就跳过
            if (lockState && obj.hideFlags == HideFlags.NotEditable)
                continue;

            if (!lockState && obj.hideFlags == HideFlags.None)
                continue;

            Undo.RecordObject(obj, lockState ? "Lock Object" : "Unlock Object");

            obj.hideFlags = lockState ? HideFlags.NotEditable : HideFlags.None;

            EditorUtility.SetDirty(obj);

            count++;
        }

        return count;
    }
}