using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BullBrukBruker
{
#if UNITY_EDITOR
    [CustomEditor(typeof(LevelMaker))]
    public class LevelMakerEditor : Editor
    {
        private SerializedProperty currentLevel;
        private SerializedProperty blockMaptile;
        private SerializedProperty itemMapTile;
        private SerializedProperty startPoint;
        private SerializedProperty endPoint;
        private SerializedProperty minStepsPerItemSpawn;
        private SerializedProperty maxStepsPerItemSpawn;
        private SerializedProperty items;

        const float markerWidth = 5;
        const float markerHeight = 7.5f;
        int selectedItemKeyIndex = -1;

        private void OnEnable()
        {
            currentLevel = serializedObject.FindProperty("currentLevel");
            blockMaptile = serializedObject.FindProperty("blockMaptile");
            itemMapTile = serializedObject.FindProperty("itemMapTile");

            startPoint = serializedObject.FindProperty("startPoint");
            endPoint = serializedObject.FindProperty("endPoint");
            minStepsPerItemSpawn = serializedObject.FindProperty("minStepsPerItemSpawn");
            maxStepsPerItemSpawn = serializedObject.FindProperty("maxStepsPerItemSpawn");
            items = serializedObject.FindProperty("items");

            selectedItemKeyIndex = -1;
        }

        public override void OnInspectorGUI()
        {
            GUIStyle style = new()
            {
                padding = new RectOffset(10, 10, 10, 10),
            };

            GUILayout.Label("====== Level Maker ======");

            EditorGUILayout.BeginVertical(style);

            EditorGUILayout.PropertyField(currentLevel);
            EditorGUILayout.PropertyField(blockMaptile);
            EditorGUILayout.PropertyField(itemMapTile);

            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Level"))
                (target as LevelMaker).ClearLevel();
            if (GUILayout.Button("Save Level"))
                (target as LevelMaker).SaveLevel();
            if (GUILayout.Button("Load Level"))
                (target as LevelMaker).LoadLevel();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            GUILayout.Label("=== Random Items ===");

            EditorGUILayout.BeginVertical(style);

            GUILayout.Label("- Spawn Area -");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);
            GUILayout.Label("Start"); EditorGUILayout.PropertyField(startPoint, GUIContent.none);
            EditorGUILayout.Space(10);
            GUILayout.Label("End"); EditorGUILayout.PropertyField(endPoint, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Label("--- Random Steps Per Spawn ---");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);
            GUILayout.Label("Min"); EditorGUILayout.PropertyField(minStepsPerItemSpawn, GUIContent.none);
            EditorGUILayout.Space(10);
            GUILayout.Label("Max"); EditorGUILayout.PropertyField(maxStepsPerItemSpawn, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Label("--- Items Spawned ---");
            EditorGUILayout.Space(10);
            var itemKeyTrackingRect = DrawItemKeysEditor();
            HandleDragItemKey(itemKeyTrackingRect);
            HandleAddNewItemKey(itemKeyTrackingRect);
            EditorGUILayout.Space(10);

            GUILayout.Label("Current Item Key");
            if (selectedItemKeyIndex != -1)
                EditorGUILayout.PropertyField(items.GetArrayElementAtIndex(selectedItemKeyIndex), GUIContent.none, true);
            else
                GUILayout.Label("(Not Any Current Item Key Selected!)");

            GUILayout.Space(5);
            if (GUILayout.Button("Generate Random Items"))
                (target as LevelMaker).GenerateRandomItems();

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private Rect DrawItemKeysEditor()
        {
            // Draw tracking area
            Rect itemKeyTrackingRect = GUILayoutUtility.GetRect(200, 5);
            EditorGUI.DrawRect(itemKeyTrackingRect, Color.cyan);

            DrawItemKeyMarkers(itemKeyTrackingRect);

            return itemKeyTrackingRect;
        }

        private void DrawItemKeyMarkers(Rect trackRect)
        {
            for (int i = 0; i < items.arraySize; i++)
            {
                var keyProp = items.GetArrayElementAtIndex(i);
                float position = keyProp.FindPropertyRelative("Value").floatValue;

                // Draw marker
                Rect markerRect = new(
                    trackRect.x + position * trackRect.width - markerWidth * 0.5f,
                    trackRect.yMax,
                    markerWidth,
                    markerHeight
                );
                EditorGUI.DrawRect(markerRect, selectedItemKeyIndex == i ? Color.red : Color.gray);

                if (TryHandleItemKeyMarkerClicked(markerRect, i))
                    break;
            }
        }

        private bool TryHandleItemKeyMarkerClicked(Rect markerRect, int markerIndex)
        {
            if (markerRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    // Select marker
                    selectedItemKeyIndex = markerIndex;
                    Event.current.Use();
                    return false;
                }
                else if (Event.current.button == 1)
                {
                    // Delete marker
                    items.DeleteArrayElementAtIndex(markerIndex);
                    if (markerIndex == selectedItemKeyIndex)
                        selectedItemKeyIndex = -1;

                    Event.current.Use();
                    return true;
                }
            }

            return false;
        }

        private void HandleDragItemKey(Rect trackingRect)
        {
            if (selectedItemKeyIndex == -1 || Event.current.type != EventType.MouseDrag)
                return;

            float newPosition = Mathf.Clamp01((Event.current.mousePosition.x - trackingRect.x) / trackingRect.width);
            items.GetArrayElementAtIndex(selectedItemKeyIndex)
                .FindPropertyRelative("Value").floatValue = newPosition;
            Event.current.Use();
        }

        private void HandleAddNewItemKey(Rect trackRect)
        {
            if (!trackRect.Contains(Event.current.mousePosition) ||
                Event.current.type != EventType.MouseDown ||
                Event.current.button != 0)
                return;

            // Add new marker
            items.arraySize++;
            var newKey = items.GetArrayElementAtIndex(items.arraySize - 1);
            newKey.FindPropertyRelative("Value").floatValue =
                Mathf.Clamp01((Event.current.mousePosition.x - trackRect.x) / trackRect.width);
            selectedItemKeyIndex = items.arraySize - 1;
            Event.current.Use();
        }
    }
#endif
}