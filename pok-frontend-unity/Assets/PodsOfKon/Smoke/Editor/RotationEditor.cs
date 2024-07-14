using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace de.enjoyLife.Smoke
{
    [CustomEditor(typeof(Rotation))]
    public class RotationEditor : Editor
    {
        Rotation script;
        private void OnEnable()
        {
            script = (Rotation)target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(" X:");           
            if (EditorGUILayout.Toggle(script.checkAxis(Rotation.rotationAxis.X)))
            {
                script.addAxis(Rotation.rotationAxis.X);
            }
            else
            {
                script.removeAxis(Rotation.rotationAxis.X);
            }
            GUILayout.Label(" Y:");
            if (EditorGUILayout.Toggle(script.checkAxis(Rotation.rotationAxis.Y)))
            {
                script.addAxis(Rotation.rotationAxis.Y);
            }
            else
            {
                script.removeAxis(Rotation.rotationAxis.Y);
            }
            GUILayout.Label(" Z:");
            if (EditorGUILayout.Toggle(script.checkAxis(Rotation.rotationAxis.Z)))
            {
                script.addAxis(Rotation.rotationAxis.Z);
            }
            else
            {
                script.removeAxis(Rotation.rotationAxis.Z);
            }
            GUILayout.Space(EditorGUILayout.GetControlRect().size.x);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            string[] spaces = { Space.World.ToString(),  Space.Self.ToString() };
            switch (EditorGUILayout.Popup("Space to use: ", (script.UseWorldSpace == Space.World)?0:1, spaces))
            {
                case 0:
                    script.UseWorldSpace = Space.World;
                    break;
                case 1:
                    script.UseWorldSpace = Space.Self;
                    break;
                default:
                    script.UseWorldSpace = Space.World;
                    break;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            script.RotationSpeed = EditorGUILayout.FloatField("Rotation Speed",script.RotationSpeed);
            GUILayout.EndHorizontal();
        }
    }

}
