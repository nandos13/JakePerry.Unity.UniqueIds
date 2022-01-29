using System.Collections.Generic;
using UnityEditor;

namespace JakePerry.Unity
{
    [CustomEditor(typeof(UniqueId))]
    public class UniqueIdInspector : Editor
    {
        private static List<UniqueId> GetAllFromAssetDatabase()
        {
            var list = new List<UniqueId>();

            foreach (var assetGuid in AssetDatabase.FindAssets("t:UniqueId"))
            {
                var path = AssetDatabase.GUIDToAssetPath(assetGuid);
                var asset = AssetDatabase.LoadAssetAtPath<UniqueId>(path);

                if (asset != null)
                    list.Add(asset);
            }

            return list;
        }

        private static bool CheckUniqueness(string id, out List<UniqueId> collisions)
        {
            collisions = null;

            var comparer = System.StringComparer.OrdinalIgnoreCase;

            UniqueId first = null;

            foreach (var asset in GetAllFromAssetDatabase())
            {
                if (comparer.Equals(asset.Id, id))
                {
                    if (first == null)
                    {
                        first = asset;
                    }
                    else
                    {
                        if (collisions is null)
                        {
                            collisions = new List<UniqueId>();
                            collisions.Add(first);
                        }

                        collisions.Add(asset);
                    }
                }
            }

            return collisions == null;
        }

        private void DrawMessageBoxRegion(string id)
        {
            EditorGUILayout.Space();

            if (string.IsNullOrEmpty(id))
            {
                EditorGUILayout.HelpBox("Id is not set.", MessageType.Warning);
            }
            else if (!CheckUniqueness(id, out List<UniqueId> collisions))
            {
                EditorGUILayout.HelpBox(
                    $"Id is shared between {collisions.Count.ToString()} objects (see list below).",
                    MessageType.Warning);

                using (EditorGuiHelper.DisabledBlock)
                {
                    var list = new UnityEditorInternal.ReorderableList(collisions, typeof(UniqueId), false, false, false, false);

                    list.drawElementCallback += (r, index, isActive, isFocused) =>
                    {
                        var obj = collisions[index];
                        EditorGUI.ObjectField(r, obj, typeof(UniqueId), false);
                    };

                    list.DoLayoutList();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Id is unique.", MessageType.Info);
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGuiHelper.DrawMonoScriptField(target);

            var idProperty = serializedObject.FindProperty("m_id");

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(idProperty);

            var id = idProperty.stringValue ?? string.Empty;

            // Clean up user input
            if (EditorGUI.EndChangeCheck())
            {
                var trimmed = id.Trim();

                if (id.Length != trimmed.Length)
                {
                    idProperty.stringValue = trimmed;
                    id = trimmed;
                }
            }

            serializedObject.ApplyModifiedProperties();

            DrawMessageBoxRegion(id);
        }
    }
}
