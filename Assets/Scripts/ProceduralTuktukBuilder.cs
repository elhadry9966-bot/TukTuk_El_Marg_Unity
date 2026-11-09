using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Procedurally constructs the authentic Egyptian Tuk-Tuk 3D model
/// with driver NPC, yellow/green bodywork, cabin roof, windshield,
/// front steering wheel pivot, rear wheels, headlights, and taillights.
/// </summary>
public class ProceduralTuktukBuilder : MonoBehaviour
{
    public Transform chassisGroup;
    public Transform frontWheelPivot;
    public List<Transform> wheels = new List<Transform>();
    public List<Renderer> tailLightRenderers = new List<Renderer>();
    public Light headLight;
    public Transform passengerSlot1;
    public Transform passengerSlot2;
    private List<GameObject> activePassengers = new List<GameObject>();

    public void BuildTuktuk()
    {
        // 1. Root chassis
        GameObject chassisGo = new GameObject("ChassisGroup");
        chassisGo.transform.parent = transform;
        chassisGo.transform.localPosition = Vector3.zero;
        chassisGo.transform.localRotation = Quaternion.identity;
        chassisGroup = chassisGo.transform;

        // Colors
        Color bodyGreen = new Color(0.09f, 0.47f, 0.30f);
        Color roofYellow = new Color(0.95f, 0.78f, 0.26f);
        Color blackMat = new Color(0.11f, 0.12f, 0.13f);
        Color chromeMat = new Color(0.89f, 0.91f, 0.94f);
        Color goldMat = new Color(1.0f, 0.82f, 0.4f);
        Color glassCol = new Color(0.6f, 0.8f, 0.82f, 0.55f);

        // Lower body
        CreateBox(chassisGroup, new Vector3(2.14f, 0.6f, 2.24f), new Vector3(0, 0.92f, -0.35f), bodyGreen, 0.25f, 0.35f);
        // Chrome bumper
        CreateBox(chassisGroup, new Vector3(2.22f, 0.11f, 0.11f), new Vector3(0, 0.66f, -1.52f), chromeMat, 0.85f, 0.2f);
        // Front hood
        CreateBox(chassisGroup, new Vector3(1.25f, 0.75f, 1.28f), new Vector3(0, 0.88f, 1.05f), bodyGreen, 0.25f, 0.35f);
        // Cabin middle
        CreateBox(chassisGroup, new Vector3(2.04f, 1.22f, 1.74f), new Vector3(0, 1.82f, -0.48f), roofYellow, 0.15f, 0.45f);
        // Roof
        CreateBox(chassisGroup, new Vector3(2.28f, 0.2f, 2.4f), new Vector3(0, 2.52f, -0.25f), bodyGreen, 0.25f, 0.35f);
        CreateBox(chassisGroup, new Vector3(2.32f, 0.09f, 2.44f), new Vector3(0, 2.42f, -0.25f), goldMat, 0.2f, 0.35f);
        // Windshield Glass
        CreateBox(chassisGroup, new Vector3(1.68f, 0.84f, 0.06f), new Vector3(0, 1.92f, 1.28f), glassCol, 0.5f, 0.15f, new Vector3(-7f, 0, 0));
        // Seats
        CreateBox(chassisGroup, new Vector3(1.62f, 0.25f, 0.7f), new Vector3(0, 1.26f, -0.52f), blackMat, 0.1f, 0.75f);
        CreateBox(chassisGroup, new Vector3(0.65f, 0.22f, 0.52f), new Vector3(0, 1.24f, 0.45f), blackMat, 0.1f, 0.75f);

        // Driver NPC
        GameObject driver = CreateDriverNPC(new Vector3(0, 1.12f, 0.45f));
        driver.transform.parent = chassisGroup;
        driver.transform.localScale = Vector3.one * 0.58f;

        // Front Headlight Box & SpotLight
        CreateBox(chassisGroup, new Vector3(0.38f, 0.38f, 0.14f), new Vector3(0, 1.08f, 1.72f), new Color(1f, 0.96f, 0.72f), 0.0f, 0.1f, Vector3.zero, true);
        GameObject lightObj = new GameObject("TuktukHeadLight");
        lightObj.transform.parent = chassisGroup;
        lightObj.transform.localPosition = new Vector3(0, 1.25f, 1.75f);
        lightObj.transform.localRotation = Quaternion.Euler(8f, 0f, 0f);
        headLight = lightObj.AddComponent<Light>();
        headLight.type = LightType.Spot;
        headLight.color = new Color(1.0f, 0.91f, 0.62f);
        headLight.range = 36.0f;
        headLight.spotAngle = 38.0f;
        headLight.intensity = 0.0f; // Adjusted by Day/Night

        // Rear Taillights
        foreach (float side in new float[] { -0.78f, 0.78f })
        {
            GameObject tl = CreateBox(chassisGroup, new Vector3(0.28f, 0.17f, 0.08f), new Vector3(side, 0.98f, -1.48f), new Color(0.73f, 0.11f, 0.11f), 0.0f, 0.2f, Vector3.zero, true);
            tailLightRenderers.Add(tl.GetComponent<Renderer>());
        }

        // Front Wheel Pivot
        GameObject fPivot = new GameObject("FrontWheelPivot");
        fPivot.transform.parent = transform;
        fPivot.transform.localPosition = new Vector3(0, 0.46f, 1.28f);
        frontWheelPivot = fPivot.transform;

        // Front Wheel
        GameObject fw = CreateWheel(frontWheelPivot, Vector3.zero, 0.42f, 0.24f);
        CreateBox(frontWheelPivot, new Vector3(0.38f, 0.22f, 0.64f), new Vector3(0, 0.34f, 0), bodyGreen, 0.25f, 0.35f);
        wheels.Add(fw.transform);

        // Rear Wheels
        foreach (float xPos in new float[] { -1.08f, 1.08f })
        {
            GameObject rw = CreateWheel(transform, new Vector3(xPos, 0.46f, -0.95f), 0.44f, 0.26f);
            wheels.Add(rw.transform);
        }

        // Passenger slots
        GameObject ps1 = new GameObject("PassengerSlot1");
        ps1.transform.parent = chassisGroup;
        ps1.transform.localPosition = new Vector3(-0.46f, 1.15f, -0.55f);
        ps1.transform.localRotation = Quaternion.Euler(0, 180f, 0);
        passengerSlot1 = ps1.transform;

        GameObject ps2 = new GameObject("PassengerSlot2");
        ps2.transform.parent = chassisGroup;
        ps2.transform.localPosition = new Vector3(0.46f, 1.15f, -0.55f);
        ps2.transform.localRotation = Quaternion.Euler(0, 180f, 0);
        passengerSlot2 = ps2.transform;
    }

