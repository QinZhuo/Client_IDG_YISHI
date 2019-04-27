using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
namespace IDG
{

    [CustomPropertyDrawer(typeof(Fixed))]
    public class FixedView : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var f =new Fixed().SetValue(property.FindPropertyRelative("m_Bits").longValue).ToFloat();
            f= EditorGUI.FloatField(position,label.text+"(fixed)",f);
            property.FindPropertyRelative("m_Bits").longValue = new Fixed(f).m_Bits;
        }
    }
    //[CustomPropertyDrawer(typeof(FixedDictionary))]
    //public class FixedDictionaryView : PropertyDrawer
    //{
    //    ReorderableList list;
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        Dictionary<string, Fixed> dic = property.FindPropertyRelative("dic") ;
    //        list = new ReorderableList(property.,);
    //        var f = new Fixed().SetValue(property.FindPropertyRelative("m_Bits").longValue).ToFloat();
    //        f = EditorGUI.FloatField(position, label.text + "(fixed)", f);
    //        property.FindPropertyRelative("m_Bits").longValue = new Fixed(f).m_Bits;
    //    }
    //}

}