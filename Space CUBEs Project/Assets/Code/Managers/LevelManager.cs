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
        player.GetComponent<ShipMotor>().speed = args.speed;
        player.GenerateCollider();
    }

    #endregion
}