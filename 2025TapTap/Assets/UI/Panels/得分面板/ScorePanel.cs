using DG.Tweening;
using ScoreSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class ScorePanel : MonoBehaviour
    {
        [SerializeField] private ScoreText scoreText;
        [SerializeField] public ScoreText totalScoreText;
        [SerializeField] private MultiplierCell multiplierCell;
        [SerializeField] private ScoreCell scoreCell;
        [SerializeField] private RectTransform fillArea;
        [SerializeField] private float comboTime;

        [SerializeField] private GameObject scoreEffect;
        private Tweener timeBarTweener;

        private AudioSource countingAudioSource;

        private void Start() => Init();

        public void Init()
        {
            if (scoreEffect != null)
            {
                scoreEffect.SetActive(false);
            }

            if (scoreText.Value != 0)
            {
                totalScoreText.Value += scoreText.Value;
            };

            // 把玩家当前分数设置为总分
            if (miao.ScoreManager.Instance != null)
            {
                miao.ScoreManager.Instance.AddScore(scoreText.Value);
            }
        
            scoreText.Clear();
            multiplierCell.Multiplier = 0;

            // 计分阶段结束：停止计数器声 + 播放结算奖励音
            if (countingAudioSource != null)
            {
                miao.AudioManager.Instance.StopAudio(countingAudioSource);
                countingAudioSource = null;

                miao.AudioManager.Instance.PlayAudio(
                    "数字增长完成后结算的奖励音效（欢快）",
                    transform.position,
                    false,
                    0.5f
                );
            }

        }
        public void IncreaseScore(int score)
        {
            if (scoreEffect != null)
            {
                scoreEffect.SetActive(true);
            }

            // 如果还没有播放计数器音效，就开始播放
            if (countingAudioSource == null && SceneManager.GetActiveScene().name !="Title")
            {
                countingAudioSource = miao.AudioManager.Instance.PlayAudio(
                    "数字增长时，数字不停跳动的计数器声（欢快、紧张）",
                    transform.position,
                    true,
                    0.1f
                );
            }

            if (multiplierCell.Multiplier == 0)
            {
                multiplierCell.Multiplier = 1;
                TimeBarStart();
            }
            int finalAdded = multiplierCell.Multiplier * score;
            scoreText.Value += finalAdded;
            scoreCell.Text = "+" + finalAdded;
            scoreCell.OnEnable();
        }
        public void IncreaseMultiplier()
        {
            if (scoreEffect != null)
            {
                scoreEffect.SetActive(true);
            }

            multiplierCell.Multiplier = multiplierCell.Multiplier == 0 ? 2 : multiplierCell.Multiplier + 1;
            TimeBarStart();
        }
        private void TimeBarStart()
        {
            timeBarTweener.Kill();
            fillArea.anchorMax = Vector2.one;
            timeBarTweener = DOTween.To(
                () => fillArea.anchorMax.x,
                value => fillArea.anchorMax = new(value, 1),
                0,
                comboTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    Init();
                });
        }
    }
}
