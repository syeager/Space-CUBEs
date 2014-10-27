// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.21
// Edited: 2014.10.26

using System.Collections;
using Annotations;
using UnityEngine;

namespace SpaceCUBEs
{
    public class PaintMenu : MonoBehaviour
    {
        #region Private Fields

        [Header("Widgets")]
        [SerializeField, UsedImplicitly]
        private UISprite primaryColor;

        [SerializeField, UsedImplicitly]
        private UISprite secondaryColor;

        private bool primarySelected = true;

        private Job primaryColorJob;

        private Job secondaryColorJob;

        [SerializeField, UsedImplicitly]
        private UISprite mainSection;

        [SerializeField, UsedImplicitly]
        private UISprite detailSection;

        private bool mainSelected = true;

        private int colorPrimary;

        private int colorSecondary;

        private Job mainSegmentJob;

        private Job detailSegmentJob;

        [SerializeField, UsedImplicitly]
        private UISprite trimColor;

        private bool trimSelected;

        // TODO: replace with one widget
        [SerializeField, UsedImplicitly]
        private ButtonWhite paintAllMain;

        [SerializeField, UsedImplicitly]
        private ButtonWhite paintAllDetail;

        [Header("Animations")]
        [SerializeField, UsedImplicitly]
        private float deselectedScale;

        [SerializeField, UsedImplicitly]
        private float toggleTime;

        private float scaleSpeed;

        private float rotationSpeed;

        [Header("Buttons")]
        [SerializeField, UsedImplicitly]
        private GarageActionButtons actionButtons;

        [Header("Colors")]
        [SerializeField, UsedImplicitly]
        private GameObject pallete;

        private ConstructionGrid grid;

        #endregion

        #region MonoBehaviour Overrides

        [UsedImplicitly]
        private void Awake()
        {
            // cached
            grid = GarageManager.Main.grid;

            scaleSpeed = (1f - deselectedScale) / toggleTime;
            rotationSpeed = 180f / toggleTime;

            SetColor(0);
            primarySelected = false;
            SetColor(1);
            primarySelected = true;
        }


        [UsedImplicitly]
        private void Start()
        {
            // events
            grid.StatusChangedEvent += OnCursorStatusChanged;
            actionButtons.PalleteEvent += OnPalleteClicked;
            actionButtons.PaintEvent += OnPaintClicked;
            actionButtons.PaintAllEvent += OnPaintAllClicked;
            actionButtons.SampleEvent += OnSampleClicked;

            ActivateButton[] colors = pallete.GetComponentsInChildren<ActivateButton>(true);
            foreach (ActivateButton color in colors)
            {
                color.ActivateEvent += OnColorSelected;
            }

            // setup
            UpdatePaintButton();
            UpdateSampleButton();
            UpdatePaintAllButton();
            trimColor.GetComponent<ButtonWhite>().SetColor(CUBE.Colors[grid.currentTrimColor]);
        }

        #endregion

        #region Public Methods

        public void ToggleColor()
        {
            primarySelected = !primarySelected;

            if (primaryColorJob != null) primaryColorJob.Kill();
            primaryColorJob = new Job(primarySelected ? Opening(primaryColor.transform) : Closing(primaryColor.transform));

            if (secondaryColorJob != null) secondaryColorJob.Kill();
            secondaryColorJob = new Job(!primarySelected ? Opening(secondaryColor.transform) : Closing(secondaryColor.transform));

            UpdateSections();
            UpdatePaintButton();
            UpdatePaintAllButton();
        }


        public void ToggleSection()
        {
            mainSelected = !mainSelected;

            if (mainSegmentJob != null) mainSegmentJob.Kill();
            mainSegmentJob = new Job(mainSelected ? Opening(mainSection.transform) : Closing(mainSection.transform));

            if (detailSegmentJob != null) detailSegmentJob.Kill();
            detailSegmentJob = new Job(!mainSelected ? Opening(detailSection.transform) : Closing(detailSection.transform));

            UpdateSampleButton();
        }


        public void ToggleTrim()
        {
            trimSelected = !trimSelected;
            TogglePallette(!pallete.activeSelf);
        }

