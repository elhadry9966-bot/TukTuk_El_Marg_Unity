using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Autonomous Game Initializer & Scene Builder:
/// Guarantees that whether you run from MainScene or a fresh empty scene,
/// all game systems (Camera, Directional Light, City, Player Tuk-Tuk, Traffic,
/// Canvas UI, EventSystem, and Audio) are instantiated and wired up automatically!
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    void Awake()
    {
        SetupLighting();
        SetupCamera();
        SetupPlayer();
        SetupCity();
        SetupTraffic();
        SetupEventSystem();
        SetupUI();
        SetupGameManager();
    }

    private void SetupLighting()
    {
        Light existingSun = FindObjectOfType<Light>();
        if (existingSun == null || existingSun.type != LightType.Directional)
        {
            GameObject sunGo = new GameObject("DirectionalLight_Sun");
            Light sun = sunGo.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1.0f, 0.94f, 0.84f);
            sun.intensity = 1.4f;
            sun.shadows = LightShadows.Soft;
            sunGo.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
        }
    }

    private void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
        }

        cam.fieldOfView = 60f;
        cam.farClipPlane = 500f;
        cam.transform.position = new Vector3(-62f, 6f, -30f);

        CameraController cc = cam.GetComponent<CameraController>();
        if (cc == null) cc = cam.gameObject.AddComponent<CameraController>();
    }

    private void SetupPlayer()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();
        if (pc == null)
        {
            GameObject playerGo = new GameObject("Player_TukTuk");
            playerGo.transform.position = new Vector3(-62f, 0f, -20f);
            pc = playerGo.AddComponent<PlayerController>();

            BoxCollider col = playerGo.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 1.2f, 0);
            col.size = new Vector3(2.2f, 2.2f, 3.2f);
        }

        CameraController cc = Camera.main.GetComponent<CameraController>();
        if (cc != null) cc.target = pc.transform;
    }

    private void SetupCity()
    {
        CitySetup city = FindObjectOfType<CitySetup>();
        if (city == null)
        {
            GameObject cityGo = new GameObject("City_ElMarg");
            city = cityGo.AddComponent<CitySetup>();
        }
    }

    private void SetupTraffic()
    {
        TrafficManager tm = FindObjectOfType<TrafficManager>();
        if (tm == null)
        {
            GameObject tmGo = new GameObject("TrafficManager");
            tm = tmGo.AddComponent<TrafficManager>();
        }
    }

    private void SetupEventSystem()
    {
        EventSystem es = FindObjectOfType<EventSystem>();
        if (es == null)
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();
        }
    }

    private void SetupGameManager()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null)
        {
            GameObject gmGo = new GameObject("GameManager");
            gm = gmGo.AddComponent<GameManager>();
        }
    }

    private void SetupUI()
    {
        UIController ui = FindObjectOfType<UIController>();
        if (ui != null) return;

        // Create Canvas
        GameObject canvasGo = new GameObject("GameCanvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        ui = canvasGo.AddComponent<UIController>();

        Font defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // Top Bar HUD
        GameObject topBar = new GameObject("TopBar");
        topBar.transform.SetParent(canvasGo.transform, false);
        RectTransform rtTop = topBar.AddComponent<RectTransform>();
        rtTop.anchorMin = new Vector2(0, 1);
        rtTop.anchorMax = new Vector2(1, 1);
        rtTop.pivot = new Vector2(0.5f, 1);
        rtTop.anchoredPosition = new Vector2(0, -10);
        rtTop.sizeDelta = new Vector2(-28, 80);

        // Stats Labels
        ui.lblMoney = CreateText(topBar.transform, "lblMoney", "💵 الأجرة: 0 ج", new Vector2(-460, 20), new Vector2(180, 30), defaultFont, 16, new Color(1.0f, 0.82f, 0.4f));
        ui.lblPassengers = CreateText(topBar.transform, "lblPassengers", "👥 الركاب: 0/2", new Vector2(-280, 20), new Vector2(150, 30), defaultFont, 16, Color.white);
        ui.lblSpeed = CreateText(topBar.transform, "lblSpeed", "⚡ السرعة: 0", new Vector2(-130, 20), new Vector2(140, 30), defaultFont, 16, Color.white);
        ui.lblTimer = CreateText(topBar.transform, "lblTimer", "⏱️ الوقت: --", new Vector2(20, 20), new Vector2(140, 30), defaultFont, 16, Color.white);

        // Center Mission & Distance
        ui.lblMission = CreateText(topBar.transform, "lblMission", "📍 خريطة المرج 3D", new Vector2(260, 20), new Vector2(360, 30), defaultFont, 17, new Color(1.0f, 0.82f, 0.4f), TextAnchor.MiddleCenter);
        ui.lblDist = CreateText(topBar.transform, "lblDist", "📍 المسافة: 0 م", new Vector2(260, -10), new Vector2(360, 26), defaultFont, 14, Color.white, TextAnchor.MiddleCenter);

        // Top Action Buttons
        ui.btnShop = CreateButton(topBar.transform, "btnShop", "🔧 الورشة", new Vector2(-460, -22), new Vector2(85, 36), defaultFont);
        ui.btnTime = CreateButton(topBar.transform, "btnTime", "☀️ نهار", new Vector2(-365, -22), new Vector2(85, 36), defaultFont);
        ui.btnCam = CreateButton(topBar.transform, "btnCam", "🎥 كاميرا", new Vector2(-270, -22), new Vector2(85, 36), defaultFont);
        ui.btnVehicle = CreateButton(topBar.transform, "btnVehicle", "🚗 العربية", new Vector2(-175, -22), new Vector2(85, 36), defaultFont);

        // Toast Message Label (Center Bottom)
        ui.lblToast = CreateText(canvasGo.transform, "lblToast", "", new Vector2(0, -220), new Vector2(600, 45), defaultFont, 20, new Color(1.0f, 0.85f, 0.35f), TextAnchor.MiddleCenter);

        // Steering Touch Buttons (Bottom Left)
        ui.btnLeft = CreateHoldButton(canvasGo.transform, "btnLeft", "◀ شمال", new Vector2(-540, -280), new Vector2(90, 70), defaultFont);
        ui.btnRight = CreateHoldButton(canvasGo.transform, "btnRight", "يمين ▶", new Vector2(-435, -280), new Vector2(90, 70), defaultFont);

        // Pedals Touch Buttons (Bottom Right)
        ui.btnHorn = CreateButton(canvasGo.transform, "btnHorn", "📯 كلاكس", new Vector2(330, -280), new Vector2(85, 70), defaultFont);
        ui.btnBrake = CreateHoldButton(canvasGo.transform, "btnBrake", "فرامل", new Vector2(430, -280), new Vector2(85, 70), defaultFont);
        ui.btnGas = CreateHoldButton(canvasGo.transform, "btnGas", "بنزين", new Vector2(530, -280), new Vector2(85, 70), defaultFont);

        // Workshop Modal Panel
        GameObject shopModal = new GameObject("ShopModalPanel");
        shopModal.transform.SetParent(canvasGo.transform, false);
        Image pnlImg = shopModal.AddComponent<Image>();
        pnlImg.color = new Color(0.12f, 0.15f, 0.20f, 0.94f);
        RectTransform rtModal = shopModal.GetComponent<RectTransform>();
        rtModal.sizeDelta = new Vector2(480, 360);
        ui.shopPanel = shopModal;

        CreateText(shopModal.transform, "ShopTitle", "🔧 ورشة توك توك المرج", new Vector2(0, 140), new Vector2(400, 35), defaultFont, 22, new Color(1.0f, 0.82f, 0.4f), TextAnchor.MiddleCenter);
        ui.lblShopMoney = CreateText(shopModal.transform, "lblShopMoney", "رصيدك الحالي: 0 جنيه", new Vector2(0, 100), new Vector2(400, 28), defaultFont, 16, Color.white, TextAnchor.MiddleCenter);

        ui.btnBuyEngine = CreateButton(shopModal.transform, "btnBuyEngine", "🚀 تعلية الموتور (1/4) - 50 ج", new Vector2(0, 50), new Vector2(360, 42), defaultFont);
        ui.btnBuyBrakes = CreateButton(shopModal.transform, "btnBuyBrakes", "🛑 فرامل وعفشة (1/4) - 40 ج", new Vector2(0, -2), new Vector2(360, 42), defaultFont);
        ui.btnBuyFare = CreateButton(shopModal.transform, "btnBuyFare", "🪑 كراسي مريحة (1/4) - 60 ج", new Vector2(0, -54), new Vector2(360, 42), defaultFont);
        ui.btnCloseShop = CreateButton(shopModal.transform, "btnCloseShop", "رجوع لشوارع المرج ▶", new Vector2(0, -120), new Vector2(260, 40), defaultFont);

        shopModal.SetActive(false);
    }

    private Text CreateText(Transform parent, string name, string text, Vector2 pos, Vector2 size, Font font, int fontSize, Color col, TextAnchor align = TextAnchor.MiddleLeft)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Text t = go.AddComponent<Text>();
        t.text = text;
        t.font = font;
        t.fontSize = fontSize;
        t.color = col;
        t.alignment = align;
        return t;
    }

    private Button CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Font font)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.24f, 0.30f, 0.38f);

        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.35f, 0.44f, 0.55f);
        cb.pressedColor = new Color(0.18f, 0.22f, 0.28f);
        btn.colors = cb;

        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        RectTransform rtTxt = textGo.AddComponent<RectTransform>();
        rtTxt.sizeDelta = size;
        Text t = textGo.AddComponent<Text>();
        t.text = label;
        t.font = font;
        t.fontSize = 15;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleCenter;

        return btn;
    }

    private HoldButton CreateHoldButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Font font)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = go.AddComponent<Image>();
        img.color = new Color(0.24f, 0.30f, 0.38f);

        HoldButton hb = go.AddComponent<HoldButton>();

        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        RectTransform rtTxt = textGo.AddComponent<RectTransform>();
        rtTxt.sizeDelta = size;
        Text t = textGo.AddComponent<Text>();
        t.text = label;
        t.font = font;
        t.fontSize = 16;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleCenter;

        return hb;
    }
}
