using miao;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

namespace AchievementSystem
{

    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Ins { get; private set; }

        [SerializeField] private PopupWindow popup;

        [SerializeField] internal List<AchievementData> goldAchievements;
        [SerializeField] internal List<AchievementData> silverAchievements;
        [SerializeField] internal List<AchievementData> bronzeAchievements;

        private string savePath;


        private void Awake()
        {
            Ins = this;
            savePath = Path.Combine(Application.persistentDataPath, "Achievements.json");

            LoadAchievements(); // 游戏启动时读取本地存档
        }

        public void OnApplicationQuit()
        {
            SaveAchievements(); // 游戏退出时自动保存
        }

        public bool IsCompleted(Achievement achievement)
        {
            AchievementData data = Find(achievement);
            return data != null && data.IsCompleted; // 用只读属性访问
        }

        public static void Enable(Achievement achievement) => Ins.Find(achievement)?.Enable();

        private AchievementData Find(Achievement achievement)
        {
            AchievementData target;
            target = goldAchievements.Find(data => data.achievement == achievement);
            if (target != null) return target;
            target = silverAchievements.Find(data => data.achievement == achievement);
            if (target != null) return target;
            target = bronzeAchievements.Find(data => data.achievement == achievement);
            if (target != null) return target;
            return null;

        }

        internal void PopupAchievement(Achievement achievement)
        {
            PopupWindow p = Instantiate(popup, SingleCanvas.Ins.canvas.transform, false);  //pouupwindow
            p.SetAchievement(achievement);
            p.transform.SetParent(SingleCanvas.Ins.canvas.transform);
        }

        // ===  保存到 JSON ===
        public void SaveAchievements()
        {
            List<string> completed = new List<string>();

            AddCompleted(goldAchievements, completed);
            AddCompleted(silverAchievements, completed);
            AddCompleted(bronzeAchievements, completed);

            string json = JsonUtility.ToJson(new SaveWrapper { completedNames = completed }, true);
            File.WriteAllText(savePath, json);

#if UNITY_EDITOR
            Debug.Log($" 成就保存到: {savePath}");
#endif
        }

        private void AddCompleted(List<AchievementData> list, List<string> result)
        {
            foreach (var data in list)
            {
                if (data.isCompleted && data.achievement != null)
                    result.Add(data.achievement.name); // ScriptableObject 文件名
            }
        }

        // ===  从 JSON 读取 ===
        private void LoadAchievements()
        {
            if (!File.Exists(savePath))
            {
#if UNITY_EDITOR
                Debug.Log("没有发现成就存档，将创建新文件。");
#endif
                return;
            }

            string json = File.ReadAllText(savePath);
            SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(json);
            if (wrapper == null || wrapper.completedNames == null) return;

            // 遍历匹配并标记已完成
            MarkCompleted(goldAchievements, wrapper.completedNames);
            MarkCompleted(silverAchievements, wrapper.completedNames);
            MarkCompleted(bronzeAchievements, wrapper.completedNames);
        }

        private void MarkCompleted(List<AchievementData> list, List<string> completedNames)
        {
            foreach (var data in list)
            {
                if (data.achievement != null && completedNames.Contains(data.achievement.name))
                {
                    data.isCompleted = true; //标记完成但不播放动画
                }
            }
        }

        // ===内部存档结构 ===
        [Serializable]
        private class SaveWrapper
        {
            public List<string> completedNames;
        }
    }
}

