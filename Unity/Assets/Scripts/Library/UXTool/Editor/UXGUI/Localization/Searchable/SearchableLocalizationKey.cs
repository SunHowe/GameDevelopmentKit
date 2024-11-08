using System;
using ThunderFireUITool;
using ThunderFireUnityEx;
using UnityEditor;
using UnityEngine;

public class SearchableLocalizationKey
{
    private static readonly int s_IdHash = typeof(SearchableLocalizationKey).GetHashCode();

    public static void PropertyField(SerializedProperty property, GUIContent label, Action onKeyChange)
    {
        Rect position = GUILayoutUtility.GetRect(label, EditorStyles.layerMaskField);

        int id = GUIUtility.GetControlID(s_IdHash, FocusType.Keyboard, position);

        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, id, label);

        GUIContent buttonText = new GUIContent(property.stringValue);

        if (DropdownButton(id, position, buttonText))
        {
            void OnSelect(int i)
            {
                property.stringValue = EditorLocalizationTool.AllKeys[i];
                property.serializedObject.ApplyModifiedProperties();
                onKeyChange?.Invoke();
            }
            position.x += 1;
            SearchablePopup.Show(position, EditorLocalizationTool.AllKeys, EditorLocalizationTool.AllKeys.IndexOf(property.stringValue), OnSelect);
        }

        EditorGUI.EndProperty();
    }

    public static void PropertyField(SerializedProperty property, Action onKeyChange)
    {
        Rect position = GUILayoutUtility.GetRect(null, EditorStyles.layerMaskField);

        int id = GUIUtility.GetControlID(s_IdHash, FocusType.Keyboard, position);

        GUIContent buttonText = new GUIContent(property.stringValue);

        if (DropdownButton(id, position, buttonText))
        {
            void OnSelect(int i)
            {
                property.stringValue = EditorLocalizationTool.AllKeys[i];
                property.serializedObject.ApplyModifiedProperties();
                onKeyChange?.Invoke();
            }
            position.x += 1;
            SearchablePopup.Show(position, EditorLocalizationTool.AllKeys, EditorLocalizationTool.AllKeys.IndexOf(property.stringValue), OnSelect);
        }
    }

    /// <summary>
    /// A custom button drawer that allows for a controlID so that we can
    /// sync the button ID and the label ID to allow for keyboard
    /// navigation like the built-in enum drawers.
    /// </summary>
    private static bool DropdownButton(int id, Rect position, GUIContent content)
    {
        Event current = Event.current;
        switch (current.type)
        {
            case EventType.MouseDown:
                if (position.Contains(current.mousePosition) && current.button == 0)
                {
                    Event.current.Use();
                    return true;
                }

                break;
            case EventType.KeyDown:
                if (GUIUtility.keyboardControl == id && current.character == '\n')
                {
                    Event.current.Use();
                    return true;
                }

                break;
            case EventType.Repaint:
                EditorStyles.popup.Draw(position, content, id, false);
                break;
        }

        return false;
    }
}