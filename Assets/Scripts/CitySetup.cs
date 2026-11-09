using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets up the 3D City of El Marg in Unity:
/// 1. Instantiates or sets up city.glb with correct position and rotation (-90 deg Y, pos (-208.67, 0, 212.5))
/// 2. Generates 89 3D BoxColliders for all El Marg buildings matching the city map
/// 3. Spawns 3D landmark signs with names and kiosks/shops for all 16 locations
/// 4. Spawns ground floor and world boundaries
/// </summary>
public class CitySetup : MonoBehaviour
{
    [Header("Building Colliders (El Marg Map)")]
    public static readonly Rect[] BuildingRects = new Rect[]
    {
        new Rect(105.8f, -66.6f, 33.0f, 11.7f),
        new Rect(-182.0f, 168.7f, 46.2f, 23.0f),
        new Rect(-126.2f, 146.5f, 28.1f, 44.7f),
        new Rect(-173.8f, 134.4f, 12.5f, 12.4f),
        new Rect(-158.8f, 115.0f, 12.5f, 12.4f),
        new Rect(-85.0f, 122.0f, 10.1f, 30.2f),
        new Rect(-87.7f, 169.6f, 21.6f, 21.7f),
        new Rect(-53.4f, 141.2f, 25.3f, 25.3f),
        new Rect(-55.6f, 177.6f, 53.1f, 13.8f),
        new Rect(-2.8f, 127.9f, 25.3f, 25.3f),
        new Rect(7.8f, 181.4f, 22.6f, 9.8f),
        new Rect(36.8f, 174.1f, 16.3f, 16.3f),
        new Rect(52.7f, 174.3f, 6.8f, 5.9f),
        new Rect(30.0f, 114.7f, 39.2f, 39.3f),
        new Rect(89.9f, 104.2f, 57.6f, 25.9f),
        new Rect(89.9f, 137.8f, 57.6f, 25.9f),
        new Rect(116.9f, 175.6f, 19.6f, 6.1f),
        new Rect(154.3f, 169.6f, 30.5f, 14.2f),
        new Rect(155.2f, 139.7f, 18.6f, 24.6f),
        new Rect(147.6f, 107.5f, 17.1f, 7.7f),
        new Rect(169.0f, 103.3f, 5.0f, 11.1f),
        new Rect(67.3f, 173.0f, 42.6f, 11.2f),
        new Rect(48.8f, 99.5f, 19.6f, 6.0f),
        new Rect(48.6f, 74.6f, 19.6f, 6.0f),
        new Rect(48.7f, 80.3f, 6.0f, 19.6f),
        new Rect(149.3f, 66.3f, 25.3f, 25.3f),
        new Rect(149.0f, 41.0f, 25.3f, 25.3f),
        new Rect(67.8f, -14.8f, 13.8f, 53.1f),
        new Rect(45.2f, 15.1f, 13.8f, 53.1f),
        new Rect(-123.1f, 74.1f, 25.9f, 57.6f),
        new Rect(-4.2f, 102.1f, 25.9f, 8.7f),
        new Rect(-4.6f, 78.6f, 25.8f, 8.6f),
        new Rect(-53.2f, 98.2f, 14.3f, 30.5f),
        new Rect(104.3f, 66.9f, 33.0f, 11.7f),
        new Rect(27.7f, -80.5f, 46.1f, 22.9f),
        new Rect(151.0f, -24.9f, 23.0f, 46.1f),
        new Rect(-52.9f, 69.6f, 12.5f, 12.4f),
        new Rect(-32.8f, 69.6f, 12.5f, 12.4f),
        new Rect(-176.3f, -5.0f, 39.2f, 39.1f),
        new Rect(-4.6f, 50.1f, 16.3f, 16.3f),
        new Rect(-4.6f, 34.0f, 16.3f, 16.4f),
        new Rect(-4.2f, 0.9f, 9.7f, 22.6f),
        new Rect(96.6f, 27.1f, 42.6f, 11.2f),
        new Rect(96.6f, -26.4f, 42.6f, 11.2f),
        new Rect(128.4f, -15.4f, 11.1f, 42.6f),
        new Rect(-1.9f, -107.8f, 11.6f, 33.0f),
        new Rect(129.0f, -192.4f, 46.2f, 22.9f),
        new Rect(91.3f, -192.0f, 28.1f, 44.8f),
        new Rect(154.5f, -147.6f, 12.5f, 12.5f),
        new Rect(139.5f, -128.2f, 12.5f, 12.5f),
        new Rect(68.1f, -153.0f, 10.1f, 30.2f),
        new Rect(59.3f, -192.0f, 21.6f, 21.6f),
        new Rect(21.3f, -167.3f, 25.3f, 25.3f),
        new Rect(-4.3f, -192.1f, 53.1f, 13.7f),
        new Rect(-55.7f, -52.5f, 25.4f, 25.3f),
        new Rect(-34.9f, -169.0f, 22.6f, 9.7f),
        new Rect(-127.3f, -130.7f, 39.2f, 39.2f),
        new Rect(-129.1f, -48.8f, 56.9f, 25.3f),
        new Rect(-129.1f, -74.7f, 56.9f, 25.8f),
        new Rect(-159.6f, -187.8f, 6.1f, 19.6f),
        new Rect(-182.1f, -132.5f, 30.4f, 14.3f),
        new Rect(-179.0f, -106.2f, 18.6f, 24.5f),
        new Rect(-129.7f, -154.9f, 42.7f, 11.1f),
        new Rect(-28.5f, 36.8f, 6.0f, 19.6f),
        new Rect(-53.4f, 36.9f, 6.0f, 19.6f),
        new Rect(-47.7f, 50.4f, 19.6f, 6.1f),
        new Rect(-179.0f, -69.5f, 25.3f, 25.4f),
        new Rect(-178.8f, -44.2f, 25.4f, 25.3f),
        new Rect(-125.3f, 14.5f, 53.1f, 13.8f),
        new Rect(-124.7f, 41.5f, 53.1f, 13.8f),
        new Rect(90.4f, -132.1f, 25.9f, 56.9f),
        new Rect(-51.3f, -8.6f, 25.8f, 8.6f),
        new Rect(-50.9f, 15.0f, 25.9f, 8.6f),
        new Rect(32.1f, -129.4f, 14.3f, 30.4f),
        new Rect(-125.3f, -11.6f, 33.0f, 11.7f),
        new Rect(150.0f, -100.2f, 22.9f, 46.2f),
        new Rect(-1.1f, -30.7f, 16.1f, 16.0f),
        new Rect(-7.6f, -55.5f, 16.1f, 16.1f),
        new Rect(47.2f, -16.5f, 9.7f, 22.6f),
        new Rect(-52.8f, -110.5f, 23.0f, 46.2f),
        new Rect(-152.7f, 58.4f, 11.2f, 42.6f),
        new Rect(-176.2f, 47.4f, 11.1f, 42.7f),
        new Rect(-76.4f, -188.2f, 28.1f, 44.7f),
        new Rect(-34.9f, -192.6f, 22.6f, 9.8f),
        new Rect(-36.5f, -144.4f, 22.6f, 9.8f),
        new Rect(-107.7f, -187.9f, 17.2f, 7.7f),
        new Rect(-130.1f, -182.2f, 17.1f, 7.7f),
        new Rect(-179.3f, -187.7f, 6.0f, 19.6f),
        new Rect(-182.1f, -156.7f, 18.6f, 24.6f),
    };

