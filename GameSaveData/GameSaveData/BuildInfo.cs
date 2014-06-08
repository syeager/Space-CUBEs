// GameSaveData
// Author: Steve Yeager
// Created: 2014.06.07
// Edited: 2014.06.07

using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameSaveData
{
    [Serializable]
    [ProtoContract]
    public class BuildInfo
    {
        #region Public Fields

        [ProtoMember(1)]
        public string name;

        [ProtoMember(2)]
        public float health;

        [ProtoMember(3)]
        public float shield;

        [ProtoMember(4)]
        public float speed;

        [ProtoMember(5)]
        public float damage;

        [ProtoMember(6)]
        public List<KeyValuePair<int, CUBEGridInfo>> partList;

        #endregion

        #region Static Fields

        public const string DataSep = "|";
        public const string PieceSep = "/";
        public const string ColorSep = "~";

        #endregion

        #region Properties

        public static string Empty
        {
            get { return "" + DataSep + "0" + DataSep + "0" + DataSep + "0" + DataSep + "0" + DataSep + "0"; }
        }

        #endregion
    }
}