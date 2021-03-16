using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MageBattle.Editors
{
    public class ScenesWindow : EditorWindow
    {
        private Vector2 _scrollPosition;

        [MenuItem("Editors/Scenes")]
        public static void ShowWindow()
        {
            GetWindow<ScenesWindow>("Scenes");
        }

        private void OnGUI()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true));

            foreach (var editorBuildSettingsScene in EditorBuildSettings.scenes)
            {
                var splittedPath = editorBuildSettingsScene.path.Split('/');
                var sceneName = splittedPath[splittedPath.Length - 1];
                sceneName = sceneName.Remove(sceneName.Length - 6);

                if (GUILayout.Button(sceneName))
                    EditorSceneManager.OpenScene(editorBuildSettingsScene.path);
            }

            GUILayout.EndScrollView();
        }
    }
}