using MageBattle.Core.Level;
using UnityEditor;
using UnityEngine;

namespace MageBattle.Editors
{
    [CustomEditor(typeof(LevelBuilder))]
    public class LevelBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Build level"))
            {
                var builder = target as LevelBuilder;
                builder.GenerateLevel();
            }
        }
    }
}