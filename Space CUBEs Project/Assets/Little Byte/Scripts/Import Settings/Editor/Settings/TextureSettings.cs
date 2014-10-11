// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.02
// Edited: 2014.09.04

using System;
using UnityEditor;
using UnityEngine;

namespace LittleByte.ImportSettings
{
    public class TextureSettings : ImportSettings
    {
        #region Public Fields

        public TextureImporterNPOTScale nonPowerOf2 = TextureImporterNPOTScale.None;
        public TextureImporterGenerateCubemap generateCubemap;
        public bool readWriteEnabled;
        public bool alphaFromGreyscale;
        public bool bypassSampling;
        public SpriteImportMode spriteMode = SpriteImportMode.Multiple;
        public string packingTag;
        public int pixelsToUnits = 100;
        public bool generateMipMaps;
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        public FilterMode filterMode = FilterMode.Bilinear;
        public int anisoLevel = 1;
        public TextureImporterFormat format = TextureImporterFormat.AutomaticTruecolor;

        #endregion

        #region ImportSettings Override

        /// <inherit/>
        public override Type AssetType
        {
            get { return typeof(TextureImporter); }
        }


        /// <inherit/>
        public override string Name
        {
            get { return "Default Texture"; }
        }


        /// <inherit/>
        public override void Apply(AssetImporter assetImporter)
        {
            TextureImporter importer = (TextureImporter)assetImporter;

            importer.textureType = TextureImporterType.Advanced;
            importer.npotScale = nonPowerOf2;

            if (nonPowerOf2 != TextureImporterNPOTScale.None)
            {
                importer.generateCubemap = generateCubemap;
            }

            importer.isReadable = readWriteEnabled;

            importer.grayscaleToAlpha = alphaFromGreyscale;
            importer.generateMipsInLinearSpace = bypassSampling;

            importer.spriteImportMode = spriteMode;
            importer.spritePackingTag = packingTag;
            importer.spritePixelsToUnits = pixelsToUnits;

            importer.mipmapEnabled = generateMipMaps;

            importer.wrapMode = wrapMode;
            importer.filterMode = filterMode;
            importer.anisoLevel = anisoLevel;

            importer.textureFormat = format;
        }

        #endregion
    } 
}