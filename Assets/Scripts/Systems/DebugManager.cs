using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{

    private float deltaTime;
    private float fps;
    public static DebugManager _instance;
    public GUISkin guiSkin;
    public void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this);

    }
    void Update()
    {
        if (Time.timeScale > 0)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            fps = Mathf.RoundToInt(1.0f / deltaTime);
        }
    }
    void OnGUI()
    {

        int index = 1;
        int height = 50;
        int spacing = 10;
        GUI.Label(new Rect(25, height * index + spacing, 100, height), (fps).ToString(), guiSkin.label);
        index++;

    }
}
