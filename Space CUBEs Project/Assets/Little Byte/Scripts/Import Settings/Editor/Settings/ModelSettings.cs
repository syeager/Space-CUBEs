// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.02
// Edited: 2014.09.03

using System;
using UnityEditor;
using UnityEngine;

namespace LittleByte.ImportSettings
{
    /// <summary>
    /// All import settings for a model importer.
    /// </summary>
    public class ModelSettings : ImportSettings
    {
        #region Public Fields

        public float scaleFactor = 1f;
        public ModelImporterMeshCompression meshCompression;
        public bool readWriteEnabled;
        public bool optimizeMesh;
        public bool importBlendShapes;
        public bool generateColliders;
        public bool swapUVs;
        public bool generateLightmapUV;
        public ModelImporterTangentSpaceMode normals = ModelImporterTangentSpaceMode.Import;
        public ModelImporterTangentSpaceMode tangents = ModelImporterTangentSpaceMode.Calculate;
        public float smoothingAngle = 60;
        public bool splitTangents;
        public bool importMaterials;
        public ModelImporterMaterialName materialNaming = ModelImporterMaterialName.BasedOnTextureName;
        public ModelImporterMaterialSearch materialSearch = ModelImporterMaterialSearch.RecursiveUp;

        #endregion

        #region Const Fields

        private const float SmoothingAngleMin = 0f;
        private const float SmoothingAngleMax = 180f;

        #endregion

        #region ImportSettings Override

        /// <inherit/>
        public override Type AssetType
        {
            get { return typeof(ModelImporter); }
        }


        /// <inherit/>
        public override string Name
        {
            get { return "Default Model"; }
        }


        /// <inherit/>
        public override void Apply(AssetImporter assetImporter)
        {
            ModelImporter importer = (ModelImporter)assetImporter;

            importer.globalScale = scaleFactor;
            importer.meshCompression = meshCompression;
            importer.isReadable = readWriteEnabled;
            importer.optimizeMesh = optimizeMesh;
            importer.importBlendShapes = importBlendShapes;
            importer.addCollider = generateColliders;
            importer.swapUVChannels = swapUVs;
            importer.generateSecondaryUV = generateLightmapUV;
            importer.normalImportMode = normals;
            importer.tangentImportMode = tangents;
            importer.normalSmoothingAngle = smoothingAngle;
            importer.importMaterials = importMaterials;
            if (importer.importMaterials)
            {
                importer.materialName = materialNaming;
                importer.materialSearch = materialSearch;
            }
        }


        /// <inherit/>
        public override void OnGUI()
        {
            // meshes
            EditorGUILayout.LabelField("Meshes", EditorStyles.boldLabel);
            scaleFactor = EditorGUILayout.FloatField("Scale Factor", scaleFactor);
            meshCompression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Mesh Compression", meshCompression);
            readWriteEnabled = EditorGUILayout.Toggle("Read/Write Enabled", readWriteEnabled);
            optimizeMesh = EditorGUILayout.Toggle("Optimize Mesh", optimizeMesh);
            importBlendShapes = EditorGUILayout.Toggle("Import BlendShapes", importBlendShapes);
            generateColliders = EditorGUILayout.Toggle("Generate Colliders", generateColliders);
            swapUVs = EditorGUILayout.Toggle("Swap UVs", swapUVs);
            generateLightmapUV = EditorGUILayout.Toggle("Generate Lightmap UVs", generateLightmapUV);

            // normals and tangents
            EditorGUILayout.LabelField("Normals & Tangents", EditorStyles.boldLabel);
            normals = (ModelImporterTangentSpaceMode)EditorGUILayout.EnumPopup("Normals", normals);
            tangents = (ModelImporterTangentSpaceMode)EditorGUILayout.EnumPopup("Tangents", tangents);
            GUI.enabled = normals == ModelImporterTangentSpaceMode.Calculate;
            smoothingAngle = EditorGUILayout.Slider("Smoothing Angle", smoothingAngle, SmoothingAngleMin, SmoothingAngleMax);
            GUI.enabled = true;
            GUI.enabled = tangents == ModelImporterTangentSpaceMode.None;
            splitTangents = EditorGUILayout.Toggle("Split Tangents", splitTangents);
            GUI.enabled = true;

            // materials
            EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
            importMaterials = EditorGUILayout.Toggle("Import Materials", importMaterials);

            if (importMaterials)
            {
                materialNaming = (ModelImporterMaterialName)EditorGUILayout.EnumPopup("Material Naming", materialNaming);
                materialSearch = (ModelImporterMaterialSearch)EditorGUILayout.EnumPopup("Material Search", materialSearch);
            }
        }

        #endregion
    } 
}