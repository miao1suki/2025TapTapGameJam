using miao;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace miao
{
    public class RecordUIController : MonoBehaviour
    {
        public static RecordUIController Instance;

        [System.Serializable]
        public class RecordUI
        {
            public string recordID;
            public Image icon;
            public AudioClip musicClip;
        }

        public List<RecordUI> recordUIList;

        private void Awake()
        {
            Instance = this;
        }

        public void UnlockRecord(RecordData data)
        {
            foreach (var ui in recordUIList)
            {
                if (ui.recordID == data.recordID)
                {
                    ui.icon.color = Color.white; // ½âËøÐ§¹û
                    AudioSource.PlayClipAtPoint(data.musicClip, Camera.main.transform.position);
                }
            }
        }
    }
}



