using System;
using ET;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityToolbarExtender;

namespace Game.Editor
{
    [InitializeOnLoad]
    internal sealed class CompileDllToolBar_ET
    {
        private const CodeOptimization s_CodeOptimization = CodeOptimization.Debug;
        
        private static readonly GUIContent s_BuildHotfixButtonGUIContent;
        private static readonly GUIContent s_BuildHotfixModelButtonGUIContent;
        private static bool s_IsReloading;

        static CompileDllToolBar_ET()
        {
            s_BuildHotfixButtonGUIContent = new GUIContent($"Reload ET.Hotfix", $"Compile And Reload ET.Hotfix Dll.");
            s_BuildHotfixModelButtonGUIContent = new GUIContent($"Compile All ET", $"Compile All ET Dll.");
            s_IsReloading = false;
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            if (Application.isPlaying && GUILayout.Button(s_BuildHotfixButtonGUIContent))
            {
                GlobalConfig globalConfig = Resources.Load<GlobalConfig>("ET/GlobalConfig");
                
                BuildAssemblyHelper.BuildModel(s_CodeOptimization, globalConfig.CodeMode);
                BuildAssemblyHelper.BuildHotfix(s_CodeOptimization, globalConfig.CodeMode);
                
                EditorWindow game = EditorWindow.GetWindow(typeof (EditorWindow).Assembly.GetType("UnityEditor.GameView"));
                if (game != null) game.ShowNotification(new GUIContent("Build Model And Hotfix Success!"));
                
                if (s_IsReloading)
                    return;
                s_IsReloading = true;

                async void ReloadAsync()
                {
                    try
                    {
                        await CodeLoader.Instance.LoadHotfixAsync();
                        EventSystem.Instance.Load();
                    }
                    finally
                    {
                        s_IsReloading = false;
                    }
                }

                ReloadAsync();
            }

            if (GUILayout.Button(s_BuildHotfixModelButtonGUIContent))
            {
                GlobalConfig globalConfig = Resources.Load<GlobalConfig>("ET/GlobalConfig");
                
                BuildAssemblyHelper.BuildModel(s_CodeOptimization, globalConfig.CodeMode);
                BuildAssemblyHelper.BuildHotfix(s_CodeOptimization, globalConfig.CodeMode);

                EditorWindow game = EditorWindow.GetWindow(typeof (EditorWindow).Assembly.GetType("UnityEditor.GameView"));
                if (game != null) game.ShowNotification(new GUIContent("Build Model And Hotfix Success!"));
            }
        }
    }
}