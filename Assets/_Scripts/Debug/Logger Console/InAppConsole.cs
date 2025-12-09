using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System;


/*
 * Summary:
 *      A runtime debug logger console inside the application
*/
public class InAppConsole : MonoBehaviour
{
    [Header("Console UI")]
    [SerializeField] Button toggleButton;
    [SerializeField] Button clearButton;
    [SerializeField] ScrollRect consoleScrollRect;
    [SerializeField] GameObject consolePanel;
    [SerializeField] TMP_Text logText;

    [Header("Console Settings")]
    [SerializeField] int maxLogs = 50;
    [SerializeField] bool showErrors = true;
    [SerializeField] bool showWarnings = true;
    [SerializeField] bool showInfos = true;

    private Queue<string> logs = new Queue<string>();
    private StringBuilder sb = new StringBuilder();
    private bool isConsoleVisible = false;

    // Singleton instance of InAppConsole (One logger console for the whole app)
    public static InAppConsole Instance { get; private set; }

    private void Awake()
    {
        // Singleton logic for the InAppConsole static object
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLogs;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLogs;
    }

    private void Start()
    {
        consolePanel.SetActive(isConsoleVisible);
        SetConsoleBtnText();
        if (toggleButton != null) toggleButton.onClick.AddListener(ToggleConsolePanel);
        if (clearButton != null) clearButton.onClick.AddListener(ClearConsolePanel);
        Debug.Log("[InAppConsole/Start()] Logger console initialized");
    }

    // Handle the logs
    private void HandleLogs(string logString, string stackTrace, LogType type)
    {
        string formattedLog = "";
        // Format log based on type
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                if (!showErrors) return;
                formattedLog = $"<color=#FF0000>[ERROR] {logString}</color>";
                break;
            case LogType.Warning:
                if (!showWarnings) return;
                formattedLog = $"<color=yellow>[WARN] {logString}</color>";
                break;
            default:
                if (!showInfos) return;
                formattedLog = $"[INFO] {logString}";
                break;
        }
        // Add log timestamps
        string timeStamp = DateTime.Now.ToString("HH:mm:ss");
        formattedLog = $"[{timeStamp}] {formattedLog}";
        // Add the formatted log to the log queue
        logs.Enqueue(formattedLog);
        // Dequeue logs if the total logs greater than maximum number of logs allowed
        while (logs.Count > maxLogs)
        {
            logs.Dequeue();
        }
        // Build the logs using StringBuilder for text display
        sb.Clear();
        foreach (string log in logs)
        {
            sb.AppendLine(log);
        }
        // Update the UI
        if (logText != null)
        {
            logText.text = sb.ToString();
        }
    }
    
    // Toggle the logger console UI panel
    public void ToggleConsolePanel()
    {
        isConsoleVisible = !isConsoleVisible;
        if (consolePanel != null)
            consolePanel.SetActive(isConsoleVisible);
        if (toggleButton != null)
        {
            SetConsoleBtnText();
        }
    }

    // Change the console button text on toggle
    private void SetConsoleBtnText()
    {
        TMP_Text buttonText = toggleButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = isConsoleVisible ? "X" : ">_";
    }

    // Clear logger console panel
    public void ClearConsolePanel()
    {
        logs.Clear();
        sb.Clear();
        if (logText != null) logText.text = "";
    }
}
