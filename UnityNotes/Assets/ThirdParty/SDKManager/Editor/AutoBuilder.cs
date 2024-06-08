/* 
AutoBuilder.cs
Automatically changes the target platform and creates a build.
 
Installation
Place in an Editor folder.
 
Usage
Go to File > AutoBuilder and select a platform. These methods can also be run from the Unity command line using -executeMethod AutoBuilder.MethodName.
 
License
Copyright (C) 2011 by Thinksquirrel Software, LLC
 
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
 
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using static UnityEngine.GraphicsBuffer;

public static class AutoBuilder
{

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    public static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }

    [MenuItem("AutoBuilder/Windows/32-bit")]
    static void PerformWinBuild()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Win/" + GetProjectName() + ".exe", buildTarget, BuildOptions.None);
    }

    [MenuItem("AutoBuilder/Windows/64-bit")]
    static void PerformWin64Build()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(BuildTarget.StandaloneWindows64);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Win64/" + GetProjectName() + ".exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    [MenuItem("AutoBuilder/Mac OSX/Universal")]
    static void PerformOSXUniversalBuild()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneOSX;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/OSX-Universal/" + GetProjectName() + ".app", buildTarget, BuildOptions.None);
    }

    [MenuItem("AutoBuilder/Mac OSX/Intel")]
    static void PerformOSXIntelBuild()
    {
        BuildTarget buildTarget = BuildTarget.StandaloneOSX;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/OSX-Intel/" + GetProjectName() + ".app", buildTarget, BuildOptions.None);
    }

    [MenuItem("AutoBuilder/Mac OSX/PPC")]
    static void PerformOSXPPCBuild()
    {
        //EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneOSXPPC);
        //BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/OSX-PPC/" + GetProjectName() + ".app", BuildTarget.StandaloneOSXPPC, BuildOptions.None);
    }

    [MenuItem("AutoBuilder/Mac OSX/Dashboard")]
    static void PerformOSXDashboardBuild()
    {
        //EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.DashboardWidget);
        //BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/OSX-Dashboard/" + GetProjectName() + ".wdgt", BuildTarget.DashboardWidget, BuildOptions.None);
    }

    [MenuItem("AutoBuilder/iOS")]
    static void PerformiOSBuild()
    {
        BuildTarget buildTarget = BuildTarget.iOS;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/iOS", buildTarget, BuildOptions.None);
    }
    [MenuItem("AutoBuilder/Android")]
    static void PerformAndroidBuild()
    {
        BuildTarget buildTarget = BuildTarget.Android;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Android/Monopoly" + System.DateTime.Now.ToString("MM-dd") + ".apk", buildTarget, BuildOptions.None);
    }
    [MenuItem("AutoBuilder/Web/Standard")]
    static void PerformWebBuild()
    {
        BuildTarget buildTarget = BuildTarget.WebGL;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Web", buildTarget, BuildOptions.None);
    }
    [MenuItem("AutoBuilder/Web/Streamed")]
    static void PerformWebStreamedBuild()
    {
        BuildTarget buildTarget = BuildTarget.WebGL;
        BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
        EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
        BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Web-Streamed", buildTarget, BuildOptions.None);
    }
}