        #endregion

        #region Private Methods

        private IEnumerator Opening(Transform widget)
        {
            while (widget.localScale.x < 1f)
            {
                widget.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
                widget.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
                yield return null;
            }
            widget.localScale = Vector3.one;
            widget.localRotation = Quaternion.identity;
        }


        private IEnumerator Closing(Transform widget)
        {
            while (widget.localScale.x > deselectedScale)
            {
                widget.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
                widget.Rotate(Vector3.back, rotationSpeed * Time.deltaTime);
                yield return null;
            }
            widget.localScale = Vector3.one * deselectedScale;
            widget.localRotation = Quaternion.identity;
        }


        private void SetColor(int colorIndex)
        {
            if (trimSelected)
            {
                grid.currentTrimColor = colorIndex;
                trimColor.GetComponent<ButtonWhite>().SetColor(CUBE.Colors[colorIndex]);
            }
            else if (primarySelected)
            {
                colorPrimary = colorIndex;
                primaryColor.GetComponent<ButtonWhite>().SetColor(CUBE.Colors[colorIndex]);
            }
            else
            {
                colorSecondary = colorIndex;
                secondaryColor.GetComponent<ButtonWhite>().SetColor(CUBE.Colors[colorIndex]);
            }

            UpdateSections();
            UpdatePaintButton();
            UpdatePaintAllButton();
        }


        private void UpdateSections()
        {
            Color color = CUBE.Colors[primarySelected ? colorPrimary : colorSecondary];
            mainSection.GetComponent<ButtonWhite>().SetColor(color);
            detailSection.GetComponent<ButtonWhite>().SetColor(color);
        }


        private void UpdateSampleButton()
        {
            if (grid.hoveredCUBE != null)
            {
                actionButtons.buttons[1].buttons[1].isEnabled = true;
                Color color = CUBE.Colors[grid.hoveredCUBE.GetComponent<ColorVertices>().GetColor(mainSelected ? 0 : 1)];
                actionButtons.buttons[1].buttons[1].GetComponent<ButtonWhite>().SetColor(color);
            }
            else
            {
                actionButtons.buttons[1].buttons[1].isEnabled = false;
            }
        }


        private void UpdatePaintButton()
        {
            if (grid.hoveredCUBE != null)
            {
                actionButtons.buttons[1].buttons[0].isEnabled = true;
                Color color = CUBE.Colors[primarySelected ? colorPrimary : colorSecondary];
                actionButtons.buttons[1].buttons[0].GetComponent<ButtonWhite>().SetColor(color);
            }
            else
            {
                actionButtons.buttons[1].buttons[0].isEnabled = false;
            }
        }


        private void UpdatePaintAllButton()
        {
            paintAllMain.SetColor(CUBE.Colors[primarySelected ? colorPrimary : colorSecondary]);
            paintAllDetail.SetColor(CUBE.Colors[!primarySelected ? colorPrimary : colorSecondary]);
        }


        private void TogglePallette(bool open)
        {
            if (!open) trimSelected = false;
            pallete.SetActive(open);
        }

        #endregion

        #region Event Handlers

        private void OnCursorStatusChanged(object sender, CursorUpdatedArgs args)
        {
            UpdateSampleButton();
            UpdatePaintButton();
        }


        private void OnColorSelected(object sender, ActivateButtonArgs args)
        {
            SetColor(int.Parse(args.value));
            TogglePallette(false);
        }


        private void OnPalleteClicked()
        {
            TogglePallette(!pallete.activeSelf);
        }


        private void OnPaintClicked()
        {
            grid.Paint(mainSelected ? 0 : 1, primarySelected ? colorPrimary : colorSecondary);
        }


        private void OnPaintAllClicked()
        {
            grid.PaintAll(primarySelected ? colorPrimary : colorSecondary, !primarySelected ? colorPrimary : colorSecondary);
        }


        private void OnSampleClicked()
        {
            SetColor(grid.hoveredCUBE.GetComponent<ColorVertices>().GetColor(mainSelected ? 0 : 1));
            UpdateSections();
        }

        #endregion
    }
}