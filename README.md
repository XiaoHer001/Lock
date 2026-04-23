# Unity 编辑器对象锁定工具（HideFlags Tool）

## 📌 简介

在 Unity 项目中，有很多对象是运行时生成、系统维护或依赖复杂逻辑的对象。

如果在编辑器中误修改这些对象，可能会导致：

- 数据错乱
- 场景异常
- 运行逻辑失效

因此，本工具通过 `HideFlags` 提供一种轻量级的“编辑器对象保护机制”。

---

## 🔒 HideFlags 作用说明

`HideFlags` 是 Unity 提供的对象标记系统，本质是一个位掩码（bitmask），用于控制 GameObject 在编辑器中的行为。

### 常用标记

- `HideFlags.NotEditable`：对象不可编辑  
- `HideFlags.HideInHierarchy`：在层级面板中隐藏  
- `HideFlags.DontSave`：不会被保存到场景  
- `HideFlags.HideAndDontSave`：完全隐藏 + 不保存  

### 组合效果

可以实现：

- 👁️ 不可见对象  
- 🔒 可见但不可编辑对象  
- 🧹 运行后自动销毁对象  
- 🧠 编辑器保护对象  

---

## 🧪 示例：运行时自动保护对象

```csharp
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

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
```

📌 说明：

- 在编辑模式（未进入 Play）也会执行  
- 对象被删除后会自动重建  
- 可用于“编辑器守护对象”  

---

## 🧰 Unity 编辑器工具（菜单扩展）

将脚本放入 `Assets/Editor/` 文件夹：

```csharp
using UnityEditor;
using UnityEngine;

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
```

---

## ▶️ 工具运行

<img width="2560" height="1380" alt="image" src="https://github.com/user-attachments/assets/be6674c7-01a7-4a99-9c82-ef2932b2bc7e" />

---

## 🎮 快捷键

- Ctrl + Shift + L → 锁定对象  
- Ctrl + Shift + U → 解锁对象  

---

## ⚠️ 注意事项

- 仅在 Unity Editor 中有效  
- 不影响打包运行  
- 不建议锁定 Prefab Root  
- Force Unlock 会影响所有隐藏对象，请谨慎使用  

---

## 🚀 总结

该工具用于：

- 保护运行时对象  
- 防止误编辑  
- 提供批量锁定 / 解锁能力  
- 提供编辑器级对象管理能力  
