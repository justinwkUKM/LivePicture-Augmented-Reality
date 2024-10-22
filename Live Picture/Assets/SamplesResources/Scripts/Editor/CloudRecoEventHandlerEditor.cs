/*==============================================================================
Copyright (c) 2015-2017 PTC Inc. All Rights Reserved.

Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.  
==============================================================================*/
using UnityEditor;
using UnityEngine;
using Vuforia;

/// <summary>
/// This editor class renders the custom inspector for the CloudRecoEventHandler MonoBehaviour
/// </summary>
[CustomEditor(typeof(CloudRecoEventHandler))]
public class CloudRecoEventHandlerEditor : Editor
{
    #region UNITY_EDITOR_METHODS

    /// <summary>
    /// Draws a custom UI for the cloud reco event handler inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        // record potential changes for this object
        Undo.RecordObject(target, "Inspector Change");

        CloudRecoEventHandler m_CloudRecoEventHandler = (CloudRecoEventHandler)target;

        EditorGUILayout.HelpBox(
            "Here you can set the ImageTargetBehaviour from the scene " +
            "that will be used to augment new cloud reco search results.",
            MessageType.Info
        );

        bool allowSceneObjects = !EditorUtility.IsPersistent(target);

        m_CloudRecoEventHandler.ImageTargetTemplate = (ImageTargetBehaviour)EditorGUILayout.ObjectField(
            "Image Target Template",
            m_CloudRecoEventHandler.ImageTargetTemplate, typeof(ImageTargetBehaviour), allowSceneObjects);

        m_CloudRecoEventHandler.scanLine = (ScanLine)EditorGUILayout.ObjectField(
            "Scan Line",
            m_CloudRecoEventHandler.scanLine, typeof(ScanLine), true);

        m_CloudRecoEventHandler.cloudErrorCanvas = (UnityEngine.Canvas)EditorGUILayout.ObjectField(
            "Cloud Reco Error Canvas",
            m_CloudRecoEventHandler.cloudErrorCanvas, typeof(UnityEngine.Canvas), true);

        m_CloudRecoEventHandler.cloudErrorTitle = (UnityEngine.UI.Text)EditorGUILayout.ObjectField(
            "Cloud Reco Error Title",
            m_CloudRecoEventHandler.cloudErrorTitle, typeof(UnityEngine.UI.Text), true);

        m_CloudRecoEventHandler.cloudErrorText = (UnityEngine.UI.Text)EditorGUILayout.ObjectField(
            "Cloud Reco Error Text",
            m_CloudRecoEventHandler.cloudErrorText, typeof(UnityEngine.UI.Text), true);

        m_CloudRecoEventHandler.cloudActivityIcon = (UnityEngine.UI.Image)EditorGUILayout.ObjectField(
            "Cloud Activity Icon",
            m_CloudRecoEventHandler.cloudActivityIcon, typeof(UnityEngine.UI.Image), true);

        if (GUI.changed)
            EditorUtility.SetDirty(m_CloudRecoEventHandler);
    }

    #endregion // UNITY_EDITOR_METHODS
}
