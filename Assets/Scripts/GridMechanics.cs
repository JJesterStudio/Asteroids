using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GridMechanics : MonoBehaviour
{
    public static GridMechanics Instance { get; private set; }

    public PersistentAsteroid persistentAsteroid;
    public bool generateAsteroidsStats;
    public GameObject toSpawn;
    public int gridSize;
    public int gridSpread;

    [HideInInspector] public Vector2 cameraBorder;
    [HideInInspector] public ObjectPool<GameObject> asteroidPool;
    [HideInInspector] public float edgePosition;
    [HideInInspector] public Unit[][] grid;
    [HideInInspector] public List<Unit> units = new List<Unit>();

    void Start()
    {
        Instance = this;

        cameraBorder = new Vector2(Camera.main.aspect, 1) * Camera.main.orthographicSize + Vector2.one;
        edgePosition = (gridSize / 2 - 1) * gridSpread + gridSpread / 2;

        if (generateAsteroidsStats || persistentAsteroid.direction == null || persistentAsteroid.direction.Length != gridSize * gridSize)
        {
            persistentAsteroid.direction = new Vector2[gridSize * gridSize];
            generateAsteroidsStats = true;
        }

        InitializeAsteroidPool();     
        InitializeGrid();
    }

    private void InitializeAsteroidPool()
    {
        asteroidPool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(toSpawn);
        }, asteroid =>
        {
            asteroid.gameObject.SetActive(true);
        }, asteroid =>
        {
            asteroid.gameObject.SetActive(false);
        }, asteroid =>
        {
            Destroy(asteroid.gameObject);
        },
        false, gridSize * gridSize, gridSize * gridSize
        );
    }

    private void InitializeGrid()
    {
        grid = new Unit[gridSize][];

        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = new Unit[gridSize];

            for (int j = 0; j < grid[i].Length; j++)
            {
                if (generateAsteroidsStats)
                {
                    persistentAsteroid.direction[i * j + j] = Random.insideUnitCircle * Random.Range(1.0f, 3.0f);       //Vector2 is enough to keep info about both direction and speed
                }
                grid[i][j] = new Unit(new Vector2(GridIndexToPosition(i), GridIndexToPosition(j)), persistentAsteroid.direction[i * j + j]);
                units.Add(grid[i][j]);
            }
        }
    }

    private void Update()
    {
        UpdateUnits();
        CheckCollisionsWithinUnits(); 
    }

    private void UpdateUnits()
    {
        var time = Time.deltaTime;                  //calling Time.deltaTime multiple times takes a lot of time
        for (int i = 0; i < gridSize * gridSize; i++)
        {
            if (units[i].active)
            {
                units[i].Update(time);
            }
            else
            {
                if (units[i].ReadyForActivation(time))
                {
                    ActivateUnit(units[i]);
                }
            }
        }
    }

    private void CheckCollisionsWithinUnits()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i][j] == null) continue;
                grid[i][j].CheckCollisions();
            }
        }
    }

    public int PositionToGridIndex(float position)
    {
        return (int)((position + edgePosition) / gridSpread);
    }

    public float GridIndexToPosition(int gridIndex)
    {
        return gridIndex * gridSpread - edgePosition;
    }

    public bool CheckCollisionOnPosition(Vector3 position, float radius)
    {
        Unit target = grid[PositionToGridIndex(position.x)][PositionToGridIndex(position.y)];
        while (target != null)
        {
            if (Vector2.Distance(position, target.position) <= radius + Unit.diameter / 2)
            {
                target.VisibilityCheck();
                target.Deactivate();
                return true;
            }
            else
            {
                target = target.nextUnit;
            }
        }
        return false;
    }

    public void ActivateUnit(Unit unit)
    {
        float x = Random.Range(-edgePosition, edgePosition);
        while (Mathf.Abs(x) < cameraBorder.x)
        {
            x = Random.Range(-edgePosition, edgePosition);
        }
        float y = Random.Range(-edgePosition, edgePosition);
        while (Mathf.Abs(y) < cameraBorder.y)
        {
            y = Random.Range(-edgePosition, edgePosition);
        }
        unit.Add(PositionToGridIndex(x), PositionToGridIndex(y));
        unit.position = new Vector2(x, y);
    }
}