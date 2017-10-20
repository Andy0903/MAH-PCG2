using UnityEngine;

[RequireComponent(typeof(TerrainCollider))]
public class DiamondSquare : MonoBehaviour
{
    TerrainData data;
    float[,] heights;

    int size;
    [SerializeField]
    float roughness = 0.8f;
    [SerializeField]
    float randomRangeStartValue = 0.5f;
    float randomRange;

    private void Awake()
    {
        data = transform.GetComponent<TerrainCollider>().terrainData;
        size = data.heightmapWidth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Algorithm();
        }
    }

    public void Initialize()
    {
        heights = new float[size, size];
        heights = new float[size, size];
        randomRange = randomRangeStartValue;

        heights[0, 0] = 1;
        heights[size - 1, 0] = 1;
        heights[0, size - 1] = 1;
        heights[size - 1, size - 1] = 1;

        data.SetHeights(0, 0, heights);
    }

    void Diamond(int sideLength)
    {
        int halfSideLength = sideLength / 2;

        for (int x = 0; x < size - 1; x += sideLength)
        {
            for (int y = 0; y < size - 1; y += sideLength)
            {
                float average = heights[x, y];
                average += heights[x + sideLength, y];
                average += heights[x, y + sideLength];
                average += heights[x + sideLength, y + sideLength];
                average /= 4.0f;

                average += (Random.value * (randomRange * 2.0f)) - randomRange;
                heights[x + halfSideLength, y + halfSideLength] = average;
            }
        }
    }

    void Square(int sideLength)
    {
        int halfSideLength = sideLength / 2;

        for (int x = 0; x < size - 1; x += halfSideLength)
        {
            for (int y = (x + halfSideLength) % sideLength; y < size - 1; y += sideLength)
            {
                float average = heights[(x - halfSideLength + size - 1) % (size - 1), y];
                average += heights[(x + halfSideLength) % (size - 1), y];
                average += heights[x, (y + halfSideLength) % (size - 1)];
                average += heights[x, (y - halfSideLength + size - 1) % (size - 1)];
                average /= 4.0f;
                
                average += (Random.value * (randomRange * 2.0f)) - randomRange;
                heights[x, y] = average;

                if (x == 0)
                {
                    heights[size - 1, y] = average;
                }

                if (y == 0)
                {
                    heights[x, size - 1] = average;
                }
            }
        }
    }

    void Algorithm()
    {
        Initialize();

        for (int sideLength = size - 1; sideLength > 1; sideLength /= 2)
        {
            Diamond(sideLength);
            Square(sideLength);
            
            randomRange -= randomRange * 0.5f * roughness;
        }

        data.SetHeights(0, 0, heights);
    }
}