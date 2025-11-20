using System;
using System.Net.Sockets;
using Henry.EditorKit;
using Henry.EditorKit.Component;
using UnityEditor;
using UnityEngine;

namespace Henry.EditorKit.BuiltInComponent
{
    public class CacheServerTool : ScriptableObject, IComponent
    {
        private const string ModeKey = "CacheServer2Mode";
        private const string IPKey = "CacheServer2IPAddress";
        private const int DefaultPort = 8126;

        private string connectionStatusMessage;
        private MessageType connectionStatusType;
        private string globalEndpoint;
        private int globalMode;

        public static Config Info => new("Cache Server Tool")
        {
            Author = "李育杰",
            Version = "1.0.0"
        };

        void IComponent.OnEnable()
        {
            globalMode = EditorPrefs.GetInt(ModeKey, 1);
            globalEndpoint = EditorPrefs.GetString(IPKey, string.Empty);
        }

        void IComponent.OnGUI(Rect rect)
        {
            EditorGUI.BeginChangeCheck();
            var currentMode = EditorSettings.cacheServerMode;
            var newMode = (CacheServerMode)EditorGUILayout.EnumPopup("Project Mode", currentMode);

            if (EditorGUI.EndChangeCheck())
            {
                EditorSettings.cacheServerMode = newMode;
                ResetConnectionMessage();
            }

            if (newMode == CacheServerMode.Disabled && globalMode == 0)
            {
                newMode = CacheServerMode.AsPreferences;
            }

            switch (newMode)
            {
                case CacheServerMode.AsPreferences:
                    DrawGlobalMode();
                    break;
                case CacheServerMode.Enabled:
                    DrawLocalMode();
                    break;
            }

            EditorGUILayout.Space();
        }

        private void DrawGlobalMode()
        {
            EditorGUI.indentLevel++;
            DrawGlobalSettings();
            DrawConnectionCheck(globalEndpoint);
            EditorGUI.indentLevel--;
        }

        private void DrawLocalMode()
        {
            EditorGUI.indentLevel++;
            DrawLocalIpSettings();
            DrawConnectionCheck(EditorSettings.cacheServerEndpoint);
            EditorGUI.indentLevel--;
            DrawLocalPermissions();
        }

        private void DrawLocalPermissions()
        {
            EditorGUILayout.LabelField("權限設定", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorSettings.cacheServerNamespacePrefix = EditorGUILayout.TextField("指定 Namespace", EditorSettings.cacheServerNamespacePrefix);
            EditorSettings.cacheServerEnableDownload = EditorGUILayout.Toggle("允許下載 (Download)", EditorSettings.cacheServerEnableDownload);
            EditorSettings.cacheServerEnableUpload = EditorGUILayout.Toggle("允許上傳 (Upload)", EditorSettings.cacheServerEnableUpload);
            EditorGUI.indentLevel--;
        }

        private void DrawGlobalSettings()
        {
            EditorGUI.BeginChangeCheck();

            var isEnabled = globalMode == 0;
            var newEnabled = EditorGUILayout.Toggle("Global Enable", isEnabled);

            if (EditorGUI.EndChangeCheck())
            {
                globalMode = newEnabled ? 0 : 1;
                EditorPrefs.SetInt(ModeKey, globalMode);
            }

            if (newEnabled)
            {
                EditorGUI.BeginChangeCheck();
                var newEndpoint = EditorGUILayout.DelayedTextField("Global Endpoint", globalEndpoint);
                if (EditorGUI.EndChangeCheck())
                {
                    globalEndpoint = newEndpoint;
                    EditorPrefs.SetString(IPKey, newEndpoint);
                }
            }
        }

        private void DrawLocalIpSettings()
        {
            EditorGUI.BeginChangeCheck();
            var newEndpoint = EditorGUILayout.DelayedTextField("Endpoint", EditorSettings.cacheServerEndpoint);

            if (EditorGUI.EndChangeCheck())
            {
                EditorSettings.cacheServerEndpoint = newEndpoint;
                ResetConnectionMessage();
            }
        }

        private void DrawConnectionCheck(string ip)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Check Connection"))
            {
                (connectionStatusMessage, connectionStatusType) = TestConnection(ip);
            }

            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(connectionStatusMessage))
            {
                EditorGUILayout.HelpBox(connectionStatusMessage, connectionStatusType);
            }
        }

        private void ResetConnectionMessage()
        {
            connectionStatusMessage = string.Empty;
            connectionStatusType = MessageType.None;
        }

        private (string, MessageType) TestConnection(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                return ("請先輸入 IP 位址", MessageType.Warning);
            }

            var parts = endpoint.Split(':');
            var ip = parts[0];
            var port = DefaultPort;

            if (parts.Length > 1)
            {
                if (!int.TryParse(parts[1], out port))
                {
                    return ("Port 格式錯誤", MessageType.Error);
                }
            }

            try
            {
                using TcpClient client = new TcpClient();
                var result = client.BeginConnect(ip, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

                if (success)
                {
                    client.EndConnect(result);
                    return ($"連線成功！ (Connected to {endpoint})", MessageType.Info);
                }

                return ($"連線逾時，無法抵達伺服器 ({endpoint})", MessageType.Error);
            }
            catch (Exception e)
            {
                return ($"連線失敗: {e.Message}", MessageType.Error);
            }
        }
    }
}