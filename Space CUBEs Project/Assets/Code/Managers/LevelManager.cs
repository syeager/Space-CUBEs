// Steve Yeager
// 12.3.2013

using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region References

    public ConstructionGrid Grid;

    #endregion

    #region Private Fields

    private string build;

    #endregion


    #region Unity Overrides

    private void Start()
    {
        build = (string)GameData.Main.levelData;
        CreatePlayer(build);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameData.Main.LoadScene("Garage");
        }

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).tapCount == 2)
            {
                GameData.Main.ReloadScene(build);
            }
            else if (Input.GetTouch(0).tapCount == 1)
            {
                Debugger.LogConsoleLine("2 taps to reload.");
            }
        }
    }

    #endregion

    #region Private Methods

    private void CreatePlayer(string build)
    {
        Grid.BuildFinishedEvent += OnBuildFinished;
        StartCoroutine(Grid.Build(build, 10, new Vector3(-35f, 0, 0), new Vector3(0f, 90f, 270f), 2f));
    }

    #endregion

    #region Event Handlers

    private void OnBuildFinished(object sender, BuildFinishedArgs args)
    {
        Grid.BuildFinishedEvent -= OnBuildFinished;
        var player = args.ship.AddComponent<Player>();
        player.GetComponent<WeaponManager>().weapons = args.weapons;
        player.GetComponent<ShieldHealth>().Initialize(args.health, args.shield);
        player.GetComponent<ShipMotor>().horizontalSpeed = args.speed;
    }

    #endregion
}