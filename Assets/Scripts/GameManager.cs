using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Master Game Controller for TukTuk El Marg 3D in Unity:
/// - Manages Economy (Money, Passengers, Upgrades)
/// - Missions (Passenger pickup & delivery in El Marg streets)
/// - 16 El Marg Street & Landmark Locations
/// - Time of Day presets (Day, Sunset, Night)
/// - Vehicle switcher
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public struct ElMargLocation
    {
        public string name;
        public Vector3 pos;
        public Color color;
        public Color accent;
        public bool isShop;

        public ElMargLocation(string n, Vector3 p, string colHex, string accHex, bool shop)
        {
            name = n;
            pos = p;
            ColorUtility.TryParseHtmlString(colHex, out color);
            ColorUtility.TryParseHtmlString(accHex, out accent);
            isShop = shop;
        }
    }

    public static readonly ElMargLocation[] ElMargLocations = new ElMargLocation[]
    {
        new ElMargLocation("قسم المرج", new Vector3(55f, 0f, -190f), "#1e3a8a", "#38bdf8", true),
        new ElMargLocation("شارع الترعة", new Vector3(-8f, 0f, -192f), "#0f766e", "#2dd4bf", false),
        new ElMargLocation("شارع محمد حماد", new Vector3(-100f, 0f, -138f), "#1e293b", "#fbbf24", false),
        new ElMargLocation("شارع سيد عنبر", new Vector3(86f, 0f, -120f), "#1e293b", "#fbbf24", false),
        new ElMargLocation("شارع البرماوي", new Vector3(186f, 0f, -120f), "#1e293b", "#fbbf24", false),
        new ElMargLocation("شارع سويدان", new Vector3(24f, 0f, -40f), "#1e293b", "#fbbf24", false),
        new ElMargLocation("مطعم طشه", new Vector3(-62f, 0f, 12f), "#9a3412", "#fb923c", true),
        new ElMargLocation("حلال ماركت المرج", new Vector3(-62f, 0f, 42f), "#1d4ed8", "#60a5fa", true),
        new ElMargLocation("شارع بدران", new Vector3(-36f, 0f, 62f), "#1e293b", "#fbbf24", false),
        new ElMargLocation("شارع محمد رجب", new Vector3(15f, 0f, 94f), "#1e3a8a", "#fde047", false),
        new ElMargLocation("شارع شوقي", new Vector3(-92f, 0f, 88f), "#1e293b", "#fbbf24", false),
        new ElMargLocation("فرن حلمي", new Vector3(-100f, 0f, 136f), "#78350f", "#fcd34d", true),
        new ElMargLocation("شارع صلاح أحمد الاقرع", new Vector3(-25f, 0f, 134f), "#1e293b", "#fbbf24", false),
        new ElMargLocation("شارع عبد الغفار الاقرع", new Vector3(-23f, 0f, 164f), "#1e293b", "#fbbf24", false),
        new ElMargLocation("شارع أم عويس", new Vector3(86f, 0f, 145f), "#1e293b", "#fbbf24", false),
        new ElMargLocation("شارع حجاج سالم", new Vector3(186f, 0f, 145f), "#1e293b", "#fbbf24", false)
    };

    [Header("Game State")]
    public int money = 0;
    public int passengers = 0;
    public float tripTimer = 0.0f;
    public bool isDelivery = false;
    public int currentLocIdx = -1;
    public bool isPaused = false;

    [Header("Upgrades")]
    public int engineLevel = 1; // 1 to 4
    public int brakeLevel = 1;  // 1 to 4
    public int fareLevel = 1;   // 1 to 4

    [Header("References")]
    public PlayerController player;
    public CameraController camController;
    public Light sunLight;
    public MissionMarker missionMarker;

    [Header("Vehicle Prefabs")]
    public GameObject[] carPrefabs;
    public int currentVehicleIdx = 0;

    private int timeMode = 0;
    private float slowHintCooldown = 0.0f;

    private struct TimePreset
    {
        public string label;
        public Color sunColor;
        public float sunIntensity;
        public Vector3 sunAngles;
        public Color fogColor;
        public float fogDensity;
        public float headlightIntensity;

        public TimePreset(string lbl, string sunHex, float sInt, Vector3 angles, string fogHex, float fDens, float hInt)
        {
            label = lbl;
            ColorUtility.TryParseHtmlString(sunHex, out sunColor);
            sunIntensity = sInt;
            sunAngles = angles;
            ColorUtility.TryParseHtmlString(fogHex, out fogColor);
            fogDensity = fDens;
            headlightIntensity = hInt;
        }
    }

    private readonly TimePreset[] timePresets = new TimePreset[]
    {
        new TimePreset("☀️ نهار",  "#fff1d6", 1.5f, new Vector3(52f, 35f, 0f),  "#c4e4f4", 0.0022f, 0.0f),
        new TimePreset("🌇 غروب", "#ff9a52", 1.2f, new Vector3(15f, -60f, 0f), "#e08a5a", 0.0030f, 2.2f),
        new TimePreset("🌙 ليل",   "#8ab4f8", 0.4f, new Vector3(38f, -20f, 0f), "#0b1524", 0.0035f, 4.8f)
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();
        if (camController == null) camController = FindObjectOfType<CameraController>();

        // Start position on main street near Tosha Restaurant
        if (player != null)
        {
            player.transform.position = new Vector3(-62f, 0f, -20f);
            player.heading = 0.0f;
        }

        SpawnMissionMarkerObject();
        NextPickup();
        ApplyTimePreset(timeMode);
    }

    void Update()
    {
        if (isPaused) return;

        float dt = Time.deltaTime;
        if (tripTimer > 0f)
        {
            tripTimer = Mathf.Max(0f, tripTimer - dt);
        }
        if (slowHintCooldown > 0f)
        {
            slowHintCooldown -= dt;
        }

        // Mission interactions
        TryMissionInteraction();

        // Update UI HUD
        if (UIController.Instance != null && player != null)
        {
            string locName = currentLocIdx >= 0 ? ElMargLocations[currentLocIdx].name : "";
            string missionTxt = isDelivery ? $"🏁 وصل الركاب إلى: {locName}" : $"📍 اركب الزبون من عند: {locName}";

            float dist = 0f;
            if (missionMarker != null)
            {
                dist = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z),
                                       new Vector2(missionMarker.transform.position.x, missionMarker.transform.position.z));
            }
            string distTxt = $"📍 {locName} - المسافة: {Mathf.RoundToInt(dist)} م";

            UIController.Instance.UpdateHUD(money, passengers, player.currentSpeed, tripTimer, missionTxt, distTxt);
        }
    }

    private void SpawnMissionMarkerObject()
    {
        if (missionMarker == null)
        {
            GameObject go = new GameObject("MissionMarker");
            missionMarker = go.AddComponent<MissionMarker>();
        }
    }

    public void NextPickup()
    {
        isDelivery = false;
        currentLocIdx = PickRandomLocation(currentLocIdx);
        var loc = ElMargLocations[currentLocIdx];

        if (missionMarker != null)
        {
            missionMarker.transform.position = loc.pos;
            missionMarker.Initialize(new Color(1.0f, 0.83f, 0.31f), true); // Gold Yellow
        }
        if (passengers == 0) tripTimer = 0.0f;
    }

    public void NextDestination()
    {
        isDelivery = true;
        currentLocIdx = PickRandomLocation(currentLocIdx);
        var loc = ElMargLocations[currentLocIdx];

        tripTimer = 55.0f + passengers * 10.0f;

        if (missionMarker != null)
        {
            missionMarker.transform.position = loc.pos;
            missionMarker.Initialize(new Color(0.33f, 0.88f, 0.63f), false); // Bright Green
        }
    }

    private int PickRandomLocation(int excludeIdx)
    {
        List<int> candidates = new List<int>();
        for (int i = 0; i < ElMargLocations.Length; i++)
        {
            if (i != excludeIdx)
            {
                float d = Vector3.Distance(ElMargLocations[i].pos, player.transform.position);
                if (d > 28.0f)
                {
                    candidates.Add(i);
                }
            }
        }
        if (candidates.Count == 0) return (excludeIdx + 1) % ElMargLocations.Length;
        return candidates[Random.Range(0, candidates.Count)];
    }

    private void TryMissionInteraction()
    {
        if (missionMarker == null || player == null) return;

        float dist = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z),
                                     new Vector2(missionMarker.transform.position.x, missionMarker.transform.position.z));
        if (dist > 7.5f) return;

        if (Mathf.Abs(player.currentSpeed) > 4.5f)
        {
            if (slowHintCooldown <= 0f)
            {
                UIController.Instance.ShowToast("🛑 هدي السرعة عند العلامة علشان تقف صح!");
                slowHintCooldown = 1.8f;
            }
            return;
        }

        string placeName = currentLocIdx >= 0 ? ElMargLocations[currentLocIdx].name : "المحطة";

        if (passengers < 2 && !isDelivery)
        {
            passengers++;
            player.currentSpeed *= 0.35f;
            if (passengers == 1) tripTimer = 55.0f;

            if (player.proceduralTuktuk != null)
            {
                player.proceduralTuktuk.UpdatePassengers(passengers);
            }

            UIController.Instance.ShowToast($"🧍 ركب الزبون من {placeName} ({passengers}/2)");

            if (passengers == 2) NextDestination();
            else NextPickup();
        }
        else
        {
            // Dropoff completed
            int baseFare = passengers * (30 + (fareLevel - 1) * 12);
            int bonus = tripTimer > 0f ? (Mathf.RoundToInt(tripTimer * 0.6f) + (fareLevel - 1) * 5) : 0;
            int total = baseFare + bonus;
            money += total;

            UIController.Instance.ShowToast($"💵 وصلت لـ {placeName}! +{total}ج");

            passengers = 0;
            tripTimer = 0.0f;
            player.currentSpeed *= 0.3f;

            if (player.proceduralTuktuk != null)
            {
                player.proceduralTuktuk.UpdatePassengers(0);
            }

            NextPickup();
        }
    }

    // Upgrades
    public void UpgradeEngine()
    {
        int cost = engineLevel * 50;
        if (money >= cost && engineLevel < 4)
        {
            money -= cost;
            engineLevel++;
            UIController.Instance.ShowToast("🚀 تم تعلية الموتور!");
            UIController.Instance.UpdateShopUI();
        }
    }

    public void UpgradeBrakes()
    {
        int cost = brakeLevel * 40;
        if (money >= cost && brakeLevel < 4)
        {
            money -= cost;
            brakeLevel++;
            UIController.Instance.ShowToast("🛑 تم تطوير الفرامل والعفشة!");
            UIController.Instance.UpdateShopUI();
        }
    }

    public void UpgradeFare()
    {
        int cost = fareLevel * 60;
        if (money >= cost && fareLevel < 4)
        {
            money -= cost;
            fareLevel++;
            UIController.Instance.ShowToast("🪑 الكراسي بقت مريحة والأجرة زادت!");
            UIController.Instance.UpdateShopUI();
        }
    }

    // Controls toggles
    public void CycleTimeOfDay()
    {
        timeMode = (timeMode + 1) % timePresets.Length;
        ApplyTimePreset(timeMode);
    }

    private void ApplyTimePreset(int idx)
    {
        var p = timePresets[idx];
        if (UIController.Instance != null && UIController.Instance.btnTime != null)
        {
            UIController.Instance.btnTime.GetComponentInChildren<Text>().text = p.label;
        }

        if (sunLight != null)
        {
            sunLight.color = p.sunColor;
            sunLight.intensity = p.sunIntensity;
            sunLight.transform.rotation = Quaternion.Euler(p.sunAngles);
        }

        RenderSettings.fog = true;
        RenderSettings.fogColor = p.fogColor;
        RenderSettings.fogDensity = p.fogDensity;

        if (player != null && player.proceduralTuktuk != null && player.proceduralTuktuk.headLight != null)
        {
            player.proceduralTuktuk.headLight.intensity = p.headlightIntensity;
        }
    }

    public void CycleCamera()
    {
        if (camController != null)
        {
            camController.CycleMode();
            if (UIController.Instance != null && UIController.Instance.btnCam != null)
            {
                UIController.Instance.btnCam.GetComponentInChildren<Text>().text = camController.GetModeName();
            }
        }
    }

    public void CycleVehicle()
    {
        string[] carNames = new string[]
        {
            "توك توك المرج الشعبي الأصلي",
            "ميكروباص هايس بورسعيد",
            "هايس سوبر GL 2018",
            "هايس 2020",
            "هايس 2000",
            "تويوتا كورولا 2004",
            "بيجو 504"
        };
        currentVehicleIdx = (currentVehicleIdx + 1) % carNames.Length;
        UIController.Instance.ShowToast($"🚗 تم اختيار: {carNames[currentVehicleIdx]}");
    }
}
