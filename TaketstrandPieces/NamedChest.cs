using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace TaketstrandPieces;

public class NamedChest : MonoBehaviour
{
    public Sign sign;
    public TextMeshProUGUI textFront;
    public Image iconLeft;
    public Image iconRight;

    private string _lastText;

    private void Update()
    {
        if (!sign || !sign.m_textWidget)
            return;

        var text = sign.GetText();
        if (text == _lastText)
            return;

        _lastText = text;

        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        UpdateDisplay(text);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void UpdateDisplay(string raw)
    {
        if (raw.StartsWith("#"))
        {
            var mode = "all";
            if (raw.EndsWith("#_mode_all"))
            {
                raw = raw.Replace("#_mode_all", "");
            }
            else if (raw.EndsWith("#_mode_text"))
            {
                mode = "text";
                raw = raw.Replace("#_mode_text", "");
            }
            else if (raw.EndsWith("#_mode_icon"))
            {
                mode = "icon";
                raw = raw.Replace("#_mode_icon", "");
            }

            var itemName = raw.Substring(1);
            var prefab = ObjectDB.instance?.GetItemPrefab(itemName);
            if (prefab != null)
            {
                var drop = prefab.GetComponent<ItemDrop>();
                if (drop != null)
                {
                    textFront.text = null;
                    iconLeft.sprite = null;
                    iconRight.sprite = null;
                    iconLeft.gameObject.SetActive(false);
                    iconRight.gameObject.SetActive(false);
                    if (mode is "all" or "text")
                    {
                        textFront.text = Localization.instance.Localize(drop.m_itemData.m_shared.m_name);
                    }
                    if (mode is "all" or "icon")
                    {
                        iconLeft.sprite = drop.m_itemData.GetIcon();
                        iconRight.sprite = drop.m_itemData.GetIcon();
                        iconLeft.gameObject.SetActive(true);
                        iconRight.gameObject.SetActive(true);
                    }

                    return;
                }
            }
        }

        textFront.text = raw;

        iconLeft.gameObject.SetActive(false);
        iconRight.gameObject.SetActive(false);
    }
}