    public void UpdatePassengers(int count)
    {
        foreach (var p in activePassengers)
        {
            Destroy(p);
        }
        activePassengers.Clear();

        if (count >= 1 && passengerSlot1 != null)
        {
            GameObject p1 = CreatePassengerVisual(new Color(0.19f, 0.36f, 0.55f));
            p1.transform.parent = passengerSlot1;
            p1.transform.localPosition = Vector3.zero;
            p1.transform.localRotation = Quaternion.identity;
            p1.transform.localScale = Vector3.one * 0.56f;
            activePassengers.Add(p1);
        }
        if (count >= 2 && passengerSlot2 != null)
        {
            GameObject p2 = CreatePassengerVisual(new Color(0.60f, 0.29f, 0.20f));
            p2.transform.parent = passengerSlot2;
            p2.transform.localPosition = Vector3.zero;
            p2.transform.localRotation = Quaternion.identity;
            p2.transform.localScale = Vector3.one * 0.56f;
            activePassengers.Add(p2);
        }
    }

    public void SetBraking(bool braking)
    {
        Color brakeCol = braking ? new Color(1.0f, 0.16f, 0.12f) : new Color(0.53f, 0.07f, 0.07f);
        foreach (var rend in tailLightRenderers)
        {
            if (rend != null)
            {
                rend.material.color = brakeCol;
                if (braking)
                {
                    rend.material.EnableKeyword("_EMISSION");
                    rend.material.SetColor("_EmissionColor", brakeCol * 1.5f);
                }
                else
                {
                    rend.material.DisableKeyword("_EMISSION");
                }
            }
        }
    }

    private GameObject CreateWheel(Transform parent, Vector3 localPos, float radius, float width)
    {
        GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheel.name = "Wheel";
        wheel.transform.parent = parent;
        wheel.transform.localPosition = localPos;
        wheel.transform.localScale = new Vector3(radius * 2f, width * 0.5f, radius * 2f);
        wheel.transform.localRotation = Quaternion.Euler(0, 0, 90f);
        Destroy(wheel.GetComponent<Collider>());

        Renderer rend = wheel.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.11f, 0.12f, 0.13f);
            mat.SetFloat("_Glossiness", 0.25f);
            rend.material = mat;
        }
        return wheel;
    }

    private GameObject CreateBox(Transform parent, Vector3 size, Vector3 pos, Color col, float metallic, float roughness, Vector3 rot = default, bool unshaded = false)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.transform.parent = parent;
        box.transform.localPosition = pos;
        box.transform.localScale = size;
        box.transform.localRotation = Quaternion.Euler(rot);
        Destroy(box.GetComponent<Collider>());

        Renderer rend = box.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = col;
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Glossiness", 1.0f - roughness);
            if (unshaded)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", col);
            }
            rend.material = mat;
        }
        return box;
    }

    private GameObject CreateDriverNPC(Vector3 pos)
    {
        GameObject person = new GameObject("TuktukDriver");
        person.transform.localPosition = pos;

        Color clothes = new Color(0.15f, 0.27f, 0.33f);
        Color skin = new Color(0.73f, 0.47f, 0.31f);
        Color hair = new Color(0.11f, 0.08f, 0.07f);

        CreatePart(person.transform, PrimitiveType.Capsule, new Vector3(0, 1.0f, 0), new Vector3(0.54f, 0.45f, 0.54f), clothes);
        CreatePart(person.transform, PrimitiveType.Sphere, new Vector3(0, 1.62f, 0), Vector3.one * 0.4f, skin);
        CreatePart(person.transform, PrimitiveType.Sphere, new Vector3(0, 1.69f, -0.02f), Vector3.one * 0.41f, hair);

        return person;
    }

    private GameObject CreatePassengerVisual(Color shirtCol)
    {
        GameObject person = new GameObject("PassengerVisual");
        Color skin = new Color(0.73f, 0.47f, 0.31f);
        CreatePart(person.transform, PrimitiveType.Capsule, new Vector3(0, 1.0f, 0), new Vector3(0.54f, 0.45f, 0.54f), shirtCol);
        CreatePart(person.transform, PrimitiveType.Sphere, new Vector3(0, 1.62f, 0), Vector3.one * 0.4f, skin);
        return person;
    }

    private void CreatePart(Transform parent, PrimitiveType type, Vector3 pos, Vector3 scale, Color col)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.transform.parent = parent;
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        Destroy(go.GetComponent<Collider>());

        Renderer rend = go.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = col;
            rend.material = mat;
        }
    }
}
