// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.16
// Edited: 2014.09.17

using System.Collections.ObjectModel;
using System.Linq;
using Annotations;
using LittleByte.Data;
using System.Collections.Generic;
using LittleByte.NGUI;
using UnityEngine;
using LittleByte;

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
        
        [Header("Rename")]
        [SerializeField, UsedImplicitly]
        private GameObject renamePanel;

        [SerializeField, UsedImplicitly]
        private UIInput renameInput;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Awake()
        {
            NavigationBar.Main.gameObject.SetActive(true);

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

        #region State Methods

        private void LoadExit(Dictionary<string, object> info)
        {
            //CreateGrid();
            //Grid.CreateBuild(ConstructionGrid.selectedBuild);
            //shipName.value = Grid.buildName;
            //corePointsLabel.text = Grid.corePointsAvailable.ToString();
        }

        #endregion

        #region Public Methods

        public void ConfirmRename()
        {
            renamePanel.SetActive(true);
            renameInput.value = ConstructionGrid.SelectedBuild;
        }


        public void RenameBuild()
        {
            ConstructionGrid.RenameBuild(ConstructionGrid.SelectedBuild, renameInput.value);
            renamePanel.SetActive(false);

            ScrollviewButton button = loadGrid.GetComponentsInChildren<ScrollviewButton>().First(b => b.value == ConstructionGrid.SelectedBuild);
            ConstructionGrid.SelectedBuild = renameInput.value;
            button.value = ConstructionGrid.SelectedBuild;
            button.label.text = ConstructionGrid.SelectedBuild;
        }


        public void CancelRename()
        {
            renamePanel.SetActive(false);
        }


        public void NewBuild()
        {
            // get name
            string shipName = CustomName();
            ConstructionGrid.SelectedBuild = shipName;

            // add build to build list
            ConstructionGrid.SaveBuild(shipName, new BuildInfo(shipName, new ShipStats(), new Collection<KeyValuePair<CUBE, CUBEGridInfo>>()));

            // load workshop
            SceneManager.LoadScene(Scenes.Scene(Scenes.Menus.Workshop), true, true);
        }


        public void CopyBuild()
        {
            // TODO
        }


        /// <summary>
        /// Delete selected build from data.
        /// </summary>
        public void DeleteBuild()
        {
            // delete build
            ConstructionGrid.DeleteBuild(ConstructionGrid.SelectedBuild);

            // remove build button
            var button = buildPreviews.Single(b => b.value == ConstructionGrid.SelectedBuild);
            button.ActivateEvent -= OnBuildChosen;
            Destroy(button.gameObject);

            // reload grid
            StartCoroutine(Utility.UpdateScrollView(loadGrid, (UIScrollBar)loadScrollView.verticalScrollBar, loadScrollView));

            // select first build
//            ConstructionGrid.SelectedBuild = ConstructionGrid.BuildNames()[0];
            button = buildPreviews[0];
            SelectableButton.SetSelected(button);
            OnBuildChosen(button, new ActivateButtonArgs(button.value, false));
            button.Activate(true);
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
                    OnBuildChosen(button, new ActivateButtonArgs(buildName, false));
                }
            }
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
            if (args.isPressed) return;

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