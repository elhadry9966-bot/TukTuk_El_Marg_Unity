using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the entire Canvas UI for TukTuk El Marg in Unity:
/// - Top Bar HUD: Money, Passengers, Speed, Timer, Current Mission, Distance
/// - Bottom Steering & Pedal Touch Buttons (Left, Right, Brake, Gas, Horn)
/// - Top Action Buttons (Workshop, Time of Day, Camera, Switch Vehicle)
/// - Toast Notifications Banner
/// - Workshop Modal (Engine, Brakes, Fare Upgrades)
/// </summary>
public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("HUD Text Labels")]
    public Text lblMoney;
    public Text lblPassengers;
    public Text lblSpeed;
    public Text lblTimer;
    public Text lblMission;
    public Text lblDist;
    public Text lblToast;

    [Header("Top Action Buttons")]
    public Button btnShop;
    public Button btnTime;
    public Button btnCam;
    public Button btnVehicle;

    [Header("Hold Buttons (Touch / Mouse)")]
    public HoldButton btnGas;
    public HoldButton btnBrake;
    public HoldButton btnLeft;
    public HoldButton btnRight;
    public Button btnHorn;

    [Header("Workshop Modal")]
    public GameObject shopPanel;
    public Text lblShopMoney;
    public Button btnBuyEngine;
    public Button btnBuyBrakes;
    public Button btnBuyFare;
    public Button btnCloseShop;

    private float toastTimer = 0.0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Hook up button listeners
        if (btnShop != null) btnShop.onClick.AddListener(OpenShop);
        if (btnCloseShop != null) btnCloseShop.onClick.AddListener(CloseShop);
        if (btnTime != null) btnTime.onClick.AddListener(() => GameManager.Instance.CycleTimeOfDay());
        if (btnCam != null) btnCam.onClick.AddListener(() => GameManager.Instance.CycleCamera());
        if (btnVehicle != null) btnVehicle.onClick.AddListener(() => GameManager.Instance.CycleVehicle());

        if (btnBuyEngine != null) btnBuyEngine.onClick.AddListener(() => GameManager.Instance.UpgradeEngine());
        if (btnBuyBrakes != null) btnBuyBrakes.onClick.AddListener(() => GameManager.Instance.UpgradeBrakes());
        if (btnBuyFare != null) btnBuyFare.onClick.AddListener(() => GameManager.Instance.UpgradeFare());

        if (btnHorn != null)
        {
            btnHorn.onClick.AddListener(() =>
            {
                if (GameManager.Instance != null && GameManager.Instance.player != null)
                {
                    GameManager.Instance.player.HonkHorn();
                }
            });
        }

        if (shopPanel != null) shopPanel.SetActive(false);
    }

    void Update()
    {
        if (toastTimer > 0f)
        {
            toastTimer -= Time.deltaTime;
            if (toastTimer <= 0f && lblToast != null)
            {
                lblToast.text = "";
            }
        }
    }

    public void UpdateHUD(int money, int passengers, float speed, float timer, string mission, string dist)
    {
        if (lblMoney != null) lblMoney.text = $"💵 الأجرة: {money} ج";
        if (lblPassengers != null) lblPassengers.text = $"👥 الركاب: {passengers}/2";
        if (lblSpeed != null) lblSpeed.text = $"⚡ السرعة: {Mathf.RoundToInt(Mathf.Abs(speed) * 8.0f)}";
        if (lblTimer != null) lblTimer.text = timer > 0f ? $"⏱️ الوقت: {Mathf.CeilToInt(timer)}ث" : "⏱️ الوقت: --";
        if (lblMission != null) lblMission.text = mission;
        if (lblDist != null) lblDist.text = dist;
    }

    public void ShowToast(string message)
    {
        if (lblToast != null)
        {
            lblToast.text = message;
            toastTimer = 2.4f;
        }
    }

    public void OpenShop()
    {
        if (GameManager.Instance != null) GameManager.Instance.isPaused = true;
        if (shopPanel != null) shopPanel.SetActive(true);
        UpdateShopUI();
    }

    public void CloseShop()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.isPaused = false;
    }

    public void UpdateShopUI()
    {
        if (GameManager.Instance == null) return;

        int money = GameManager.Instance.money;
        int eLvl = GameManager.Instance.engineLevel;
        int bLvl = GameManager.Instance.brakeLevel;
        int fLvl = GameManager.Instance.fareLevel;

        if (lblShopMoney != null) lblShopMoney.text = $"رصيدك الحالي: {money} جنيه";

        int ec = eLvl * 50;
        if (btnBuyEngine != null)
        {
            btnBuyEngine.GetComponentInChildren<Text>().text = $"🚀 تعلية الموتور ({eLvl}/4) - {(eLvl >= 4 ? "مكتمل ✅" : $"{ec} ج")}";
            btnBuyEngine.interactable = eLvl < 4 && money >= ec;
        }

        int bc = bLvl * 40;
        if (btnBuyBrakes != null)
        {
            btnBuyBrakes.GetComponentInChildren<Text>().text = $"🛑 فرامل وعفشة ({bLvl}/4) - {(bLvl >= 4 ? "مكتمل ✅" : $"{bc} ج")}";
            btnBuyBrakes.interactable = bLvl < 4 && money >= bc;
        }

        int fc = fLvl * 60;
        if (btnBuyFare != null)
        {
            btnBuyFare.GetComponentInChildren<Text>().text = $"🪑 كراسي مريحة ({fLvl}/4) - {(fLvl >= 4 ? "مكتمل ✅" : $"{fc} ج")}";
            btnBuyFare.interactable = fLvl < 4 && money >= fc;
        }
    }

    public bool IsGasPressed() => btnGas != null && btnGas.isPressed;
    public bool IsBrakePressed() => btnBrake != null && btnBrake.isPressed;
    public bool IsLeftPressed() => btnLeft != null && btnLeft.isPressed;
    public bool IsRightPressed() => btnRight != null && btnRight.isPressed;
}
