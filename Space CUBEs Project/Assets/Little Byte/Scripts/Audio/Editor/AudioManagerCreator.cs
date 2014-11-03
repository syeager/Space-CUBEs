// Little Byte Games

using System;
using Annotations;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LittleByte.Audio
{
    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerCreator : Creator<AudioManager>
    {
        #region Textures

        public const string MasterAudioFolderPath = "MasterAudio";

        public static Texture deleteTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/deleteIcon.png", MasterAudioFolderPath)) as Texture;
        public static Texture gearTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/gearIcon.png", MasterAudioFolderPath)) as Texture;
        public static Texture muteOffTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/muteOff.png", MasterAudioFolderPath)) as Texture;
        public static Texture muteOnTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/muteOn.png", MasterAudioFolderPath)) as Texture;
        public static Texture nextTrackTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/nextTrackIcon.png", MasterAudioFolderPath)) as Texture;
        public static Texture pauseTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/pauseIcon.png", MasterAudioFolderPath)) as Texture;
        public static Texture pauseOnTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/pauseIconOn.png", MasterAudioFolderPath)) as Texture;
        public static Texture playSongTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/playIcon.png", MasterAudioFolderPath)) as Texture;
        public static Texture previousTrackTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/prevTrackIcon.png", MasterAudioFolderPath)) as Texture;
        public static Texture randomTrackTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/randomIcon.png", MasterAudioFolderPath)) as Texture;
        public static Texture soloOffTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/soloOff.png", MasterAudioFolderPath)) as Texture;
        public static Texture soloOnTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/soloOn.png", MasterAudioFolderPath)) as Texture;
        public static Texture previewTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/speakerIcon.png", MasterAudioFolderPath)) as Texture;
        public static Texture stopTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/stopIcon.png", MasterAudioFolderPath)) as Texture;

        public static Texture[] ledTextures =
        {
            EditorGUIUtility.LoadRequired(string.Format("{0}/LED5.png", MasterAudioFolderPath)) as Texture,
            EditorGUIUtility.LoadRequired(string.Format("{0}/LED4.png", MasterAudioFolderPath)) as Texture,
            EditorGUIUtility.LoadRequired(string.Format("{0}/LED3.png", MasterAudioFolderPath)) as Texture,
            EditorGUIUtility.LoadRequired(string.Format("{0}/LED2.png", MasterAudioFolderPath)) as Texture,
            EditorGUIUtility.LoadRequired(string.Format("{0}/LED1.png", MasterAudioFolderPath)) as Texture,
            EditorGUIUtility.LoadRequired(string.Format("{0}/LED0.png", MasterAudioFolderPath)) as Texture
        };

        #endregion

        #region GUI Fields

        private static AudioManagerCreator editor;
        private AudioManager audioManager;
        private SerializedObject poolManager;
        private GUIStyle imageButton;

        #endregion

        #region Const Fields

        private const string PrefabName = "_AudioManager";
        private const string PrefabPath = "Assets/Global/";

        #endregion

        #region Creator Overrides

        [MenuItem("GameObject/Singletons/Audio Manager", false, 4)]
        public static void Create()
        {
            Create(PrefabName, "", true);
        }

        #endregion

        #region Editor Overrides

        [UsedImplicitly]
        private void OnEnable()
        {
            if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab) return;
            editor = this;
            audioManager = target as AudioManager;
            poolManager = new SerializedObject(audioManager.poolManager);

            if (!Application.isPlaying)
            {
                audioManager.Initialize();
            }
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            editor = null;
        }

        public override void OnInspectorGUI()
        {
            if (PrefabUtility.GetPrefabType(target) == PrefabType.Prefab)
            {
                GUILayout.Label("Need instance.");
                return;
            }

            if (poolManager == null) poolManager = new SerializedObject(audioManager.poolManager);
            poolManager.Update();
            serializedObject.Update();

            EditorGUIUtility.LookLikeControls();
            imageButton = new GUIStyle(GUI.skin.button) {padding = new RectOffset(0, 0, 0, 0)};

            MasterVolume();
            Buses();
            //AudioGroups();
            EditorGUILayout.PropertyField(poolManager.FindProperty("poolList"), true);

            poolManager.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Private Methods

        private void MasterVolume()
        {
            EditorGUILayout.LabelField("Master", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal("box");
            {
                // label
                GUILayout.Label("Master Volume", GUILayout.Width(100f));

                // level
                float volume = EditorGUILayout.Slider(audioManager.MasterVolume.level, 0f, 1f, GUILayout.Width(150f));
                if (volume != audioManager.MasterVolume.level)
                {
                    audioManager.setMasterLevel(volume);
                    if (!Application.isPlaying) audioManager.Save();
                }

                // mute
                if (GUILayout.Button(audioManager.MasterVolume ? muteOnTexture : muteOffTexture, imageButton, GUILayout.Width(16)))
                {
                    audioManager.setMasterMute(!audioManager.MasterVolume);
                    if (!Application.isPlaying) audioManager.Save();
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private void Buses()
        {
            EditorGUILayout.LabelField("Buses", EditorStyles.boldLabel);
            GUILayout.BeginVertical("box");
            {
                foreach (object value in Enum.GetValues(typeof(AudioManager.Bus)))
                {
                    AudioManager.Bus bus = (AudioManager.Bus)value;
                    GUILayout.BeginHorizontal();
                    {
                        // label
                        GUILayout.Label(bus + " Volume", GUILayout.Width(100f));

                        // level
                        float volume = EditorGUILayout.Slider(audioManager.busVolumes[bus].level, 0f, 1f, GUILayout.Width(150f));
                        if (volume != audioManager.busVolumes[bus].level)
                        {
                            audioManager.setBusLevel(bus, volume);
                            if (!Application.isPlaying) audioManager.Save();
                        }

                        // actual volume
                        GUI.enabled = !audioManager.MasterVolume && !audioManager.busVolumes[bus];
                        EditorGUILayout.LabelField((audioManager.MasterVolume * volume).ToString("0.00"), GUILayout.Width(30f));
                        GUI.enabled = true;

                        // mute
                        if (GUILayout.Button(audioManager.busVolumes[bus] ? muteOnTexture : muteOffTexture, imageButton, GUILayout.Width(16)))
                        {
                            audioManager.setBusMute(bus, !audioManager.busVolumes[bus]);
                            if (!Application.isPlaying) audioManager.Save();
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private bool groupToggle;

        private void AudioGroups()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Audio Groups", EditorStyles.boldLabel);
                if (GUILayout.Button(groupToggle ? "O" : "|", EditorStyles.miniButton))
                {
                    groupToggle = !groupToggle;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (groupToggle)
            {
                GUILayout.BeginVertical("box");
                {
                    foreach (Pool pool in audioManager.poolManager.poolList)
                    {
                        //AudioManager.Bus bus = ((AudioPlayer)pool.prefab).bus;

                        // toolbar
                        GUILayout.BeginHorizontal();
                        {
                            // label
                            EditorGUILayout.Foldout(true, pool.prefab.name);

                            //// level
                            //float volume = EditorGUILayout.Slider(audioManager.busVolumes[bus].level, 0f, 1f, GUILayout.Width(150f));
                            //if (volume != audioManager.busVolumes[bus].level)
                            //{
                            //    audioManager.SetBusLevel(bus, volume);
                            //    if (Application.isEditor) audioManager.Save();
                            //}

                            //// actual volume
                            //GUI.enabled = !audioManager.MasterVolume && !audioManager.busVolumes[bus];
                            //EditorGUILayout.LabelField((audioManager.MasterVolume * volume).ToString("0.00"), GUILayout.Width(30f));
                            //GUI.enabled = true;

                            //// mute
                            //if (GUILayout.Button(audioManager.busVolumes[bus] ? muteOnTexture : muteOffTexture, imageButton, GUILayout.Width(16)))
                            //{
                            //    audioManager.SetBusMute(bus, !audioManager.busVolumes[bus]);
                            //    if (Application.isEditor) audioManager.Save();
                            //}
                        }
                        GUILayout.EndHorizontal();

                        // pool
                        if (true)
                        {
                            GUILayout.BeginHorizontal("box");
                            {
                                //EditorGUILayout.PropertyField()
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
        }

        #endregion

        #region Menu Methods

        [MenuItem("Assets/Audio/Create Audio Player", true, 0)]
        public static bool ValidateCreateAudioPlayer()
        {
            return Selection.activeObject is AudioClip;
        }

        [MenuItem("Assets/Audio/Create Audio Player", false, 0)]
        public static void CreateAudioPlayer()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path = path.Substring(0, path.Length - 4) + ".prefab";
            Debugger.Log(path);

            GameObject surrogate = new GameObject("audio player", typeof(AudioPlayer));
            Object prefab = PrefabUtility.CreatePrefab(path, surrogate);
            DestroyImmediate(surrogate);

            AudioPlayer audioPlayer = ((GameObject)prefab).GetComponent<AudioPlayer>();
            audioPlayer.audio.playOnAwake = false;
            audioPlayer.audio.clip = (AudioClip)Selection.activeObject;
            audioPlayer.myAudio = audioPlayer.audio;
            audioPlayer.myTransform = audioPlayer.transform;

            Selection.activeObject = prefab;
        }

        [MenuItem("Assets/Audio/Create Audio Player Variation", true, 1)]
        public static bool ValidateCreateAudioPlayerVariation()
        {
            return Selection.activeObject is AudioClip;
        }

        [MenuItem("Assets/Audio/Create Audio Player Variation", false, 1)]
        public static void CreateAudioPlayerVariation()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path = path.Substring(0, path.Length - 4) + ".prefab";
            Debugger.Log(path);

            GameObject surrogate = new GameObject("audio player", typeof(AudioPlayerVariation));
            Object prefab = PrefabUtility.CreatePrefab(path, surrogate);
            DestroyImmediate(surrogate);

            AudioPlayerVariation audioPlayer = ((GameObject)prefab).GetComponent<AudioPlayerVariation>();
            audioPlayer.audio.playOnAwake = false;
            audioPlayer.audio.clip = (AudioClip)Selection.activeObject;
            audioPlayer.myAudio = audioPlayer.audio;
            audioPlayer.myTransform = audioPlayer.transform;

            Selection.activeObject = prefab;
        }

        [MenuItem("Assets/Audio/Create Playlist", true, 2)]
        public static bool ValidateCreatePlaylist()
        {
            return Selection.activeObject is AudioClip;
        }

        [MenuItem("Assets/Audio/Create Playlist", false, 2)]
        public static void CreatePlaylist()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            path = path.Substring(0, path.Length - 4) + ".prefab";
            Debugger.Log(path);

            AudioClip clip = (AudioClip)Selection.activeObject;
            GameObject surrogate = new GameObject(clip.name, typeof(Playlist));
            Object prefab = PrefabUtility.CreatePrefab(path, surrogate);
            DestroyImmediate(surrogate);

            Playlist playlist = ((GameObject)prefab).GetComponent<Playlist>();
            playlist.audio.playOnAwake = false;
            playlist.audio.loop = true;
            playlist.audio.clip = clip;
            playlist.myAudio = playlist.audio;
            playlist.playlistName = clip.name;

            Selection.activeObject = prefab;
        }

        [MenuItem("Shortcuts/Mute Game %M", true, 150)]
        public static bool ValidateMute()
        {
            return FindObjectOfType(typeof(AudioManager));
        }

        [MenuItem("Shortcuts/Mute Game %M", false, 150)]
        public static void Mute()
        {
            AudioManager audioManager = (AudioManager)FindObjectOfType(typeof(AudioManager));
            audioManager.setMasterMute(!audioManager.MasterVolume);
            if (!Application.isPlaying) audioManager.Save();

            if (editor != null)
            {
                editor.Repaint();
            }
        }

        #endregion
    }
}