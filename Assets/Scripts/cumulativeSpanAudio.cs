using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class CumulativeSpanAudios : MonoBehaviour
{
    public Slider progressBar;

    public TextMeshProUGUI levelTime;
    public TextMeshProUGUI countdownDisplay;

    public Button optionPrefab;
    public Button deleteButton;
    public Button checkAnswerButton;

    public GameObject showingImageObject;
    public GameObject successionDialog;
    public GameObject options;
    public GameObject imageDigitCirclePrefab;
    public GameObject Digitcircle;

    public AudioSource audioSource;

    public AudioClip[] allSounds;

    public int countdownTime = 1;

    public bool isBackward;



    private List<AudioClip> playedSounds = new List<AudioClip>();
    private List<Sprite> optionImages = new List<Sprite>();
    private List<string> enteredOptions = new List<string>();
    private List<string> playedSoundsNames = new List<string>();


    private Sprite[] allImages;

    private bool isLevelCompleted = false;

    private int level = 1;
    private int initialCountdownTime = 0;
    private int currentIndex = 0;
    private int succesCount = 0;
    private int failCount = 0;
    private int levelTimeCountdown;


    private void Start()
    {
        StartCoroutine(ReadyForStart());
    }

    private void Update()
    {
        checkAnswerButton.interactable = (playedSounds.Count == enteredOptions.Count);
    }

    private IEnumerator ReadyForStart()
    {
        Debug.Log(level);
        progressBar.gameObject.SetActive(false);
        makeButtonsVisible(false);
        initialCountdownTime = countdownTime;
        countdownDisplay.gameObject.SetActive(true);

        var messages = new[] {
    new { Text = !isBackward ? "Kümülatif Audio Forward Span'e hoşgeldiniz!" : "Kümülatif Audio Backward Span'e hoşgeldiniz!", Time = 1f },
    new { Text = "Önünüze gelecek resimleri kümülatif bir şekilde seçmeniz gerekiyor.", Time = 1f },
    new { Text = "Hazırsan başlayalım!", Time = 1f },
};

        foreach (var message in messages)
        {
            countdownDisplay.text = message.Text;
            yield return new WaitForSeconds(message.Time);
        }

        while (initialCountdownTime > 0)
        {
            countdownDisplay.text = initialCountdownTime.ToString();
            yield return new WaitForSeconds(1);
            initialCountdownTime--;
        }

        if (isBackward)
        {
            StartCoroutine(ActivateRotation());
            yield return new WaitForSeconds(1.5f);
        }

        countdownDisplay.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);

        StartCoroutine(PlaySound());
    }

    private IEnumerator ActivateRotation()
    {
        float rotationDuration = 1.5f;
        float rotationTime = 0.0f;
        Quaternion startRotation = Digitcircle.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, startRotation.eulerAngles.z + 180.0f);

        while (rotationTime < rotationDuration)
        {
            rotationTime += Time.deltaTime;
            Digitcircle.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationTime / rotationDuration);
            yield return null;
        }

        Digitcircle.transform.rotation = startRotation;
    }

    private IEnumerator PlaySound()
    {
        allSounds = Resources.LoadAll<AudioClip>("Sounds");
        Sprite speakerImage = Resources.Load<Sprite>("speaker");
        GameObject circleToHighlight;

        if (allSounds == null || allSounds.Length == 0)
        {
            Debug.LogError("No sound found in the 'Sounds' folder in Resources.");
            yield return null;
        }
        CreateElementsInDigitCircle(level);

        if (level == 4 && succesCount == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                AudioClip newSound = SelectUniqueRandomSound();
                if (newSound == null)
                {
                    Debug.LogError("No more unique images available.");
                    yield break;
                }
                circleToHighlight = Digitcircle.transform.GetChild(i).gameObject;
                Debug.Log("Highlighting circle: " + circleToHighlight.name);
                StartCoroutine(ChangeColorTemporarily(circleToHighlight));

                showingImageObject.GetComponent<Image>().sprite = speakerImage;
                showingImageObject.SetActive(true);

                audioSource.clip = newSound;
                audioSource.Play();

                while (audioSource.isPlaying)
                {
                    yield return null;
                }

                showingImageObject.SetActive(false);

                playedSounds.Add(newSound);
                playedSoundsNames.Add(newSound.name);
                Debug.Log("Played sound: " + newSound.name + " as, " + playedSounds.Count + ".");

                yield return new WaitForSeconds(1f);
            }
        }
        else
        {
            AudioClip newSound = SelectUniqueRandomSound();
            if (newSound == null)
            {
                Debug.LogError("No more unique sounds available.");
                yield break;
            }


            yield return new WaitForSeconds(1f);

            circleToHighlight = Digitcircle.transform.GetChild(level - 1).gameObject;
            Debug.Log("Highlighting circle: " + circleToHighlight.name);
            StartCoroutine(ChangeColorTemporarily(circleToHighlight));

            showingImageObject.GetComponent<Image>().sprite = speakerImage;
            showingImageObject.SetActive(true);

            audioSource.clip = newSound;
            audioSource.Play();

            while (audioSource.isPlaying)
            {
                yield return null;
            }

            showingImageObject.SetActive(false);

            playedSounds.Add(newSound);
            playedSoundsNames.Add(newSound.name);
            Debug.Log("Played sound: " + newSound.name + " as, " + playedSounds.Count + ".");

            yield return new WaitForSeconds(1f);
        }
        StartCoroutine(AskQuestionAndStartGame());
    }

    private AudioClip SelectUniqueRandomSound()
    {
        List<AudioClip> availableSounds = allSounds.Except(playedSounds).ToList();

        if (availableSounds.Count == 0)
        {
            return null;
        }

        return availableSounds[Random.Range(0, availableSounds.Count)];
    }

    private IEnumerator AskQuestionAndStartGame()
    {
        countdownDisplay.gameObject.SetActive(true);
        countdownDisplay.text = (level == 1) ? "Hangi sesi duyuyorsunuz?" : "Hangi ses eklendi?";

        if (isBackward && level != 1)
        {
            StartCoroutine(ActivateRotation());
        }
        yield return new WaitForSeconds(1.5f);

        countdownDisplay.gameObject.SetActive(false);

        makeButtonsVisible(true);

        CreateOptions();

        currentIndex = 0;

        isLevelCompleted = false;

        StartCoroutine(LevelTimeStart());
    }

    private void CreateElementsInDigitCircle(int numberOfElements)
    {
        for (int i = 0; i < numberOfElements; i++)
        {
            GameObject newSound = Instantiate(imageDigitCirclePrefab, Digitcircle.transform);

            newSound.transform.localPosition = new Vector3(i * 2, 0, 0);

            newSound.name = "ImageElement" + i;
        }
    }

    private void CreateOptions()
    {
        allImages = Resources.LoadAll<Sprite>("Animals");

        SetOptions();

        foreach (Transform child in options.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < optionImages.Count; i++)
        {
            Button option = Instantiate(optionPrefab, options.transform);
            option.transform.localPosition = new Vector3(i * 2, 0, 0);
            option.GetComponent<Image>().sprite = optionImages[i];
            option.name = optionImages[i].name;

            int index = i;
            option.onClick.AddListener(() => EnteredOption(optionImages[index].name));
        }
    }

    private void SetOptions()
    {
        optionImages.Clear();

        int nonAnswerImages = 9 - playedSounds.Count;

        List<string> imagesOfPlayedSounds = new List<string>();

        foreach (AudioClip audio in playedSounds)
        {
            Sprite playedSoundImage = allImages.FirstOrDefault(image => image.name == audio.name);

            if (playedSoundImage != null)
            {
                optionImages.Add(playedSoundImage);
            }
        }

        while (nonAnswerImages > 0)
        {
            Sprite randomImage;

            do
            {
                randomImage = allImages[Random.Range(0, allImages.Length)];
            } while (optionImages.Contains(randomImage));

            optionImages.Add(randomImage);
            nonAnswerImages--;
        }

        ShuffleList(optionImages);
    }

    private void ShuffleList(List<Sprite> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            Sprite temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private IEnumerator LevelTimeStart()
    {
        levelTimeCountdown = (playedSounds.Count * 3) + 2;
        levelTime.text = levelTimeCountdown.ToString();
        levelTime.gameObject.SetActive(true);

        while (levelTimeCountdown > 0)
        {
            levelTime.text = levelTimeCountdown.ToString();
            yield return new WaitForSeconds(1);
            levelTimeCountdown--;

            if (isLevelCompleted)
            {
                yield break;
            }
        }

        levelTime.gameObject.SetActive(false);


        countdownDisplay.text = "Süre doldu...";
        countdownDisplay.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        countdownDisplay.gameObject.SetActive(false);

        EnteredOption("checkAnswer");
    }

    public void EnteredOption(string optionName)
    {
        if (optionName == "delete")
        {
            currentIndex = (currentIndex == 0) ? currentIndex : currentIndex - 1;
            GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;

            ChangeImageColor(newImage, Color.white);
            enteredOptions.RemoveAt(currentIndex);
            changeOptionsInteraction(true);
        }
        else if (optionName == "checkAnswer")
        {
            makeButtonsVisible(false);

            bool isSuccess = IsListEqual(enteredOptions, playedSoundsNames);

            checkSuccess(isSuccess);
        }
        else
        {
            GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;

            enteredOptions.Add(optionName);
            currentIndex++;
            ChangeImageColor(newImage, Color.green);

            if (currentIndex == playedSounds.Count)
            {
                changeOptionsInteraction(false);
            }
        }
    }

    private void changeOptionsInteraction(bool isInteractable)
    {
        Button[] allOptions = options.GetComponentsInChildren<Button>();

        foreach (Button option in allOptions)
        {
            option.interactable = isInteractable;
        }
    }

    bool IsListEqual(List<string> list1, List<string> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        if (isBackward)
        {
            list2.Reverse();
        }

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
            {
                return false;
            }
        }

        if (isBackward)
        {
            list2.Reverse();
        }

        return true;
    }

    private void checkSuccess(bool isSucces)
    {
        levelTime.gameObject.SetActive(false);
        TextMeshProUGUI successionDialogText = successionDialog.GetComponent<TextMeshProUGUI>();

        if (isSucces)
        {
            level++;
            failCount = 0;
            succesCount++;

            successionDialogText.text = "Bravo :)";
            successionDialog.SetActive(true);

            progressBar.value += 0.111f;
        }
        else
        {
            if (level >= 4)
            {
                progressBar.value = 0.45f;
                level = 4;

                successionDialogText.text = "Bilemediniz, 4. seviyeden devam edelim..";
            }
            else
            {
                level = 1;
                progressBar.value = 0;

                successionDialogText.text = "Bilemediniz, baştan başlayalım..";
            }

            succesCount = 0;
            failCount++;
            playedSounds.Clear();
            playedSoundsNames.Clear();
            successionDialog.SetActive(true);
        }

        isLevelCompleted = true;

        ReadyNextLevel();
        StartCoroutine(ShowSuccessDialog(2.0f));
    }

    private void ReadyNextLevel()
    {
        makeButtonsVisible(false);

        foreach (Transform child in Digitcircle.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in options.transform)
        {
            Destroy(child.gameObject);
        }

        enteredOptions.Clear();
        currentIndex = 0;
    }

    IEnumerator ShowSuccessDialog(float delay)
    {
        yield return new WaitForSeconds(delay);

        successionDialog.SetActive(false);

        StartCoroutine(PlaySound());
    }

    private void ChangeImageColor(GameObject imageObject, Color color)
    {
        Image imageComponent = imageObject.GetComponent<Image>();

        if (imageComponent != null)
        {
            imageComponent.color = color;
        }
    }

    private IEnumerator ChangeColorTemporarily(GameObject gameObject)
    {
        ChangeImageColor(gameObject, Color.green);
        yield return new WaitForSeconds(1f);
        ChangeImageColor(gameObject, Color.white);
    }

    private void makeButtonsVisible(bool isVisible)
    {
        options.SetActive(isVisible);
        deleteButton.gameObject.SetActive(isVisible);
        checkAnswerButton.gameObject.SetActive(isVisible);
    }
}
