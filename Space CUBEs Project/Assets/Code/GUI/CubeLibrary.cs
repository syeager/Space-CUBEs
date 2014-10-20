// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.19
// Edited: 2014.10.19

using System;
using System.Collections.Generic;
using System.Linq;
using Annotations;
using LittleByte.NGUI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace SpaceCUBEs
{
    public class CubeLibrary : MonoBehaviour
    {
        #region Private Fields

        [SerializeField, UsedImplicitly]
        private StoreItem itemPrefab;

        [SerializeField, UsedImplicitly]
        private GameObject[] itemPages;

        [SerializeField, UsedImplicitly]
        private UIGrid[] itemGrids;

        [SerializeField, UsedImplicitly]
        private UILabel filterLabel;

        [SerializeField, UsedImplicitly]
        private UIButton filterLeft;

        [SerializeField, UsedImplicitly]
        private UIButton filterRight;

        private int filter;

        #endregion

        #region Const Fields

        private static readonly string[] Items =
        {
            "System",
            "Hull",
            "Weapon",
            "Aug",
            "Upgrade",
        };

        #endregion

        #region Properties

        public bool IsActive
        {
            get { return gameObject.activeInHierarchy; }
        }

        #endregion

        #region Events

        public EventHandler<ItemSelectedArgs> ItemSelectedEvent;

        #endregion

        #region MonoBehaviour Overrides

#if UNITY_EDITOR
        [MenuItem("CONTEXT/CubeLibrary/Create Buttons")]
        [UsedImplicitly]
        private static void CreateButtons(MenuCommand command)
        {
            CubeLibrary library = (CubeLibrary)command.context;
            EditorUtility.SetDirty(library);

            library.Clear();

            CUBEInfo[] allInfo = CUBE.LoadAllCUBEInfo();

            // CUBEs
            library.CreateCUBEItems(0, allInfo.Where(c => c.type == CUBE.Types.System), true);
            library.CreateCUBEItems(1, allInfo.Where(c => c.type == CUBE.Types.Hull));
            library.CreateCUBEItems(2, allInfo.Where(c => c.type == CUBE.Types.Weapon));
            library.CreateCUBEItems(3, allInfo.Where(c => c.type == CUBE.Types.Augmentation));

            // upgrades
            library.CreateUpgradeItems();
        }
#endif

        #endregion

        #region Editor Methods

#if UNITY_EDITOR
        private void CreateCUBEItems(int index, IEnumerable<CUBEInfo> cubeInfo, bool enabled = false)
        {
            itemGrids[index].gameObject.SetActive(true);
            foreach (CUBEInfo info in cubeInfo)
            {
                CreateButton(itemGrids[index], info.name, StoreManager.ItemTypes.CUBE, info.ID, info.price, info.cost);
            }
            itemGrids[index].Reposition();
            itemGrids[index].gameObject.SetActive(enabled);
        }


        private void CreateUpgradeItems()
        {
            // core
            const string coreName = "Core ";
            for (int i = 0; i < BuildStats.CoreCapacities.Length; i++)
            {
                CreateButton(itemGrids[4], coreName + (i + 1), StoreManager.ItemTypes.Core, i, BuildStats.CorePrices[i], 0);
            }

            // weapon exp
            const string weaponExpName = "Weapon Exp ";
            for (int i = 0; i < BuildStats.WeaponExpansions.Length; i++)
            {
                CreateButton(itemGrids[4], weaponExpName + (i + 1), StoreManager.ItemTypes.Weapon, i, BuildStats.WeaponPrices[i], 0);
            }

            // aug exp
            const string augExpName = "Aug Exp ";
            for (int i = 0; i < BuildStats.AugmentationExpansions.Length; i++)
            {
                CreateButton(itemGrids[4], augExpName + (i + 1), StoreManager.ItemTypes.Augmentation, i, BuildStats.AugmentationPrices[i], 0);
            }

            itemGrids[4].gameObject.SetActive(false);
        }


        private void CreateButton(UIGrid parent, string name, StoreManager.ItemTypes itemType, int id, int price, int cp)
        {
            StoreItem button = PrefabUtility.InstantiatePrefab(itemPrefab) as StoreItem;
            parent.AddChild(button.transform);
            button.transform.localScale = Vector3.one;
            button.Initialize(itemType, id, name, price, cp, this);
            button.GetComponent<SelectableButton>().group = "Store Item";
        }


        private void Clear()
        {
            EditorUtility.SetDirty(this);

            foreach (UIGrid itemGrid in itemGrids)
            {
                itemGrid.gameObject.SetActive(true);
                Transform grid = itemGrid.transform;
                int childCount = grid.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    DestroyImmediate(grid.GetChild(0).gameObject);
                }
            }
        }
#endif

        #endregion

        #region Button Methods

        public void ItemSelected(StoreItem item, StoreManager.ItemTypes itemType, int id)
        {
            ItemSelectedEvent(item, new ItemSelectedArgs(itemType, id));
        }


        public void Filter(UIButton button)
        {
            itemPages[filter].SetActive(false);

            filter = Mathf.Clamp(filter + (button == filterLeft ? -1 : 1), 0, itemPages.Length - 1);

            itemPages[filter].SetActive(true);
            filterLabel.text = Items[filter];
            filterLeft.isEnabled = filter != 0;
            filterRight.isEnabled = filter != Items.Length - 1;
        }

        #endregion

        #region Public Methods

        public void Activate(bool on)
        {
            gameObject.SetActive(on);
            OverlayEventArgs.Fire(this, "CUBE Library", on);
        }

        #endregion
    }


    public class ItemSelectedArgs : EventArgs
    {
        public readonly StoreManager.ItemTypes itemType;
        public readonly int id;


        public ItemSelectedArgs(StoreManager.ItemTypes itemType, int id)
        {
            this.itemType = itemType;
            this.id = id;
        }
    }
}