    public GameObject cityGlbPrefab;

    void Start()
    {
        SetupGround();
        SetupBuildingColliders();
        SetupLandmarkSigns();
        LoadCityGlb();
    }

    private void SetupGround()
    {
        // Base ground covering 520x520 meters
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "WorldGround";
        ground.transform.position = new Vector3(0, -0.25f, 0);
        ground.transform.localScale = new Vector3(520, 0.2f, 520);

        Renderer rend = ground.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.17f, 0.18f, 0.20f);
            mat.SetFloat("_Glossiness", 0.05f);
            rend.material = mat;
        }
    }

    private void SetupBuildingColliders()
    {
        GameObject parentObj = new GameObject("BuildingColliders_89");
        parentObj.transform.parent = transform;

        for (int i = 0; i < BuildingRects.Length; i++)
        {
            Rect r = BuildingRects[i];
            float centerX = r.x + r.width * 0.5f;
            float centerZ = r.y + r.height * 0.5f;
            float height = 14.0f; // Typical 3-4 story building height in El Marg

            GameObject bld = new GameObject($"Building_{i + 1}");
            bld.transform.parent = parentObj.transform;
            bld.transform.position = new Vector3(centerX, height * 0.5f, centerZ);

            BoxCollider col = bld.AddComponent<BoxCollider>();
            col.size = new Vector3(r.width, height, r.height);
            bld.layer = LayerMask.NameToLayer("Default");
        }
    }

    private void SetupLandmarkSigns()
    {
        GameObject parentSigns = new GameObject("LandmarkSigns_ElMarg");
        parentSigns.transform.parent = transform;

        var locs = GameManager.ElMargLocations;
        for (int i = 0; i < locs.Length; i++)
        {
            var loc = locs[i];
            CreateSignPost(parentSigns.transform, loc.name, loc.pos, loc.color, loc.accent, loc.isShop);
        }
    }

    private void CreateSignPost(Transform parent, string name, Vector3 pos, Color bgCol, Color accCol, bool isShop)
    {
        GameObject signRoot = new GameObject($"Sign_{name}");
        signRoot.transform.parent = parent;
        signRoot.transform.position = pos;

        float poleX = isShop ? -4.8f : 4.5f;

        // Pole
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.name = "Pole";
        pole.transform.parent = signRoot.transform;
        pole.transform.localPosition = new Vector3(poleX, 2.6f, 0);
        pole.transform.localScale = new Vector3(0.26f, 2.6f, 0.26f);
        SetMatColor(pole, new Color(0.8f, 0.83f, 0.88f), 0.7f, 0.3f);

        // Sign Backing
        GameObject backing = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backing.name = "Backing";
        backing.transform.parent = signRoot.transform;
        backing.transform.localPosition = new Vector3(poleX, 5.3f, 0);
        backing.transform.localScale = new Vector3(5.8f, 1.5f, 0.18f);
        SetMatColor(backing, accCol, 0.0f, 0.2f);

        // Sign Main Board
        GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = "Board";
        board.transform.parent = signRoot.transform;
        board.transform.localPosition = new Vector3(poleX, 5.3f, 0);
        board.transform.localScale = new Vector3(5.6f, 1.35f, 0.25f);
        SetMatColor(board, bgCol, 0.05f, 0.4f);

        // Kiosk/Shop Building
        if (isShop)
        {
            GameObject kiosk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            kiosk.name = "ShopKiosk";
            kiosk.transform.parent = signRoot.transform;
            kiosk.transform.localPosition = new Vector3(poleX - 2.6f, 1.6f, 0);
            kiosk.transform.localScale = new Vector3(4.2f, 3.2f, 3.2f);
            SetMatColor(kiosk, bgCol * 0.75f, 0.0f, 0.7f);

            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = "KioskRoof";
            roof.transform.parent = signRoot.transform;
            roof.transform.localPosition = new Vector3(poleX - 2.6f, 3.3f, 0);
            roof.transform.localScale = new Vector3(4.5f, 0.28f, 3.6f);
            SetMatColor(roof, accCol, 0.0f, 0.4f);
        }

        // 3D Text on both sides
        CreateTextMesh(signRoot.transform, name, new Vector3(poleX, 5.3f, 0.16f), 0.0f);
        CreateTextMesh(signRoot.transform, name, new Vector3(poleX, 5.3f, -0.16f), 180.0f);
    }

    private void CreateTextMesh(Transform parent, string text, Vector3 localPos, float rotY)
    {
        GameObject textObj = new GameObject("SignText");
        textObj.transform.parent = parent;
        textObj.transform.localPosition = localPos;
        textObj.transform.localRotation = Quaternion.Euler(0, rotY, 0);

        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 32;
        tm.characterSize = 0.08f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;

        MeshRenderer mr = textObj.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.material = Resources.GetBuiltinResource<Material>("Arial.mat");
        }
    }

    private void SetMatColor(GameObject go, Color col, float metallic, float roughness)
    {
        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = col;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Glossiness", 1.0f - roughness);
            rend.material = mat;
        }
    }

    private void LoadCityGlb()
    {
        if (cityGlbPrefab != null)
        {
            GameObject city = Instantiate(cityGlbPrefab, transform);
            city.name = "City_ElMarg_GLB";
            city.transform.position = new Vector3(-208.67f, 0f, 212.5f);
            city.transform.rotation = Quaternion.Euler(0, -90f, 0);
            CleanStaticRoadCars(city);
        }
    }

    private void CleanStaticRoadCars(GameObject root)
    {
        var transforms = root.GetComponentsInChildren<Transform>(true);
        foreach (var t in transforms)
        {
            if (t == null) continue;
            string nm = t.name.ToLower();
            if (nm.Contains("sedan") || nm.Contains("suv") || nm.Contains("van"))
            {
                Destroy(t.gameObject);
            }
        }
    }
}
