using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BowlingUISetup : MonoBehaviour
{
    [Header("Canvas Settings")]
    public Canvas mainCanvas;
    public Vector2 canvasSize = new Vector2(1920, 1080);

    [Header("Font Settings")]
    public Font customFont;
    public int scoreFontSize = 60;
    public int statusFontSize = 40;
    public int frameFontSize = 30;

#if UNITY_EDITOR
    [CustomEditor(typeof(BowlingUISetup))]
    public class BowlingUISetupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BowlingUISetup setup = (BowlingUISetup)target;

            if (GUILayout.Button("GENERATE COMPLETE UI", GUILayout.Height(40)))
            {
                setup.GenerateCompleteUI();
            }
        }
    }
#endif

    public void GenerateCompleteUI()
    {
        if (mainCanvas == null)
        {
            Debug.LogError("❌ Canvas belum di-assign! Drag Canvas ke field 'Main Canvas'");
            return;
        }

        // Hapus UI lama jika ada
        Transform oldPanel = mainCanvas.transform.Find("ScorePanel");
        if (oldPanel != null) DestroyImmediate(oldPanel.gameObject);
        
        Transform oldFramePanel = mainCanvas.transform.Find("FrameScorePanel");
        if (oldFramePanel != null) DestroyImmediate(oldFramePanel.gameObject);

        CreateBowlingUI();
    }

    public void CreateBowlingUI()
    {
        GameObject panel = CreatePanel("ScorePanel", mainCanvas.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0.8f);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.offsetMin = new Vector2(20, -200);
        panelRect.offsetMax = new Vector2(-20, -20);
        
        Image panelImg = panel.GetComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.7f);

        // 3. Score Text (Besar di tengah atas)
        GameObject scoreTextObj = CreateText("ScoreText", panel.transform, "Score: 0", scoreFontSize);
        RectTransform scoreRect = scoreTextObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0.3f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.7f, 1);
        scoreRect.offsetMin = Vector2.zero;
        scoreRect.offsetMax = Vector2.zero;
        
        Text scoreText = scoreTextObj.GetComponent<Text>();
        scoreText.alignment = TextAnchor.MiddleCenter;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.color = Color.white;

        // 4. Frame Text
        GameObject frameTextObj = CreateText("FrameText", panel.transform, "Frame: 1/10", frameFontSize);
        RectTransform frameRect = frameTextObj.GetComponent<RectTransform>();
        frameRect.anchorMin = new Vector2(0, 0.5f);
        frameRect.anchorMax = new Vector2(0.3f, 1);
        frameRect.offsetMin = Vector2.zero;
        frameRect.offsetMax = Vector2.zero;
        
        Text frameText = frameTextObj.GetComponent<Text>();
        frameText.alignment = TextAnchor.MiddleCenter;

        // 5. Target Score Text
        GameObject targetTextObj = CreateText("TargetText", panel.transform, "Target: 100", frameFontSize);
        RectTransform targetRect = targetTextObj.GetComponent<RectTransform>();
        targetRect.anchorMin = new Vector2(0.7f, 0.5f);
        targetRect.anchorMax = new Vector2(1, 1);
        targetRect.offsetMin = Vector2.zero;
        targetRect.offsetMax = Vector2.zero;
        
        Text targetText = targetTextObj.GetComponent<Text>();
        targetText.alignment = TextAnchor.MiddleCenter;
        targetText.color = Color.yellow;

        // 6. Status Text (STRIKE/SPARE/dll)
        GameObject statusTextObj = CreateText("StatusText", panel.transform, "", statusFontSize);
        RectTransform statusRect = statusTextObj.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.2f, 0);
        statusRect.anchorMax = new Vector2(0.8f, 0.5f);
        statusRect.offsetMin = Vector2.zero;
        statusRect.offsetMax = Vector2.zero;
        
        Text statusText = statusTextObj.GetComponent<Text>();
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.fontStyle = FontStyle.Bold;
        statusText.color = Color.green;

        // 7. Frame Score Display (10 kotak kecil)
        GameObject frameScorePanel = CreatePanel("FrameScorePanel", mainCanvas.transform);
        RectTransform frameScoreRect = frameScorePanel.GetComponent<RectTransform>();
        frameScoreRect.anchorMin = new Vector2(0, 0.6f);
        frameScoreRect.anchorMax = new Vector2(1, 0.75f);
        frameScoreRect.offsetMin = new Vector2(20, 0);
        frameScoreRect.offsetMax = new Vector2(-20, 0);
        
        Image frameScorePanelImg = frameScorePanel.GetComponent<Image>();
        frameScorePanelImg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        // Buat 10 kotak frame
        Text[] frameTexts = new Text[10];
        for (int i = 0; i < 10; i++)
        {
            GameObject frameBox = CreateText($"Frame{i + 1}", frameScorePanel.transform, $"{i + 1}", 20);
            RectTransform boxRect = frameBox.GetComponent<RectTransform>();
            
            float width = 1f / 10f;
            boxRect.anchorMin = new Vector2(i * width, 0);
            boxRect.anchorMax = new Vector2((i + 1) * width, 1);
            boxRect.offsetMin = new Vector2(5, 5);
            boxRect.offsetMax = new Vector2(-5, -5);
            
            Text boxText = frameBox.GetComponent<Text>();
            boxText.alignment = TextAnchor.MiddleCenter;
            boxText.fontSize = 20;
            
            frameTexts[i] = boxText;
        }

        // 8. Link ke BowlingScoreManager
        BowlingScoreManager scoreManager = FindObjectOfType<BowlingScoreManager>();
        if (scoreManager == null)
        {
            GameObject managerObj = new GameObject("BowlingScoreManager");
            scoreManager = managerObj.AddComponent<BowlingScoreManager>();
        }

        scoreManager.scoreText = scoreText;
        scoreManager.frameText = frameText;
        scoreManager.statusText = statusText;
        scoreManager.targetScoreText = targetText;
        scoreManager.frameScoreTexts = frameTexts;

        Debug.Log("✅ UI Bowling lengkap telah dibuat! Cek Canvas hierarchy.");
    }

    GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.localPosition = Vector3.zero;
        
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.5f);
        
        return panel;
    }

    GameObject CreateText(string name, Transform parent, string content, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.localPosition = Vector3.zero;
        
        Text text = textObj.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.font = customFont != null ? customFont : Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        return textObj;
    }
}
