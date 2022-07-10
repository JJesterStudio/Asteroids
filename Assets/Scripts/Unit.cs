using UnityEngine;

public class Unit 
{
    private const float timeToRespawn = 1f;
    public const float diameter = 1f; 

    private Vector2 direction;
    private bool visible;
    private Transform transform;
    private GameObject actualAsteroid;
    private int x;
    private int y;
    private float timer;
    private float edgeOfTheMap;
    private GridMechanics gridRef;

    public Vector2 position;
    public static Vector2 extraMovement;
    public bool active;

    public Unit nextUnit;
    public Unit prevUnit;

    public Unit(Vector2 position, Vector2 direction)
    {
        this.direction = direction;
        this.position = position;
        visible = false;
        active = true;
        gridRef = GridMechanics.Instance;
        edgeOfTheMap = gridRef.edgePosition;
        x = gridRef.PositionToGridIndex(position.x);
        y = gridRef.PositionToGridIndex(position.y);
    }

    public bool ReadyForActivation(float deltaTime)
    {
        timer += deltaTime;
        if (timer >= timeToRespawn)
        {
            active = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Update(float time)
    {
        VisibilityCheck();
        Movement(time);
        UpdatePositionOnGrid();
    }

    public void VisibilityCheck()
    {
        if (position.x < gridRef.cameraBorder.x && position.x > -gridRef.cameraBorder.x 
            && position.y < gridRef.cameraBorder.y && position.y > -gridRef.cameraBorder.y)
        {
            if (!visible) Appear();
        }
        else
        {
            if (visible) Disappear();
        }
    }

    private void Movement(float time)
    {
        position.x += (direction.x + extraMovement.x) * time;       //this is much faster than position += (direction + extraMovement)
        position.y += (direction.y + extraMovement.y) * time;

        CheckBorders();

        if (visible)
        {
            transform.position = position;
        }
    }

    private void CheckBorders()
    {
        if (position.x > edgeOfTheMap)
        {
            position.x -= 2 * edgeOfTheMap;
        }
        else if (position.x < -edgeOfTheMap)
        {
            position.x += 2 * edgeOfTheMap;
        }
        if (position.y > edgeOfTheMap)
        {
            position.y -= 2 * edgeOfTheMap;
        }
        else if (position.y < -edgeOfTheMap)
        {
            position.y += 2 * edgeOfTheMap;
        }
    }

    private void UpdatePositionOnGrid()
    {
        var tempX = gridRef.PositionToGridIndex(position.x);
        var tempY = gridRef.PositionToGridIndex(position.y);
        if (tempX == x && tempY == y) return;
        Unlink();

        Add(tempX, tempY);
    }

    public void Add(int x, int y)
    {
        active = true;
        this.x = x;
        this.y = y;
        prevUnit = null;
        nextUnit = gridRef.grid[x][y];
        gridRef.grid[x][y] = this;
        if (nextUnit != null)
        {
            nextUnit.prevUnit = this;
        }
    }

    public void CheckCollisions()
    {
        Unit target = nextUnit;
        while (target != null)
        {
            if(Vector2.Distance(position, target.position) <= diameter)
            {
                Deactivate();
                target.VisibilityCheck();
                target.Deactivate();
                break;
            }
            else
            {
                target = target.nextUnit;
            }
        }
        if (nextUnit != null)
        {
            nextUnit.CheckCollisions();
        }
    }

    public void Deactivate()
    {
        active = false;
        Unlink();
        if (visible)
        {
            Disappear();
        }
        timer = 0f;
    }

    private void Unlink()
    {
        if (prevUnit != null)
        {
            prevUnit.nextUnit = nextUnit;
        }
        if(nextUnit != null)
        {
            nextUnit.prevUnit = prevUnit;
        }
        if (gridRef.grid[x][y] == this)
        {
            gridRef.grid[x][y] = nextUnit;
        }
    }

    public void Appear()
    {
        actualAsteroid = gridRef.asteroidPool.Get();
        transform = actualAsteroid.transform;
        transform.position = position;
        visible = true;
    }
    public void Disappear()
    {
        gridRef.asteroidPool.Release(actualAsteroid);
        visible = false;
    }
}
