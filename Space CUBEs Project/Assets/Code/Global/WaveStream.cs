// Steve Yeager
// 1.10.2014

using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System;

public static class WaveStream
{
    #region Const Fields

    private const string PREFIX = "Wave_";
    private const string POSTFIX = ".xml";
    private const string SAVEPATH = "/Files/Level Waves/";

    #endregion


    #region Static Methods

    /// <summary>
    /// Saves Enemies and their positions.
    /// </summary>
    /// <param name="level">Level to save to.</param>
    /// <param name="wave">Wave number.</param>
    /// <param name="enemies">List of enemy references.</param>
    /// <param name="overwrite">If not true wave won't write if there is already one created.</param>
    /// <returns>True, wave already created.</returns>
    public static bool Write(string level, int wave, Enemy[] enemies, bool overwrite)
    {
        string path = Application.dataPath + SAVEPATH + PREFIX + level + POSTFIX;
        string waveString = wave.ToString();
        // create file
        //if (!File.Exists(path))
        //{
        //    FileStream file = File.Create(path);
        //    file.Close();
        //}
        XDocument doc = XDocument.Load(path);
        XElement levelElement = doc.Element("Level");
        XElement waveElement = levelElement.Elements("Wave").SingleOrDefault(w => w.Attribute("id").Value == waveString);

        // wave already there
        if (waveElement != null)
        {
            if (overwrite)
            {
                // overwrite
                waveElement.ReplaceWith(WriteWave(wave, enemies));
                doc.Save(path);
            }

            return true;
        }
        else
        {
            XElement biggerWave = levelElement.Elements("Wave").FirstOrDefault(w => int.Parse(w.Attribute("id").Value) > wave);

            // add as first child to level
            if (biggerWave == null)
            {
                levelElement.Add(WriteWave(wave, enemies));
            }
            // add before next biggest wave
            else
            {
                biggerWave.AddBeforeSelf(WriteWave(wave, enemies));
            }

            doc.Save(path);
            return false;
        }
    }


    public static WaveEnemyData[][] Read(string level)
    {
        string path = Application.dataPath + SAVEPATH + PREFIX + level + POSTFIX;
        XDocument doc = XDocument.Load(path);
        XElement levelElement = doc.Element("Level");

        var waves = levelElement.Elements("Wave").ToArray();
        var waveList = new WaveEnemyData[waves.Length][];
        foreach (var wave in waves)
        {
            waveList[int.Parse(wave.Attribute("id").Value)-1] = wave.Elements("Enemy").Select(e => new WaveEnemyData(
                                                                                            EnemyClass(e.Element("Class").Value),
                                                                                            Utility.ParseV3(e.Element("Position").Value))).ToArray();
        }

        return waveList;
    }


    public static WaveEnemyData[] Read(string level, int wave)
    {
        string path = Application.dataPath+SAVEPATH + PREFIX + level + POSTFIX;
        XDocument doc = XDocument.Load(path);

        string waveString = wave.ToString();
        XElement waveElement = doc.Element("Level").Elements("Wave").SingleOrDefault(w => w.Attribute("id").Value == waveString);

        // empty
        if (waveElement == null)
        {
            return null;
        }

        return waveElement.Elements("Enemy").Select(e => new WaveEnemyData(
                                                    EnemyClass(e.Element("Class").Value),
                                                    Utility.ParseV3(e.Element("Position").Value))).ToArray();
    }

    #endregion

    #region Private Methods

    private static Enemy.Classes EnemyClass(string enemy)
    {
        switch (enemy)
        {
            case "Grunt": return Enemy.Classes.Grunt;
        }

        Debugger.LogError(enemy + " is not a valid Enemy.Class.");
        throw new Exception();
    }


    private static XElement WriteWave(int id, Enemy[] enemies)
    {
        XElement wave = new XElement("Wave", new XAttribute("id", id));
        foreach (var enemy in enemies)
        {
            XElement enemyElement = new XElement("Enemy");
            enemyElement.Add(new XElement("Class", enemy.enemyClass));
            enemyElement.Add(new XElement("Position", enemy.transform.position));
            wave.Add(enemyElement);
        }
        return wave;
    }

    #endregion
}