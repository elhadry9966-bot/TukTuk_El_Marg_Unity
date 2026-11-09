using UnityEngine;

/// <summary>
/// Visual mission marker for pickup and dropoff points in El Marg.
/// Displays a rotating glowing cylinder base, a spinning diamond cube,
/// and an optional waiting passenger NPC with randomized clothing.
/// </summary>
public class MissionMarker : MonoBehaviour
{
    private Transform rotatingPart;
    private GameObject passengerNPC;
    private Renderer baseRenderer;
    private Renderer diamondRenderer;

    public void Initialize(Color markerColor, bool showPassenger)
    {
        // Clear previous children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        rotatingPart = new GameObject("RotatingMarker").transform;
        rotatingPart.parent = transform;
        rotatingPart.localPosition = Vector3.zero;

        // Base glowing disk
        GameObject disk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disk.name = "MarkerBase";
        disk.transform.parent = rotatingPart;
        disk.transform.localPosition = new Vector3(0, 0.1f, 0);
        disk.transform.localScale = new Vector3(3.6f, 0.1f, 3.6f);
        Destroy(disk.GetComponent<Collider>()); // No physical collision

        baseRenderer = disk.GetComponent<Renderer>();
        SetUnshadedColor(baseRenderer, markerColor);

        // Floating diamond
        GameObject diamond = GameObject.CreatePrimitive(PrimitiveType.Cube);
        diamond.name = "MarkerDiamond";
        diamond.transform.parent = rotatingPart;
        diamond.transform.localPosition = new Vector3(0, 2.8f, 0);
        diamond.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        diamond.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
        Destroy(diamond.GetComponent<Collider>());

        diamondRenderer = diamond.GetComponent<Renderer>();
        SetUnshadedColor(diamondRenderer, markerColor);

        // Waiting passenger NPC
        if (showPassenger)
        {
            passengerNPC = CreatePersonNPC(new Vector3(2.1f, 0, 0));
            passengerNPC.transform.parent = transform;
            passengerNPC.transform.localScale = Vector3.one * 0.92f;
        }
    }

    void Update()
    {
        if (rotatingPart != null)
        {
            rotatingPart.Rotate(Vector3.up, 80f * Time.deltaTime, Space.World);
        }
    }

    private void SetUnshadedColor(Renderer rend, Color col)
    {
        if (rend != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = col;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", col * 0.8f);
            mat.SetFloat("_Glossiness", 0.1f);
            rend.material = mat;
        }
    }

    private GameObject CreatePersonNPC(Vector3 localPos)
    {
        GameObject person = new GameObject("WaitingPassenger");
        person.transform.localPosition = localPos;

        Color[] shirtColors = new Color[]
        {
            new Color(0.19f, 0.36f, 0.55f), // Blue
            new Color(0.60f, 0.29f, 0.20f), // Brown
            new Color(0.16f, 0.62f, 0.56f), // Teal
            new Color(0.43f, 0.35f, 0.48f)  // Purple
        };
        Color shirtCol = shirtColors[Random.Range(0, shirtColors.Length)];
        Color skinCol = new Color(0.73f, 0.47f, 0.31f);
        Color pantsCol = new Color(0.16f, 0.15f, 0.13f);
        Color hairCol = new Color(0.11f, 0.08f, 0.07f);

        // Body
        CreatePart(person.transform, PrimitiveType.Capsule, new Vector3(0, 1.0f, 0), new Vector3(0.54f, 0.45f, 0.54f), shirtCol);
        // Head
        CreatePart(person.transform, PrimitiveType.Sphere, new Vector3(0, 1.62f, 0), Vector3.one * 0.4f, skinCol);
        // Hair
        CreatePart(person.transform, PrimitiveType.Sphere, new Vector3(0, 1.69f, -0.02f), Vector3.one * 0.41f, hairCol);

        // Arms & Legs
        foreach (float side in new float[] { -1.0f, 1.0f })
        {
            CreatePart(person.transform, PrimitiveType.Capsule, new Vector3(side * 0.37f, 0.98f, 0), new Vector3(0.16f, 0.35f, 0.16f), shirtCol);
            CreatePart(person.transform, PrimitiveType.Capsule, new Vector3(side * 0.14f, 0.36f, 0), new Vector3(0.2f, 0.35f, 0.2f), pantsCol);
        }

        return person;
    }

    private void CreatePart(Transform parent, PrimitiveType type, Vector3 pos, Vector3 scale, Color col)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.transform.parent = parent;
        part.transform.localPosition = pos;
        part.transform.localScale = scale;
        Destroy(part.GetComponent<Collider>());

        Renderer rend = part.GetComponent<Renderer>();
        if (rend != null)
        {{
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = col;
            mat.SetFloat("_Glossiness", 0.2f);
            rend.material = mat;
        }}
    }
}
