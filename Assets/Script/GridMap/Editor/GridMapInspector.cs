using UnityEngine;
using UnityEditor;
using System.Collections;
using Rotorz.ReorderableList;

[CustomEditor (typeof(LevelGenerator))]
public class GridMapInspector : Editor
{

	private LevelGenerator _mapGenerator;

	void OnEnable ()
	{
		_mapGenerator = (LevelGenerator)target;
	}

	public override void OnInspectorGUI ()
	{
		GUILayout.Space (10f);
		serializedObject.Update ();
		GUILayout.Label ("GRID MAP CONFIGURATION");
		GUILayout.Space (5f);

		DrawProperty ("_gridPrefab", 5f);
		DrawProperty ("_cameraOrthographicSize", 5f);
		DrawProperty ("_gridSize", 5f);
		DrawProperty ("_sizeRow", 5f);
		DrawProperty ("_sizeCol", 5f);
		DrawProperty ("_startingPosition", 10f);
		SerializedProperty createBoundaries = DrawProperty ("_createBoundaries", 5f);

		if (createBoundaries.boolValue) {
			DrawProperty ("_boundLayerName");
			DrawProperty ("_isTrigger");
			DrawProperty ("_distanceToMapBorder");
		}

		DrawReorderableList ("_enemyPathManager", "Enemy Path List", 5f);
		DrawReorderableList ("_bossPathManager", "Boss Path List", 5f);
		DrawArrayProperty ("_enemyInfo", 5f);

		GUILayout.Space (10f);
		if (GUILayout.Button ("Generate Map")) {
			_mapGenerator.CreateGrid ();
		}

		GUILayout.Space (5f);
		if (GUILayout.Button ("Delete Map")) {
			_mapGenerator.DeleteMap ();
		}


		// Export Menu
		GUILayout.Space (10f);
		GUILayout.Label ("LEVEL CONFIGURATION");
		GUILayout.Space (5f);
		SerializedProperty directoryPath = serializedObject.FindProperty ("_directoryPath");
		if (string.IsNullOrEmpty (directoryPath.stringValue)) {
			EditorGUILayout.HelpBox ("Please select a directory where the level will be saved.", MessageType.Error, true);
		} else {
			if (!System.IO.Directory.Exists (directoryPath.stringValue)) {
				directoryPath.stringValue = string.Empty;
			}
		}

		if (GUILayout.Button (!string.IsNullOrEmpty (directoryPath.stringValue) ? (directoryPath.stringValue.Contains ("Assets") ? directoryPath.stringValue.Substring (directoryPath.stringValue.IndexOf ("Assets")) : directoryPath.stringValue) : "Select directory", "PreDropDown")) {
			string path = EditorUtility.OpenFolderPanel ("Select directory, where the level will be saved.", "", "");
			if (!string.IsNullOrEmpty (path)) {
				directoryPath.stringValue = path;
			}
		}

		DrawProperty ("_fileName", 5f);
		DrawProperty ("_levelLocation", 5f);
		SerializedProperty gameMode = DrawProperty ("_gameMode", 5f);
		if (gameMode.enumValueIndex == EnumerationUtils.GetEnumIndexByName<GameType> ("TIME_ATTACK")) {
			DrawProperty ("_timeLimit", 5f);
		}

		DrawProperty ("_difficulty", 5f);

		DrawProperty ("_starLevel0", 5f);
		DrawProperty ("_starLevel1", 5f);
		DrawProperty ("_starLevel2", 5f);
		DrawProperty ("_starLevel3", 5f);


		DrawProperty ("_playerInitPos", 5f);
		DrawProperty ("_startingCubeNumber", 5f);

		GUILayout.Space (5f);
		if (GUILayout.Button ("Export Map")) {
			_mapGenerator.ExportMap ();
		}

		serializedObject.ApplyModifiedProperties ();
		bool guiEnabled = GUI.enabled;

		GUI.enabled = guiEnabled;

		if (GUI.changed)
			EditorUtility.SetDirty (_mapGenerator);

	}

	private SerializedProperty DrawReorderableList (string propertyName, string listTitle, float space)
	{
		GUILayout.Space (space);
		SerializedProperty propertyList = serializedObject.FindProperty (propertyName);
		ReorderableListGUI.Title (listTitle);
		ReorderableListGUI.ListField (propertyList);

		return propertyList;
	}

	private SerializedProperty DrawArrayProperty (string propertyName, float space)
	{
		GUILayout.Space (space);
		return DrawProperty (propertyName, true, true);
	}

	private SerializedProperty DrawProperty (string propertyName, float space, bool isArray = false)
	{
		GUILayout.Space (space);
		return DrawProperty (propertyName, false, isArray);
	}

	private SerializedProperty DrawProperty (string propertyName, bool isArray = false)
	{
		return DrawProperty (propertyName, false, isArray);
	}

	private SerializedProperty DrawProperty (string propertyName, bool insert, bool isArray = false)
	{
		SerializedProperty property = serializedObject.FindProperty (propertyName);
		if (insert) {
			EditorGUI.indentLevel += 1;
		}
		EditorGUILayout.PropertyField (property, isArray);

		if (insert) {
			EditorGUI.indentLevel -= 1;
		}
		return property;
	}
}
