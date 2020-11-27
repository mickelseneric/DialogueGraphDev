using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Dlog.Development {
    public class DlogVersioningWindow : EditorWindow {
        private GUIStyle bigLabel = null;
        private GUIStyle bigLabelCenter = null;
        private GUIStyle bigLabelRight = null;
        private GUIStyle label = null;
        private GUIStyle labelCenter = null;
        private GUIStyle labelRight = null;
        private GUIStyle button = null;
        private GUIStyle smallButton = null;

        private string enteredVersion;
        private bool shouldSetStyles;

        private SemVer version;
        private SemVer originalVersion;

        [MenuItem("Dialogue Graph/Versioning")]
        private static void OpenMenu() {
            var window = GetWindow<DlogVersioningWindow>();
            window.titleContent = new GUIContent("Dialogue Graph - Versioning");
            var minSize = window.minSize;
            minSize.x = 500;
            window.minSize = minSize;
            window.Initialize();
            window.Show();
        }

        private void Initialize() {
            version = DlogVersion.Version.GetValue();
            originalVersion = version;
            enteredVersion = originalVersion;
        }

        private void OnEnable() {
            shouldSetStyles = true;
        }

        private void SetStyles() {
            bigLabel = new GUIStyle(GUI.skin.label);
            label = new GUIStyle(GUI.skin.label);
            button = new GUIStyle(GUI.skin.button) {alignment = TextAnchor.MiddleCenter};

            bigLabel.fontSize = 28;
            bigLabel.fontStyle = FontStyle.Bold;
            bigLabel.richText = true;
            bigLabelCenter = new GUIStyle(bigLabel) {alignment = TextAnchor.MiddleCenter};
            bigLabelRight = new GUIStyle(bigLabel) {alignment = TextAnchor.MiddleRight};
            label.fontSize = 14;
            label.richText = true;
            labelCenter = new GUIStyle(label) {alignment = TextAnchor.MiddleCenter};
            labelRight = new GUIStyle(label) {alignment = TextAnchor.MiddleRight};
            button.fontSize = 14;
            button.richText = true;
            smallButton = new GUIStyle(button) {fontSize = 10};
        }

        private void OnGUI() {
            if (shouldSetStyles) {
                SetStyles();
                shouldSetStyles = false;
            }

            var guiColor = GUI.color;
            GUILayout.Label("Versioning", bigLabelCenter);
            GUILayout.BeginHorizontal();
            GUI.color = Color.yellow;
            GUILayout.Label($"Current {version}", labelRight);
            GUILayout.Label($"(Committed {DlogVersion.Version.GetValue()})", label);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = version != DlogVersion.Version.GetValue();
            if (GUILayout.Button("Reset", smallButton)) {
                version = DlogVersion.Version.GetValue();
            }

            if (GUILayout.Button("Commit", smallButton)) {
                CommitVersion();
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUI.color = guiColor;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.MaxHeight(32));
            GUI.backgroundColor = Color.red;
            GUILayout.Space(32);
            GUILayoutHelper.CenterVertically(() => {
                if (GUILayout.Button("Bump MAJOR")) {
                    version.BumpMajor();
                }
            });
            GUILayoutHelper.CenterVertically(() => {
                if (GUILayout.Button("Bump MINOR")) {
                    version.BumpMinor();
                }
            });
            GUILayoutHelper.CenterVertically(() => {
                if (GUILayout.Button("Bump PATCH")) {
                    version.BumpPatch();
                }
            });
            GUILayout.Space(32);
            GUI.backgroundColor = guiColor;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(GUILayout.MaxHeight(32));
            GUILayout.Space(32);
            GUILayoutHelper.CenterVertically(() => { GUILayout.Label("(Enter Manually) Version", labelRight); });
            GUILayoutHelper.CenterVertically(() => { enteredVersion = GUILayout.TextField(enteredVersion); });
            if (!SemVer.IsValid(enteredVersion)) GUI.enabled = false;
            GUILayoutHelper.CenterVertically(() => {
                if (GUILayout.Button("Set", button))
                    version = (SemVer) enteredVersion;
            });
            GUI.enabled = true;
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
        }

        private void CommitVersion() {
            DlogVersion.SaveVersion(version);
            GitTool.CommitBumpVersion(originalVersion, version);
            originalVersion = version;
        }
    }
}