using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
   
   
}