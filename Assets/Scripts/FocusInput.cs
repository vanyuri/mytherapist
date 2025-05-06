using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class iOSKeyboardFix : MonoBehaviour, IPointerClickHandler
{
    public TMP_InputField inputField;

    public void OnPointerClick(PointerEventData eventData)
    {
#if UNITY_IOS && !UNITY_EDITOR
        inputField.ActivateInputField();
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
#endif
    }
}
