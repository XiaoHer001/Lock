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
            obj.hideFlags = HideFlags.NotEditable;
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
    [MenuItem("Tools/Lock Selected %#l")]
    static void LockSelected()
    {
        foreach (var obj in Selection.gameObjects)
            Apply(obj, true);
    }

    [MenuItem("Tools/Unlock Selected %#u")]
    static void UnlockSelected()
    {
        foreach (var obj in Selection.gameObjects)
            Apply(obj, false);
    }

    static void Apply(GameObject root, bool lockState)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            var obj = t.gameObject;
            obj.hideFlags = lockState ? HideFlags.NotEditable : HideFlags.None;
            EditorUtility.SetDirty(obj);
        }
    }
}
```

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
