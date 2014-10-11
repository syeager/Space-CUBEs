// Little Byte Games
// Author: Steve Yeager
// Created: 2014.09.04
// Edited: 2014.09.04

using System;
using UnityEditor;
using UnityEngine;

namespace LittleByte.ImportSettings
{
    /// <summary>
    /// All import settings for a audio importer.
    /// </summary>
    public class AudioSettings : ImportSettings
    {
        #region Public Fields

        public AudioImporterFormat audioFormat;
        public bool _3DSound;
        public bool forceMono;
        public AudioImporterLoadType loadType = AudioImporterLoadType.DecompressOnLoad;
        public bool hardwareDecoding;
        public bool gaplessLooping;

        [Range(32, 240)]
        public int compression = 156;

        #endregion

        #region ImportSettings Override

        /// <inherit/>
        public override Type AssetType
        {
            get { return typeof(AudioImporter); }
        }

        /// <inherit/>
        public override string Name
        {
            get { return "Default Audio"; }
        }

        /// <inherit/>
        public override void Apply(AssetImporter assetImporter)
        {
            AudioImporter importer = (AudioImporter)assetImporter;
            importer.format = audioFormat;
            importer.threeD = _3DSound;
            importer.forceToMono = forceMono;
            importer.loadType = loadType;
            importer.hardware = hardwareDecoding;
            importer.loopable = gaplessLooping;
            importer.compressionBitrate = compression;
        }

        #endregion
    } 
}