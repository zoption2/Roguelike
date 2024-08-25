using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class DetailedEventSubscriptionTracker : EditorWindow
{
    private Dictionary<string, EventSubscriptionInfo> eventSubscriptionInfoWithUnderscore = new Dictionary<string, EventSubscriptionInfo>();
    private Dictionary<string, EventSubscriptionInfo> eventSubscriptionInfoWithoutUnderscore = new Dictionary<string, EventSubscriptionInfo>();
    private Vector2 scrollPosition;

    private string hoveredCodeSnippet = null;

    [MenuItem("Tools/Subscription Tracker")]
    public static void ShowWindow()
    {
        GetWindow<DetailedEventSubscriptionTracker>("Subscription Tracker");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Analyze Subscriptions"))
        {
            AnalyzeSubscriptions();
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        //GUILayout.Label("Events:", EditorStyles.boldLabel);
        DisplayEvents(eventSubscriptionInfoWithUnderscore);

        //GUILayout.Space(20);
        //GUILayout.Label("Other:", EditorStyles.boldLabel);
        //DisplayEvents(eventSubscriptionInfoWithoutUnderscore);

        EditorGUILayout.EndScrollView();

        if (!string.IsNullOrEmpty(hoveredCodeSnippet))
        {
            var mousePosition = Event.current.mousePosition;

            var style = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                fontSize = 12,
                normal = { background = MakeTex(2, 2, new Color(0.22f, 0.22f, 0.22f, 1f)) }
            };

            var content = new GUIContent(hoveredCodeSnippet);
            var size = style.CalcSize(content);
            GUI.Box(new Rect(mousePosition.x + 15, mousePosition.y + 15, size.x, size.y), GUIContent.none, style);

            string[] lines = hoveredCodeSnippet.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(">>>"))
                {
                    GUI.color = new Color(1.0f, 0.9f, 0.6f);

                    lines[i] = lines[i].Substring(3);
                }
                else
                {
                    GUI.color = Color.white;
                }

                Rect lineRect = new Rect(mousePosition.x + 15, mousePosition.y + 15 + i * (style.lineHeight + 2), size.x, style.lineHeight);
                GUI.Label(lineRect, lines[i], style);
            }

            GUI.color = Color.white;
        }

        Repaint();
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    private void DisplayElementWithHover(SubscriptionDetail element)
    {
        GUILayout.BeginHorizontal();

        GUIContent content = new GUIContent($"  - {element.SubscriberName}");
        Rect labelRect = GUILayoutUtility.GetRect(content, EditorStyles.label);

        GUI.Label(labelRect, content);

        if (labelRect.Contains(Event.current.mousePosition))
        {
            hoveredCodeSnippet = GetCodeSnippet(element.FilePath, element.LineNumber);
            Repaint();
        }
        else if (hoveredCodeSnippet == GetCodeSnippet(element.FilePath, element.LineNumber))
        {
            hoveredCodeSnippet = null;
            Repaint();
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Go to", GUILayout.Width(50)))
        {
            OpenScriptAtLine(element.FilePath, element.LineNumber);
        }

        GUILayout.EndHorizontal();
    }


    private void DisplayEvents(Dictionary<string, EventSubscriptionInfo> eventSubscriptionInfo)
    {
        foreach (var eventInfo in eventSubscriptionInfo)
        {
            if (eventInfo.Value.SubscriptionCount > eventInfo.Value.UnsubscriptionCount)
            {
                GUI.backgroundColor = Color.yellow;
            }
            else
            {
                GUI.backgroundColor = Color.white;
            }

            GUILayout.BeginVertical("box");

            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1f);

            eventInfo.Value.IsExpanded = EditorGUILayout.Foldout(eventInfo.Value.IsExpanded, $"{eventInfo.Key} ({eventInfo.Value.SubscriptionCount} subscriptions, {eventInfo.Value.UnsubscriptionCount} unsubscriptions)");

            if (eventInfo.Value.IsExpanded)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Label("Subscribed Elements:");
                foreach (var element in eventInfo.Value.SubscribedElements)
                {
                    DisplayElementWithHover(element);
                }
                GUILayout.EndVertical();

                if (eventInfo.Value.UnsubscriptionCount > 0)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label("Unsubscribed Elements:");
                    foreach (var element in eventInfo.Value.UnsubscribedElements)
                    {
                        DisplayElementWithHover(element);
                    }
                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndVertical();
        }

        GUI.backgroundColor = Color.white;
    }

    private string GetCodeSnippet(string filePath, int lineNumber)
    {
        var lines = File.ReadAllLines(filePath);
        var snippet = new System.Text.StringBuilder();

        int startLine = Mathf.Max(lineNumber - 3, 0);
        int endLine = Mathf.Min(lineNumber + 1, lines.Length);

        for (int i = startLine; i < endLine; i++)
        {
            if (i == lineNumber - 1 && (lines[i].Contains("+=") || lines[i].Contains("-=")))
            {
                snippet.AppendLine(">>>" + lines[i]);
            }
            else
            {
                snippet.AppendLine(lines[i]);
            }
        }

        return snippet.ToString();
    }

    private void AnalyzeSubscriptions()
    {
        eventSubscriptionInfoWithUnderscore.Clear();
        eventSubscriptionInfoWithoutUnderscore.Clear();

        string[] scriptFiles = Directory.GetFiles(Application.dataPath + "/Scripts", "*.cs", SearchOption.AllDirectories);

        foreach (var file in scriptFiles)
        {
            string[] lines = File.ReadAllLines(file);
            AnalyzeFileContent(lines, file);
        }
    }

    private void AnalyzeFileContent(string[] lines, string filePath)
    {
        string subscriptionPattern = @"(\w+)\s*\+=\s*(\w+)";
        string unsubscriptionPattern = @"(\w+)\s*-\=\s*(\w+)";

        var eventSubscriptionCount = new Dictionary<string, int>();
        var eventUnsubscriptionCount = new Dictionary<string, int>();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            Match match = Regex.Match(line, subscriptionPattern);
            if (match.Success)
            {
                string eventName = match.Groups[1].Value;
                string subscriber = match.Groups[2].Value;

                var targetDictionary = eventName.Contains("_") ? eventSubscriptionInfoWithUnderscore : eventSubscriptionInfoWithoutUnderscore;

                if (!targetDictionary.ContainsKey(eventName))
                {
                    targetDictionary[eventName] = new EventSubscriptionInfo();
                }

                targetDictionary[eventName].SubscriptionCount++;
                targetDictionary[eventName].SubscribedElements.Add(new SubscriptionDetail(subscriber, filePath, i + 1));

                if (!eventSubscriptionCount.ContainsKey(eventName))
                {
                    eventSubscriptionCount[eventName] = 0;
                }
                eventSubscriptionCount[eventName]++;
            }

            match = Regex.Match(line, unsubscriptionPattern);
            if (match.Success)
            {
                string eventName = match.Groups[1].Value;
                string unsubscribedElement = match.Groups[2].Value;

                var targetDictionary = eventName.Contains("_") ? eventSubscriptionInfoWithUnderscore : eventSubscriptionInfoWithoutUnderscore;

                if (!targetDictionary.ContainsKey(eventName))
                {
                    targetDictionary[eventName] = new EventSubscriptionInfo();
                }

                targetDictionary[eventName].UnsubscriptionCount++;
                targetDictionary[eventName].UnsubscribedElements.Add(new SubscriptionDetail(unsubscribedElement, filePath, i + 1));

                if (!eventUnsubscriptionCount.ContainsKey(eventName))
                {
                    eventUnsubscriptionCount[eventName] = 0;
                }
                eventUnsubscriptionCount[eventName]++;
            }
        }
    }

    private void OpenScriptAtLine(string filePath, int lineNumber)
    {
        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath, lineNumber);
    }

    private class EventSubscriptionInfo
    {
        public int SubscriptionCount { get; set; }
        public int UnsubscriptionCount { get; set; }
        public List<SubscriptionDetail> SubscribedElements { get; private set; } = new List<SubscriptionDetail>();
        public List<SubscriptionDetail> UnsubscribedElements { get; private set; } = new List<SubscriptionDetail>();
        public bool IsExpanded { get; set; }
    }

    private class SubscriptionDetail
    {
        public string SubscriberName { get; }
        public string FilePath { get; }
        public int LineNumber { get; }

        public SubscriptionDetail(string subscriberName, string filePath, int lineNumber)
        {
            SubscriberName = subscriberName;
            FilePath = filePath;
            LineNumber = lineNumber;
        }
    }
}
