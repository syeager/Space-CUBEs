// Little Byte Games
// Author: Steve Yeager
// Created: 2014.10.01
// Edited: 2014.10.01

using System;
using System.Collections;
using SimpleJSON;
using UnityEngine;

namespace LittleByte.GooglePlay
{
    public static class GoogleProfilePic
    {
        #region Const Fields

        private const string ProfilePicURL = "https://www.googleapis.com/plus/v1/people/{0}?fields=image&key={1}";

        #endregion

        #region Properties

        public static string APIKey { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public static void SetKey(string key)
        {
            APIKey = key;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="size"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        public static Coroutine LoadProfilePic(string id, int size, Action<Texture2D> onComplete)
        {
            var job = new Job(LoadingProfilePic(id, size, onComplete), false);
            return job.Start();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="size"></param>
        /// <param name="onComplete"></param>
        private static IEnumerator LoadingProfilePic(string id, int size, Action<Texture2D> onComplete)
        {
            string get = string.Format(ProfilePicURL, id, APIKey);
            WWW www = new WWW(get);
            yield return www;

            if (www.error == null)
            {
                JSONNode json = JSON.Parse(www.text);
                string picURL = json["image"]["url"].Value;
                picURL = picURL.Substring(0, picURL.Length - 2) + size;

                WWW picWWW = new WWW(picURL);
                yield return picWWW;

                onComplete(picWWW.texture);
            }
            else
            {
                Debugger.LogError("Error: " + www.error);
            }
        }

        #endregion
    }
}