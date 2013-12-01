// Steve Yeager
// 11.26.2013

using UnityEngine;

public class GarageManager : MonoBehaviour
{
    #region References

    public static GarageManager Main;
    public ConstructionGrid Grid;
    public GameObject mainCamera;

    #endregion

    #region Public Fields

    public int gridSize;

    #endregion


    #region Unity Overrides

    private void Awake()
    {
        Main = this;

        Grid.CreateGrid(gridSize);
    }


    private void OnGUI()
    {
        #region Move/Rotate
        
        // layer
        GUILayout.Label("Layer: " + (Grid.currentLayer+1));

        // rotate Y
        GUILayout.Label("RotateCursor Y");
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("← Y"))
            {
                Grid.RotateCursor(Vector3.down);
            }
            if (GUILayout.Button("Y →"))
            {
                Grid.RotateCursor(Vector3.up);
            }
        }
        GUILayout.EndHorizontal();
        // rotate X
        GUILayout.Label("RotateCursor Z");
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("← Z"))
            {
                Grid.RotateCursor(Vector3.back);
            }
            if (GUILayout.Button("Z →"))
            {
                Grid.RotateCursor(Vector3.forward);
            }
        }
        GUILayout.EndHorizontal();

        #endregion

        #region CUBEs

        GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, Screen.height));
        {
            int cursor = 0;
            foreach (var cube in GameResources.Main.CUBEs)
            {
                if (GUILayout.Button(cube.name.Substring(5)))
                {
                    Grid.CreateCUBE(cursor);
                }
                cursor++;
            }
        }
        GUILayout.EndArea();

        #endregion
    }


    private void Update()
    {
        // move CUBE
        if (Input.GetKeyDown(KeyCode.A))
        {
            Grid.MoveCursor(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Grid.MoveCursor(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Grid.MoveCursor(Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Grid.MoveCursor(Vector3.back);
        }

        // rotate CUBE
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Grid.RotateCursor(Vector3.up);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Grid.RotateCursor(Vector3.down);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Grid.RotateCursor(Vector3.forward);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Grid.RotateCursor(Vector3.back);
            }
            
        }
        else
        {
            // change layer
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Grid.ChangeLayer(1);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Grid.ChangeLayer(-1);
            }
        }

        // place/pickup
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Grid.CursorAction();
        }

        // delete
        if (Input.GetKeyUp(KeyCode.Delete))
        {
            if (!Grid.DeleteCUBE())
            {
                Debug.LogWarning("Can't delete CUBE.");
            }
        }
    }

    #endregion
}