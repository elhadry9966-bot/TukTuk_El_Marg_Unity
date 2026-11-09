using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns and manages 6 traffic cars on El Marg street coordinates matching Godot 4 setup.
/// Can spawn 3D GLB car models or procedural fallback vehicles.
/// </summary>
public class TrafficManager : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public List<GameObject> activeCars = new List<GameObject>();

    private struct TrafficRoute
    {
        public bool alongX;
        public float dir;
        public float x;
        public float z;
        public int modelIdx;
        public TrafficRoute(bool ax, float d, float xPos, float zPos, int mIdx)
        {
            alongX = ax; dir = d; x = xPos; z = zPos; modelIdx = mIdx;
        }
    }

    private readonly TrafficRoute[] routes = new TrafficRoute[]
    {
        new TrafficRoute(false,  1.0f, -60.0f, -90.0f, 1),
        new TrafficRoute(false, -1.0f, -64.0f,  70.0f, 5),
        new TrafficRoute(false,  1.0f,  86.0f, -120.0f, 4),
        new TrafficRoute(false, -1.0f, 186.0f, -60.0f, 3),
        new TrafficRoute(true,   1.0f,  60.0f, -36.0f, 2),
        new TrafficRoute(true,  -1.0f,  40.0f,  94.0f, 4)
    };

    void Start()
    {
        SpawnAllTraffic();
    }

    public void SpawnAllTraffic()
    {
        for (int i = 0; i < routes.Length; i++)
        {
            var r = routes[i];
            GameObject car = CreateTrafficVehicle(r.modelIdx);
            car.name = $"TrafficCar_{i + 1}";
            car.transform.parent = transform;
            car.transform.position = new Vector3(r.x, 0, r.z);

            TrafficCar tc = car.AddComponent<TrafficCar>();
            tc.alongX = r.alongX;
            tc.dir = r.dir;
            tc.speed = 8.0f + Random.Range(0f, 3.5f);

            if (r.alongX)
                car.transform.rotation = Quaternion.Euler(0, r.dir > 0 ? 90f : -90f, 0);
            else
                car.transform.rotation = Quaternion.Euler(0, r.dir > 0 ? 0f : 180f, 0);

            activeCars.Add(car);
        }
    }

    private GameObject CreateTrafficVehicle(int modelIdx)
    {
        // If external car prefab exists, instantiate it
        if (carPrefabs != null && modelIdx < carPrefabs.Length && carPrefabs[modelIdx] != null)
        {
            return Instantiate(carPrefabs[modelIdx]);
        }

        // Procedural Egyptian Car fallback (Peugeot 504 / Microbus style)
        GameObject car = new GameObject("ProceduralCar");
        Color[] carColors = new Color[]
        {
            new Color(0.76f, 0.16f, 0.18f), // Red
            new Color(0.18f, 0.34f, 0.58f), // Blue
            new Color(0.85f, 0.85f, 0.88f), // White
            new Color(0.88f, 0.72f, 0.22f)  // Taxi Yellow
        };
        Color paint = carColors[Random.Range(0, carColors.Length)];

        // Body
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.transform.parent = car.transform;
        body.transform.localPosition = new Vector3(0, 0.9f, 0);
        body.transform.localScale = new Vector3(2.0f, 1.2f, 4.4f);

        Renderer r = body.GetComponent<Renderer>();
        if (r != null)
        {
            Material m = new Material(Shader.Find("Standard"));
            m.color = paint;
            m.SetFloat("_Metallic", 0.3f);
            m.SetFloat("_Glossiness", 0.6f);
            r.material = m;
        }

        // Cabin Roof
        GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.transform.parent = car.transform;
        roof.transform.localPosition = new Vector3(0, 1.6f, -0.2f);
        roof.transform.localScale = new Vector3(1.8f, 0.9f, 2.5f);
        if (roof.GetComponent<Renderer>() != null)
        {
            Material m2 = new Material(Shader.Find("Standard"));
            m2.color = Color.Lerp(paint, Color.black, 0.2f);
            roof.GetComponent<Renderer>().material = m2;
        }

        return car;
    }
}
