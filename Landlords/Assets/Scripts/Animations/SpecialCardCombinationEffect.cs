using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PIXEL.Landlords.Animation.SpecialCardAnimations;

namespace PIXEL.Landlords.Game
{
    public class SpecialCardCombinationEffect : MonoBehaviour
    {
        public void AudioPlay()
        {
            AudioSource currentAudioSource = gameObject.GetComponent<AudioSource>();
            currentAudioSource.Play();
        }

        public void AnimationDownHide()
        {
            //gameObject.SetActive(false);
            gameObject.GetComponent<Image>().enabled = false;

            if (gameObject.transform.childCount != 0)
            {
                transform.GetChild(0).GetComponent<Image>().enabled = false;
            }

            Invoke("Hide", 1f);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            SpecialCardCombinationsAnimations.Instance.Recycle(gameObject);
        }
    }
}