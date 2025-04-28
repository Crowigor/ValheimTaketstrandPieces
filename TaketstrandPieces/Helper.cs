

using Jotunn.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaketstrandPieces;
public static class Helper
{
    public static GameObject CreateSignCanvas(string name, Transform parent, Vector3 pos, Quaternion rot, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent);
        go.transform.localPosition = pos;
        go.transform.localRotation = rot;
        go.transform.localScale = Vector3.one;

        var rectTransform = go.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.scaleFactor = 10f;

        go.AddComponent<CanvasRenderer>();
        go.AddComponent<GraphicRaycaster>();

        // var bg = go.AddComponent<Image>();
        // bg.color = new Color(1f, 0f, 0f, 0.3f);

        return go;
    }

    public static TextMeshProUGUI CreateText(Transform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var text = go.AddComponent<TextMeshProUGUI>();
        text.text = "";
        text.fontSize = 1f;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.color = new Color32(20, 20, 20, 255);

        var signPrefab = PrefabManager.Instance.GetPrefab("sign");
        if (signPrefab != null)
        {
            var signText = signPrefab.GetComponentInChildren<TextMeshProUGUI>(true);
            if (signText != null)
            {
                text.font = signText.font;
                text.fontMaterial = new Material(signText.fontMaterial);
                text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.3f);
            }
        }

        text.outlineColor = new Color32(255, 255, 255, 255);
        text.outlineWidth = 0.02f;

        return text;
    }

    public static Image CreateIcon(Transform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var image = go.AddComponent<Image>();
        image.preserveAspect = true;
        image.enabled = true;
        image.rectTransform.localEulerAngles = new Vector3(0, 0, 0);

        return image;
    }
    
    public static void CreateSnapPoint(Transform parent, Vector3 localPosition, string objectName)
    {
        var go = new GameObject(objectName);
        go.transform.SetParent(parent);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        go.tag = "snappoint";
        go.SetActive(false);
    }
}