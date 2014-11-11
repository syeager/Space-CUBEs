// Little Byte Games

using System.Collections.Generic;
using System.Linq;
using Annotations;
using LittleByte;
using LittleByte.Data;
using LittleByte.NGUI;
using UnityEngine;

namespace SpaceCUBEs
{
    public class GarageEntranceManager : MonoBase
    {
        #region State Fields

        private StateMachine states;
        private const string IdleState = "Idle";
        private const string RenameState = "Rename";
        private const string DeleteState = "Delete";

        #endregion

        #region Private Fields

        [Header("Builds")]
        [SerializeField, UsedImplicitly]
        private SelectableButton buildPreviewButtonPrefab;

        [SerializeField, UsedImplicitly]
        private UIScrollView loadScrollView;

        [SerializeField, UsedImplicitly]
        private UIGrid loadGrid;

        [SerializeField, UsedImplicitly]
        private BuildPreview selectedBuildPreview;

        [Header("Preview")]
        [SerializeField, UsedImplicitly]
        private GameObject preview;

        [SerializeField, UsedImplicitly]
        private Transform previewStage;

        [SerializeField, UsedImplicitly]
        private Material buildMaterial;

        private GameObject lastPreview;
        private GameObject currentPreview;

        [SerializeField, UsedImplicitly]
        private float buildTime = 0.75f;

        [SerializeField, UsedImplicitly]
        private float releaseTime = 0.75f;

        private Job disjoinJob;
        private Job joinJob;

        private readonly List<SelectableButton> buildPreviews = new List<SelectableButton>();

        [Header("Popups")]
        [SerializeField, UsedImplicitly]
        private ConfirmationPopup popupPrefab;

        [SerializeField, UsedImplicitly]
        private InputPopup renamePopup;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Awake()
        {
            NavigationBar.Show(true);

            // states
            states = new StateMachine(this, IdleState);
            states.CreateState(IdleState, info => { }, info => { });
            states.CreateState(RenameState, info => { }, info => { });
            states.CreateState(DeleteState, info => { }, info => { });
            states.Start();

            // setup
            ShowBuild.material = buildMaterial;
            CreateBuildPreviews();
        }

        #endregion

        #region Public Methods

        public void ConfirmRename()
        {
            if (renamePopup.gameObject.activeInHierarchy)
            {
                renamePopup.gameObject.SetActive(false);
                OverlayEventArgs.Fire(this, "Rename", false);
            }
            else
            {
                renamePopup.Initialize(RenameBuild, ConstructionGrid.SelectedBuild, "Rename " + ConstructionGrid.SelectedBuild + " to");
                OverlayEventArgs.Fire(this, "Rename", true);
            }
        }

        private void RenameBuild(bool saved)
        {
            OverlayEventArgs.Fire(this, "Rename", false);
            if (!saved) return;

            ConstructionGrid.RenameBuild(ConstructionGrid.SelectedBuild, renamePopup.input.value);

            SelectableButton button = buildPreviews.Single(b => b.value == ConstructionGrid.SelectedBuild);
            ConstructionGrid.SelectedBuild = renamePopup.input.value;
            button.value = ConstructionGrid.SelectedBuild;
            button.label.text = ConstructionGrid.SelectedBuild;
        }

