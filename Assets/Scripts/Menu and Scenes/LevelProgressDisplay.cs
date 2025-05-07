using UnityEngine;
using UnityEngine.UI;

public class LevelProgressDisplay : MonoBehaviour
{
    [System.Serializable]
    public class LevelVisual
    {
        public GameObject Yellow_Wings;
        public GameObject Red_Wings;
        public GameObject Lock;
        public GameObject[] Gems;
        public GameObject[] GemFrames;
        public Button Button;
        public HoverActivatorSingle HoverActivator;
    }

    [SerializeField] private LevelVisual[] _levelVisuals;

    private void Awake()
    {
        int completedCount = PlayerPrefs.GetInt("LevelsCompletedCount", 0);

        for (int i = 0; i < _levelVisuals.Length; i++)
        {
            LevelVisual visual = _levelVisuals[i];

            // —начала выключаем всЄ
            visual.Yellow_Wings.SetActive(false);
            visual.Red_Wings.SetActive(false);
            visual.Lock.SetActive(false);

            foreach (var gem in visual.Gems)
            {
                gem.SetActive(false);
            }

            foreach (var frame in visual.GemFrames)
            {
                frame.SetActive(false);
            }

            // ”ровни с индексом до completedCount включительно Ч доступные
            bool isUnlocked = i <= completedCount;

            visual.Button.interactable = isUnlocked;
            visual.HoverActivator.Block = !isUnlocked;

            if (!isUnlocked)
            {
                visual.Lock.SetActive(true);
                continue;
            }

            int levelIndex = i + 1;

            // Wings по сложности
            int difficulty = PlayerPrefs.GetInt($"LevelDifficulty_{levelIndex}", 0);
            if (difficulty == 1)
            {
                visual.Yellow_Wings.SetActive(true);
            }
            else if (difficulty == 2)
            {
                visual.Red_Wings.SetActive(true);
            }

            // √емы
            int gems = PlayerPrefs.GetInt($"LevelGems_{levelIndex}", 0);
            for (int g = 0; g < gems && g < visual.Gems.Length; g++)
            {
                visual.Gems[g].SetActive(true);
            }

            for (int g = gems; g < visual.GemFrames.Length; g++)
            {
                visual.GemFrames[g].SetActive(true);
            }
        }
    }
}
