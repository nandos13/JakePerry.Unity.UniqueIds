using UnityEditor;
using UnityEngine;

namespace JakePerry.Unity
{
    [CustomPropertyDrawer(typeof(UniqueId))]
    public class UniqueIdDrawer : PropertyDrawer
    {
        private static void GetTextAndStyle(UniqueId id, out GUIContent text, out GUIStyle style)
        {
            if (id == null)
            {
                text = GUIContent.none;
                style = EditorStyles.label;
                return;
            }

            style = new GUIStyle(EditorStyles.boldLabel);

            if (id.Valid)
            {
                text = new GUIContent(id.Id);
                style.fontStyle = FontStyle.BoldAndItalic;
            }
            else
            {
                text = new GUIContent("_INVALID_");
            }
        }

        private void DrawContent(Rect rect, SerializedProperty property)
        {
            // Min size of the object reference field
            const float kMinObjRefWidth = 35;

            // Padding between obj reference & text fields
            const float kPadding = 8;

            // Min size required to allow displaying clipped text
            const float kMinTextWidth = 30;

            var textWidth = 0f;
            var objRefWidth = rect.width;
            var padding = 0f;

            var objRef = property.objectReferenceValue as UniqueId;

            GetTextAndStyle(objRef, out GUIContent displayText, out GUIStyle textStyle);

            if (objRef != null)
            {
                var desiredTextWidth = textStyle.CalcSize(displayText).x;

                var minSizeToDisplayText = kMinObjRefWidth + kPadding + Mathf.Min(kMinTextWidth, desiredTextWidth);

                if (rect.width >= minSizeToDisplayText)
                {
                    var availableTextSize = rect.width - kMinObjRefWidth - kPadding;

                    textWidth = desiredTextWidth <= availableTextSize
                        ? desiredTextWidth
                        : availableTextSize;

                    objRefWidth = rect.width - textWidth - kPadding;

                    padding = kPadding;
                }
            }

            var textRect = new Rect(rect.x, rect.y, textWidth, rect.height);
            var objRefRect = new Rect(rect.x + textWidth + padding, rect.y, objRefWidth, rect.height);

            EditorGUI.LabelField(textRect, displayText, textStyle);
            EditorGUI.PropertyField(objRefRect, property, GUIContent.none);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Note: PrefixLabel behaves incorrectly if we don't pass a control id.
            var controlId = GUIUtility.GetControlID(FocusType.Passive, position);
            position = EditorGUI.PrefixLabel(position, controlId, label);

            DrawContent(position, property);
        }
    }
}
