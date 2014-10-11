// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.03
// Edited: 2014.09.04

#define IMPORT

using System.Collections.Generic;
using System.Linq;
using LittleByte.Extensions;
using UnityEditor;

namespace LittleByte.ImportSettings
{
    /// <summary>
    /// 
    /// </summary>
    public class FolderAssetProcessor : AssetPostprocessor
    {
        #region AssetPostprocessor Overrides

#if IMPORT
        [Annotations.UsedImplicitly]
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string importedAsset in importedAssets)
            {
                AssetImporter assetImporter = AssetImporter.GetAtPath(importedAsset);
                ImportSettings importSettings = FindImportSettings(assetImporter);
                if (importSettings != null)
                {
                    importSettings.Apply(assetImporter);
                }
            }
        }
#endif

        #endregion

        #region Static Methods

        private static ImportSettings FindImportSettings(AssetImporter assetImporter)
        {
            int[] dashIndices = assetImporter.assetPath.IndexesWhere(c => c == '/').ToArray();
            for (int i = dashIndices.Length - 1; i > 0; i--)
            {
                string path = assetImporter.assetPath.Substring(0, dashIndices[i]);
                IEnumerable<string> guids = FolderInspector.GetSettingGUIDs(path);
                if (guids == null)
                {
                    continue;
                }
                foreach (string guid in guids)
                {
                    ImportSettings importSettings = Utility.LoadObjectFromGUID<ImportSettings>(guid);
                    if (importSettings != null && importSettings.IsValid(assetImporter))
                    {
                        return importSettings;
                    }
                }
            }

            return null;
        }

        #endregion
    } 
}