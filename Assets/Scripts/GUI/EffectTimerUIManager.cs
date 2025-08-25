using UnityEngine;
using System.Collections.Generic;

public class EffectTimerUIManager : MonoBehaviour
{
    public EffectTimerUI effectTimerUIPrefab;
    public Transform uiParent; 
    private Dictionary<string, EffectTimerUI> activeEffects = new Dictionary<string, EffectTimerUI>();

    public void ShowEffect(string effectKey, float duration, Sprite icon)
    {
        if (activeEffects.ContainsKey(effectKey))
        {
            activeEffects[effectKey].Show(duration, icon); 
        }
        else
        {
            var ui = Instantiate(effectTimerUIPrefab, uiParent);
            ui.Show(duration, icon);
            activeEffects.Add(effectKey, ui);
            ui.onEffectEnd = () => { activeEffects.Remove(effectKey); Destroy(ui.gameObject); };
        }
    }
}