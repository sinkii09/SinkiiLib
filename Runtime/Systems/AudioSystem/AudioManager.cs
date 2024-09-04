using SinkiiLib.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SinkiiLib.Systems
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public bool loop = false;
    }
    /* cac buoc Setup:
     *      1. tao 1 audio mixer trong project, mo cua so audio mixer, them 2 group nho lam con cua master group, dat ten lan luot SFX va BGM (ten gi cung dc), chon tung group, click chuot phai vao phan volume ben inspector, chon expose, click vao expose parameter, sua ten param thanh SFX va BGM (bat buoc giong, kiem tra dung param)     
     *      2. tao 1 prefab chua 1 audio source component (nho disable play on awake)
     *      3. tao 1 object trong scene va add audio manager
     *      4. keo audio mixer vao mixerGroup va audio prefab vao audioSourcePrefab
     *      5. them BGM va SFX clips vao cac array tuong ung , them ten cho moi sound
     *      
     * using: 
     *      
     *      BGM: goi method PlayBGM(ten BGM) de play BG music
     *      
     *      SFX: goi method PlaySFX(ten SFX) khi xai non_partial sound nhu UI
     *           goi method PlaySFXAtPosition(ten SFX, vi tri SFX) khi muon specific vi tri cua sound
     */
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private GameObject audioSourcePrefab;
        [SerializeField] private int poolSize = 10;

        [Header("Handle BGM")]
        [SerializeField] private AudioMixerGroup BGM_MixerGroup;
        [SerializeField] private Sound[] BGM_Sounds;
        private AudioSource BGM_Source;
        private Coroutine currentFade;

        [Header("Handle SFX")]
        [SerializeField] private AudioMixerGroup SFX_MixerGroup;
        [SerializeField] private Sound[] SFX_Sounds;


        [SerializeField] private Vector3 offSet; // su dung trong truong hop audio listener nam o camera, neu camera o cach qua xa nhan vat se ko nghe thay sound nen phai them offset, offset la khoang cach tu nhan vat den camera

        private Queue<AudioSource> audioPool;

        public override void Awake()
        {
            base.Awake();

            InitializeAudioPool();
        }

        #region Init & Handle Pool
        private void InitializeAudioPool()
        {
            audioPool = new Queue<AudioSource>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(audioSourcePrefab, transform);
                AudioSource audioSource = obj.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.playOnAwake = false;
                    audioSource.gameObject.SetActive(false);
                    audioPool.Enqueue(audioSource);
                }

            }
        }
        private AudioSource GetPooledAudioSource()
        {
            if (audioPool.Count > 0)
            {
                AudioSource audioSource = audioPool.Dequeue();
                audioSource.gameObject.SetActive(true);
                return audioSource;
            }
            else
            {
                GameObject obj = Instantiate(audioSourcePrefab, transform);
                AudioSource audioSource = obj.GetComponent<AudioSource>();
                return audioSource;
            }
        }
        private IEnumerator ReturnAudioSourceToPool(AudioSource audioSource, float delay)
        {
            yield return new WaitForSeconds(delay);
            audioSource.gameObject.SetActive(false);
            audioPool.Enqueue(audioSource);
        }

        #endregion


        #region SFX
        public void PlaySFX(string name)
        {
            Sound sound = Array.Find(SFX_Sounds, s => s.name == name);
            if (sound != null)
            {
                AudioSource audioSource = GetPooledAudioSource();
                audioSource.clip = sound.clip;
                audioSource.loop = sound.loop;
                audioSource.spatialBlend = 0f;
                audioSource.outputAudioMixerGroup = SFX_MixerGroup;
                audioSource.Play();
                if (!sound.loop)
                {
                    StartCoroutine(ReturnAudioSourceToPool(audioSource, sound.clip.length));
                }
            }
            else
            {
                Debug.LogWarning("Sound not found: " + name);
            }
        }

        public void PlaySFXAtPosition(string name, Vector3 position)
        {
            Sound sound = Array.Find(SFX_Sounds, s => s.name == name);
            if (sound != null)
            {
                AudioSource audioSource = GetPooledAudioSource();
                audioSource.clip = sound.clip;
                audioSource.loop = sound.loop;
                audioSource.outputAudioMixerGroup = SFX_MixerGroup;
                audioSource.spatialBlend = 1.0f;
                audioSource.gameObject.transform.position = position + offSet;
                audioSource.Play();
                if (!sound.loop)
                {
                    StartCoroutine(ReturnAudioSourceToPool(audioSource, sound.clip.length));
                }
            }
            else
            {
                Debug.LogWarning("Sound not found: " + name);
            }
        }

        #endregion

        #region BGM
        public void PlayBGM(string name)
        {
            if (currentFade != null)
            {
                StopCoroutine(currentFade);
            }
            if(BGM_Source == null)
            {
                BGM_Source = GetPooledAudioSource();
            }
            Sound sound = Array.Find(BGM_Sounds, s => s.name == name);
            currentFade = StartCoroutine(FadeOutIn(BGM_Source, sound , 1f));
        }
        IEnumerator FadeOutIn(AudioSource audioSource, Sound newSound, float duration)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(1f, 0f, t / duration);
                yield return null;
            }

            audioSource.clip = newSound.clip;
            audioSource.loop = true;
            audioSource.outputAudioMixerGroup = BGM_MixerGroup;

            audioSource.Play();

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(0f, 1f, t / duration);
                yield return null;
            }

            audioSource.volume = 1f;
        }
        #endregion

        #region Sound Settings
        public void SetBGMVolume(float volume)
        {
            BGM_MixerGroup.audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 10);
        }
        public void SetSFXVolume(float volume)
        {
            SFX_MixerGroup.audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }

        internal void ToggleBGM(bool value)
        {
            if (value)
            {
                BGM_MixerGroup.audioMixer.SetFloat("BGM", -5);
            }
            else
            {
                BGM_MixerGroup.audioMixer.SetFloat("BGM", -80);
            }
        }

        internal void ToggleSFX(bool value)
        {
            if (value)
            {
                SFX_MixerGroup.audioMixer.SetFloat("SFX", 0);
            }
            else
            {
                SFX_MixerGroup.audioMixer.SetFloat("SFX", -80);
            }
        }
        #endregion
    }
}
