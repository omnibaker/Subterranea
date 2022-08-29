using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Subterranea
{
    /// <summary>
    /// Control loading/unloading of all scenes in the game; all scene are Additive to the global Main scene
    /// </summary>
    public class SceneLoader : Singleton<SceneLoader>
    {
        [SerializeField] private CanvasGroup _loadCanvasGroup;

        private const float FADE_TIME = 0.3f;
        public Scene CurrentScene { set; get; }
        public Scene LoadingScene { set; get; }

        public void Awake()
        {
            CreateInstance(this, gameObject);
        }

        /// <summary>
        /// Called from Unity when scene is loaded
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LoadingActions(scene, mode);
        }

        /// <summary>
        /// Called from Unity when scene has unloaded
        /// </summary>
        private void OnSceneUnloaded(Scene scene, LoadSceneMode mode) { }

        /// <summary>
        /// Public method to start process of loading new scene
        /// </summary>
        public void GoToScene(string sceneName, AssetBundle assetBundle = null)
        {
            StartCoroutine(FadeAndLoad(sceneName));
        }

        /// <summary>
        /// Start fading out to black and 
        /// </summary>
        private IEnumerator FadeAndLoad(string sceneName)
        {
            yield return StartCoroutine(FadeLoading(true, FADE_TIME));
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Processed for when scene loaded
        /// </summary>
        private void LoadingActions(Scene scene, LoadSceneMode mode)
        {
            LoadingScene = scene;

            // Ignore if first scene to load
            if (mode != LoadSceneMode.Additive) return;

            // Assign loading scene as Active
            SceneManager.SetActiveScene(scene);

            // Update current scene
            if (CurrentScene.name != null)
            {
                DisableOldScene();
            }
            CurrentScene = LoadingScene;

            // Start fading in
            StartCoroutine(FadeLoading(false, FADE_TIME));

        }

        /// <summary>
        /// Disable old scene so it doesn't interfere with scene just loaded
        /// </summary>
        private void DisableOldScene()
        {
            if (CurrentScene.IsValid())
            {
                // Turn every object off
                GameObject[] oldSceneObjects = CurrentScene.GetRootGameObjects();
                for (int i = 0; i < oldSceneObjects.Length; i++)
                {
                    oldSceneObjects[i].SetActive(false);
                }

                // Unload it
                SceneManager.UnloadSceneAsync(CurrentScene);
            }
        }


        /// <summary>
        /// Controls fade in/out of black segue image
        /// </summary>
        public IEnumerator FadeLoading(bool fadeToBlack, float duration)
        {
            float targetValue = fadeToBlack ? 1f : 0;
            float startingValue = _loadCanvasGroup.alpha;
            float time = 0;

            while (time < 1)
            {
                _loadCanvasGroup.alpha = Mathf.Lerp(startingValue, targetValue, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            _loadCanvasGroup.alpha = targetValue;
        }


        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded -= OnSceneUnloaded;
        }
    }
}