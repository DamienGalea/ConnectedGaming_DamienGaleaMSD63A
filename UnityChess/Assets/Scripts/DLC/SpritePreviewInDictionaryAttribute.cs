
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

public class SpritePreviewInDictionaryAttribute : Attribute { }

#if UNITY_EDITOR
[DrawerPriority(0, 0, 0)] // Makes sure this is run first
public class SpritePreviewInDictionaryDrawer : OdinAttributeDrawer<SpritePreviewInDictionaryAttribute, Dictionary<string, Sprite>>
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        var dictionary = this.ValueEntry.SmartValue;
        if (dictionary == null)
        {
            this.ValueEntry.SmartValue = new Dictionary<string, Sprite>();
            dictionary = this.ValueEntry.SmartValue;
        }

        foreach (var key in new List<string>(dictionary.Keys))
        {
            EditorGUILayout.BeginHorizontal();

            dictionary[key] = (Sprite)EditorGUILayout.ObjectField(dictionary[key], typeof(Sprite), false,
                GUILayout.Width(70), GUILayout.Height(70));

            EditorGUILayout.LabelField(key, GUILayout.Width(150));

            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif