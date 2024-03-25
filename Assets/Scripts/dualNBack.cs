using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class dualNBack : MonoBehaviour
{
    private Sprite[] allImages;
    private int showingImageCount = 2;
    private List<Sprite> generatedImages = new List<Sprite>();
    private List<Sprite> showingImages = new List<Sprite>();
    private List<string> shownIndices = new List<string>();
    private List<string> tempShownIndices = new List<string>();
    public GameObject blocks;

    private int currentIndex = 0;
    public TextMeshProUGUI countdownDisplay;
    private bool isLevelCompleted = false;
    public int countdowmTime;
    private int initialCountdownTime = 0;
    public Slider progressBar;
    private int succesCount = 0;
    private int failCount = 0;
    public GameObject succedDialog;
    public TextMeshProUGUI levelTime;
    private int levelTimeCountdown;
    private bool isCheckingInput = false;
    public GameObject options;
    private bool isSound = false;
    private List<AudioClip> playedSounds = new List<AudioClip>();
    private List<AudioClip> generatedSounds = new List<AudioClip>();
    public AudioSource audioSource;
    public AudioClip[] allSounds;

    private void Start()
    {
        changeOptionsInteraction(false);
        StartCoroutine(CountdownToStart());

    }

    private void Update()
    {
        // Add any update logic you need here.
    }



    private void ChangeImageColor(GameObject imageObject, Color color)
    {
        Image imageComponent = imageObject.GetComponent<Image>();

        if (imageComponent != null)
        {
            imageComponent.color = color;
        }
    }


    private bool IsSoundOrDifferent()
    {
        return Random.Range(0, 2) == 0;
    }

    private IEnumerator ReadyToStart()
    {
        isLevelCompleted = false;
        countdowmTime = 2;

        currentIndex = 0;
        isLevelCompleted = false;
        while (countdowmTime > 0)
        {
            yield return new WaitForSeconds(0.75f);
            countdowmTime--;

            if (isLevelCompleted)
            {
                yield break;
            }
        }
        changeOptionsInteraction(false);
        checkSuccesRate(false);
    }


    public void EnteredOption(string optionName)
    {
         changeOptionsInteraction(false);
            isLevelCompleted = true;

            if (optionName == "Ses")
            {
                bool isSuccess = isSound ? true : false;
                checkSuccesRate(isSuccess);
            }
            if (optionName == "Pozisyon")
            {
                bool isSuccess = !isSound ? true : false;
                checkSuccesRate(isSuccess);
            }
    }
    private void checkSuccesRate(bool isSucces)
    {
        TextMeshProUGUI succedDialogText = succedDialog.GetComponent<TextMeshProUGUI>();
        if (isSucces)
        {
            succesCount++;
            succedDialogText.text = ":)";
            succedDialog.SetActive(true);

            showingImageCount = 1;
            GameStartRules(true);
        }
        else
        {
            VibrateForDuration(0.5f);
            failCount++;
            succedDialogText.text = "";
            succedDialog.SetActive(true);
            showingImageCount = 1;
            GameStartRules(false);
        }
        // Calculate success rate
        float successRate = (float)succesCount / (succesCount + failCount);

        // Set progressBar.value based on success rate
        progressBar.value = successRate;
        StartCoroutine(HideSuccessDialog(2.0f));
    }

    private void GameStartRules(bool isSucces)
    {

        showingImages.Clear();
        playedSounds.Clear();
        currentIndex = 0;
    }

    IEnumerator HideSuccessDialog(float delay)
    {
        yield return new WaitForSeconds(delay);
        succedDialog.SetActive(false);
        StartCoroutine(StartStep(showingImageCount));
    }
    private IEnumerator LevelTimeStart()
    {

        levelTimeCountdown = 90;
        levelTime.text = levelTimeCountdown.ToString();
        levelTime.gameObject.SetActive(true);
        while (levelTimeCountdown > 0)
        {
            levelTime.text = levelTimeCountdown.ToString();
            yield return new WaitForSeconds(1);
            levelTimeCountdown--;
        }
    }
    private IEnumerator CountdownToStart()
    {
        blocks.SetActive(false);
        initialCountdownTime = countdowmTime;
        countdownDisplay.gameObject.SetActive(true);
        while (initialCountdownTime > 0)
        {
            countdownDisplay.text = initialCountdownTime.ToString();
            yield return new WaitForSeconds(1);
            initialCountdownTime--;
        }

        countdownDisplay.text = "Başlayalım...";
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);

        blocks.SetActive(true);
        StartCoroutine(LevelTimeStart());
        StartCoroutine(StartStep(showingImageCount));
    }

    private IEnumerator StartStep(int showingBlockCount)
    {

        isCheckingInput = false;
        allSounds = Resources.LoadAll<AudioClip>("NumberSounds");
        Image[] allImages = blocks.GetComponentsInChildren<Image>();
        isSound = IsSoundOrDifferent();
        for (int i = 0; i < showingBlockCount; i++)
        {
            int randomIndex;
            AudioClip randomSound;

            if (isSound)
            {
                if (shownIndices.Count > 0)
                {
                    randomIndex = int.Parse(shownIndices[shownIndices.Count - 1]);
                }
                else
                {
                    do
                    {
                        randomIndex = Random.Range(0, allImages.Length);
                    } while (shownIndices.Contains(randomIndex.ToString()));
                }
                do
                {
                    randomSound = allSounds[Random.Range(0, allSounds.Length)];
                } while (generatedSounds.Contains(randomSound));

                Debug.Log("Loaded sound name: " + generatedSounds.Count + ": " + randomSound.name);
                yield return null;
            }
            else
            {
                if (generatedSounds.Count > 0)
                {
                    randomSound = generatedSounds[generatedSounds.Count - 1];
                }
                else
                {
                    do
                    {
                        randomSound = allSounds[Random.Range(0, allSounds.Length)];
                    } while (generatedSounds.Contains(randomSound));
                }
                do
                {
                    randomIndex = Random.Range(0, allImages.Length);
                } while (shownIndices.Contains(randomIndex.ToString()));
            }
            playedSounds.Add(randomSound);
            generatedSounds.Add(randomSound);
            shownIndices.Add(randomIndex.ToString());

            AudioClip currentSound = playedSounds[playedSounds.Count - 1];
            audioSource.clip = currentSound;
            Image currentImage = allImages[randomIndex];
            Color originalColor = currentImage.GetComponent<Image>().color;

            currentImage.GetComponent<Image>().color = Color.green;
            audioSource.Play();
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            currentImage.GetComponent<Image>().color = originalColor;
            yield return new WaitForSeconds(1.0f);
        }
        tempShownIndices = shownIndices;
        changeOptionsInteraction(true);
        StartCoroutine(ReadyToStart());
    }
    private void changeOptionsInteraction(bool isInteractable)
    {
        Button[] allOptions = options.GetComponentsInChildren<Button>();

        foreach (Button option in allOptions)
        {
            option.interactable = isInteractable;
        }
    }
    
      void VibrateForDuration(float duration)
    {
        // Android ve iOS'da titreşim desteklenir
        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            // Eğer belirli bir süre boyunca titreşim istiyorsanız, Coroutine kullanabilirsiniz
            StartCoroutine(StopVibrationAfterDelay(duration));
        #else
            Debug.LogWarning("Titreşim sadece Android ve iOS platformlarında desteklenir.");
        #endif
    }

    IEnumerator StopVibrationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Handheld.Vibrate(); // Vibration stops after the specified delay
    }
}
