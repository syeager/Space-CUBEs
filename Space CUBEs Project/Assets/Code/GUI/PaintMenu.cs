// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.21
// Edited: 2014.09.22

using System;
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
            // cached
            grid = GarageManager.Main.grid;

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
            if (primarySelected)
            {
                colorPrimary = colorIndex;
                primaryColor.color = CUBE.Colors[colorIndex];
            }
            else
            {
                colorSecondary = colorIndex;
                secondaryColor.color = CUBE.Colors[colorIndex];
            }

            UpdateSections();
            UpdatePaintButton();
        }


        private void UpdateSections()
        {
            Color color = CUBE.Colors[primarySelected ? colorPrimary : colorSecondary];
            mainSection.color = color;
            detailSection.color = color;
        }


        private void UpdateSampleButton()
        {
            Color color = CUBE.Colors[grid.hoveredCUBE.GetComponent<ColorVertices>().GetColor(mainSelected ? 0 : 1)];
            actionButtons.buttons[1].buttons[1].defaultColor = color;
        }


        private void UpdatePaintButton()
        {
            Color color = CUBE.Colors[primarySelected ? colorPrimary : colorSecondary];
            actionButtons.buttons[1].buttons[0].defaultColor = color;
        }

        #endregion

        #region Event Handlers

        private void OnCursorStatusChanged(object sender, CursorUpdatedArgs args)
        {
            if (args.current != ConstructionGrid.CursorStatuses.Hover) return;

            UpdateSampleButton();
        }


        private void OnColorSelected(object sender, ActivateButtonArgs args)
        {
            SetColor(int.Parse(args.value));
        }


        private void OnPalleteClicked()
        {
            pallete.SetActive(!pallete.activeSelf);
        }


        private void OnPaintClicked()
        {
            grid.Paint(mainSelected ? 0 : 1, primarySelected ? colorPrimary : colorSecondary);
        }


        private void OnPaintAllClicked()
        {
            grid.PaintAll(colorPrimary, colorSecondary);
        }


        private void OnSampleClicked()
        {
            SetColor(grid.hoveredCUBE.GetComponent<ColorVertices>().GetColor(mainSelected ? 0 : 1));
            //if (primarySelected)
            //{
            //    colorPrimary = Array.IndexOf(CUBE.Colors, color);
            //}
            //else
            //{
            //    colorSecondary = Array.IndexOf(CUBE.Colors, color);
            //}

            UpdateSections();
        }

        #endregion
    }
}