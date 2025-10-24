using System.Collections.Generic;
using UnityEngine;
using miao;

namespace UI
{
    public class RankPanel : MonoBehaviour
    {
        [SerializeField] private RankItem template;

        public void Display(List<ScoreboardData.Entry> entrys)
        {
            foreach (Transform child in transform) Destroy(child.gameObject);
            for (int i = 0; i < entrys.Count; i++)
            {
                var entry = entrys[i];
                RankItem item = Instantiate(template, transform);
                item.SetData(i, entry.name, entry.score);
            }
        }

        private void OnEnable() => Display(ScoreManager.Instance.rankList);
    }
}
