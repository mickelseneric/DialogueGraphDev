using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dlog.Development {
    public static class GitTool {
        public static string DevelopmentPath => Path.Combine(DevelopmentFolder, @"saxion\unity\dialogue_graph_dev");
        public static string DialogueGraphPath => Path.Combine(DevelopmentFolder, DevelopmentPath, @"Assets\DialogueGraph");
        private static string DevelopmentFolder => Environment.GetEnvironmentVariable("dev", EnvironmentVariableTarget.User);

        private static int maxWaitTime = 1000;

        public static void CommitBumpVersion(SemVer oldVersion, SemVer newVersion) {
            ExecuteGit(DialogueGraphPath, "reset");
            ExecuteGit(DialogueGraphPath, "add package.json");
            ExecuteGit(DialogueGraphPath, "add Resources/DialogueGraphVersion.asset");
            ExecuteGit(DialogueGraphPath, $"commit -m \"Bump Dialogue Graph version [{oldVersion} -> {newVersion}]\"");
            
        }

        private static string ExecuteGit(string repository, string command) {
            var prc = new System.Diagnostics.Process {
                StartInfo = {
                    FileName = "git.exe",
                    Arguments = $"-C \"{repository}\" {command}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            prc.Start();
            prc.WaitForExit(maxWaitTime);
            return prc.StandardOutput.ReadToEnd();
        }
    }
}