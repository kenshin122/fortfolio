﻿using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(FITextPic))]
public class FITextPicEditor : UnityEditor.UI.TextEditor
{

	private SerializedProperty ImageScalingFactorProp;
	private SerializedProperty hyperlinkColorProp;
	private SerializedProperty imageOffsetProp;
	private SerializedProperty iconList;

	protected override void OnEnable()
	{
		base.OnEnable();
		ImageScalingFactorProp = serializedObject.FindProperty("ImageScalingFactor");
		hyperlinkColorProp = serializedObject.FindProperty("hyperlinkColor");
		imageOffsetProp = serializedObject.FindProperty("imageOffset");
		iconList = serializedObject.FindProperty("spriteContainer");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		serializedObject.Update();
		EditorGUILayout.PropertyField(imageOffsetProp, new GUIContent("Image Offset"));
		EditorGUILayout.PropertyField(ImageScalingFactorProp, new GUIContent("Image Scaling Factor"));
		EditorGUILayout.PropertyField(hyperlinkColorProp, new GUIContent("Hyperlink Color"));
		EditorGUILayout.PropertyField(iconList, new GUIContent("SpriteContainer"), true);
		serializedObject.ApplyModifiedProperties();
	}
}