        public void NewBuild()
        {
            // get name
            string shipName = CustomName();
            ConstructionGrid.SelectedBuild = shipName;

            // add build to build list
            ConstructionGrid.SaveBuild(shipName, new BuildInfo {name = shipName});

            // load workshop
            SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.Workshop), true, true);
        }

        public void CopyBuild()
        {
            BuildInfo info = ConstructionGrid.LoadBuild(ConstructionGrid.SelectedBuild);

            // get name
            string shipName = CustomName();
            ConstructionGrid.SelectedBuild = shipName;

            info.name = shipName;

            // add build to build list
            ConstructionGrid.SaveBuild(shipName, info);

            // load workshop
            SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.Workshop), true, true);
        }

        /// <summary>
        /// Delete selected build from data.
        /// </summary>
        public void DeleteBuild()
        {
            var deletePopup = (ConfirmationPopup)Instantiate(popupPrefab);
            deletePopup.Initialize(ConfirmDelete, "Delete " + ConstructionGrid.SelectedBuild + " build?", "Delete", true);
            OverlayEventArgs.Fire(this, "Confirm Delete", true);
        }

        public void EditBuild()
        {
            SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.Workshop), true, true);
        }

        #endregion

        #region Private Methods

        private void CreateBuildPreviews()
        {
            string[] buildNames = ConstructionGrid.BuildNames().ToArray();
            Debugger.LogList(buildNames);
            foreach (string buildName in buildNames)
            {
                BuildInfo info = ConstructionGrid.LoadBuild(buildName);
                var button = (SelectableButton)Instantiate(buildPreviewButtonPrefab);
                button.SetScrollView(buildName, buildName, buildName, loadGrid.transform, loadScrollView);
                button.GetComponent<BuildPreview>().Initialize(info);
                button.ActivateEvent += OnBuildChosen;

                buildPreviews.Add(button);

                if (buildName == ConstructionGrid.SelectedBuild)
                {
                    SelectableButton.SetSelected(button);
                    OnBuildChosen(button, new ActivateButtonArgs(buildName, true));
                }
            }
        }

        private void ConfirmDelete(bool confirmed)
        {
            OverlayEventArgs.Fire(this, "Confirm Delete", false);
            if (!confirmed) return;

            // delete build
            ConstructionGrid.DeleteBuild(ConstructionGrid.SelectedBuild);

            // remove build button
            SelectableButton button = buildPreviews.Single(b => b.value == ConstructionGrid.SelectedBuild);
            button.ActivateEvent -= OnBuildChosen;
            Destroy(button.gameObject);

            // reload grid
            StartCoroutine(Utility.UpdateScrollView(loadGrid, (UIScrollBar)loadScrollView.verticalScrollBar, loadScrollView));

            // select first build
            SelectableButton firstButton = buildPreviews[0];
            SelectableButton.SetSelected(firstButton);
            OnBuildChosen(firstButton, new ActivateButtonArgs(firstButton.value, true));
        }

        #endregion

        #region Static Methods

        private static string CustomName()
        {
            const string custom = "Custom";
            int i = 1;
            while (true)
            {
                if (!SaveData.Contains(custom + i, ConstructionGrid.BuildsFolder))
                {
                    return custom + i;
                }
                i++;
            }
        }

        #endregion

        #region Event Handlers

        private void OnBuildChosen(object sender, ActivateButtonArgs args)
        {
            if (!args.isPressed) return;

            ConstructionGrid.SelectedBuild = args.value;
            SelectableButton button = (SelectableButton)sender;
            BuildInfo buildInfo = button.GetComponent<BuildPreview>().Info;
            selectedBuildPreview.Initialize(buildInfo);

            // release
            if (currentPreview != null)
            {
                if (disjoinJob != null)
                {
                    disjoinJob.Kill();
                    Destroy(lastPreview);
                }
                lastPreview = currentPreview;
                disjoinJob = new Job(ShowBuild.Disjoin(lastPreview.transform, releaseTime, () => Destroy(lastPreview)));
            }

            // build
            currentPreview = new GameObject();
            currentPreview.transform.parent = previewStage;
            currentPreview.transform.localPosition = new Vector3(-0.5f, -0.5f, -0.5f);
            currentPreview.transform.localRotation = Quaternion.identity;

            if (joinJob != null)
            {
                joinJob.Kill();
            }
            joinJob = new Job(ShowBuild.Join(buildInfo, ConstructionGrid.BuildSize, currentPreview.transform, buildTime));
        }

        #endregion
    }
}