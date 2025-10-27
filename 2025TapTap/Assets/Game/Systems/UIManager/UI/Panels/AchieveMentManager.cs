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

            LoadAchievements(); // ��Ϸ����ʱ��ȡ���ش浵
        }

        public void OnApplicationQuit()
        {
            SaveAchievements(); // ��Ϸ�˳�ʱ�Զ�����
        }

        public bool IsCompleted(Achievement achievement)
        {
            AchievementData data = Find(achievement);
            return data != null && data.IsCompleted; // ��ֻ�����Է���
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

        // ===  ���浽 JSON ===
        public void SaveAchievements()
        {
            List<string> completed = new List<string>();

            AddCompleted(goldAchievements, completed);
            AddCompleted(silverAchievements, completed);
            AddCompleted(bronzeAchievements, completed);

            string json = JsonUtility.ToJson(new SaveWrapper { completedNames = completed }, true);
            File.WriteAllText(savePath, json);

#if UNITY_EDITOR
            Debug.Log($" �ɾͱ��浽: {savePath}");
#endif
        }

        private void AddCompleted(List<AchievementData> list, List<string> result)
        {
            foreach (var data in list)
            {
                if (data.isCompleted && data.achievement != null)
                    result.Add(data.achievement.name); // ScriptableObject �ļ���
            }
        }

        // ===  �� JSON ��ȡ ===
        private void LoadAchievements()
        {
            if (!File.Exists(savePath))
            {
#if UNITY_EDITOR
                Debug.Log("û�з��ֳɾʹ浵�����������ļ���");
#endif
                return;
            }

            string json = File.ReadAllText(savePath);
            SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(json);
            if (wrapper == null || wrapper.completedNames == null) return;

            // ����ƥ�䲢��������
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
                    data.isCompleted = true; //�����ɵ������Ŷ���
                }
            }
        }

        // ===�ڲ��浵�ṹ ===
        [Serializable]
        private class SaveWrapper
        {
            public List<string> completedNames;
        }
    }
}

