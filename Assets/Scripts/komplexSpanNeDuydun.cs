using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class komplexSpanNeDuydun : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public TextMeshProUGUI levelTime;
    private int levelTimeCountdown;
    public GameObject showingNumberObject;

    public GameObject Digitcircle;
    public GameObject succedDialog;

    public GameObject imageDigitCirclePrefab;

    private int succesCount = 0;

    private int failCount = 0;

    private int showingNumberCount = 2;
    private int currentIndex = 0;
    private bool isCheckingInput = false;


    private List<int> generatedNumbers = new List<int>();

    private List<int> enteredNumbers = new List<int>();

    public TextMeshProUGUI countdownDisplay;

    public int countdowmTime;

    private int initialCountdownTime = 0;

    public Slider progressBar;
    public bool isBackward;

    private ParticleSystem particleSystem;
    private bool isLevelCompleted = false;
    private int listeningAudioCount = 1;
    private List<AudioClip> playedSounds = new List<AudioClip>();
    public AudioClip[] allSounds;
    private string[] soundNames;

    private Sprite[] allImages;
    public GameObject options;
    public Button optionPrefab;
    public Button repeatButton;
    public Button deleteButton;
    public Button checkAnswerButton;
    private List<string> enteredOptions = new List<string>();
    public AudioSource audioSource;

    private void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    void Update()
    {
        repeatButton.gameObject.SetActive(progressBar.value == 0.75f && failCount == 0);
    }

    public void Repeat()
    {
        CheckSuccessRate(false);
    }



    private void CreateOptions(int numberOfElements)
    {
        allImages = Resources.LoadAll<Sprite>("Images");
        numberOfElements = Mathf.Min(numberOfElements * 2, 8);
        GridLayoutGroup optionsGridLayoutGroup = options.GetComponentInChildren<GridLayoutGroup>();

        Sprite[] generatedOptionImages = SetOptions(numberOfElements);

        for (int i = 0; i < numberOfElements; i++)
        {
            Button option = Instantiate(optionPrefab, options.transform);
            option.transform.localPosition = new Vector3(i * 2, 0, 0);
            option.GetComponent<Image>().sprite = generatedOptionImages[i];
            option.name = generatedOptionImages[i].name;
            option.onClick.AddListener(() => EnteredOption(option.name));
        }
    }

    private Sprite[] SetOptions(int numberOfElements)
    {
        List<Sprite> generatedOptionImages = new List<Sprite>();
        int nonAnswerImages = numberOfElements - playedSounds.Count;

        List<string> imagesOfPlayedSounds = new List<string>();

        foreach (AudioClip audio in playedSounds)
        {
            Sprite playedSoundImage = allImages.FirstOrDefault(image => image.name == audio.name);

            if (playedSoundImage != null)
            {
                generatedOptionImages.Add(playedSoundImage);
            }
        }

        while (nonAnswerImages > 0)
        {
            Sprite randomImage;
            do
            {
                randomImage = allImages[Random.Range(0, allImages.Length)];
            } while (generatedOptionImages.Contains(randomImage));

            generatedOptionImages.Add(randomImage);
            nonAnswerImages--;
        }

        ShuffleList1(generatedOptionImages);

        return generatedOptionImages.ToArray();
    }

    void ShuffleList1(List<Sprite> array)
    {
        for (int i = array.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Sprite temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    public void EnteredOption(string optionName)
    {

        if (isCheckingInput)
        {
            if (optionName == "Delete")
            {
                currentIndex = (currentIndex == 0) ? currentIndex : currentIndex - 1;
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;

                newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.white);
                enteredOptions.RemoveAt(currentIndex);
                changeOptionsInteraction(true);


            }
            else if (optionName == "CheckAnswer")
            {

                options.SetActive(false);
                makeButtonsVisible(false);
                List<string> playedSoundsName = playedSounds.Select(option => option.name).ToList();
                bool isSuccess = IsListEqual(enteredOptions, playedSoundsName);
                CheckSuccessRate(isSuccess);

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
    }


    bool IsListEqual(List<string> list1, List<string> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
            {
                return false;
            }
        }

        return true;
    }


    IEnumerator HideSuccessDialog(float delay)
    {
        yield return new WaitForSeconds(delay);
        succedDialog.SetActive(false);
        StartCoroutine(CorrectValueChanger());
    }

    private void CheckSuccessRate(bool isSucces)
    {
        levelTime.gameObject.SetActive(false);
        TextMeshProUGUI succedDialogText = succedDialog.GetComponent<TextMeshProUGUI>();
        if (isSucces)
        {
            failCount = 0;
            succesCount++;
            succedDialogText.text = "Bravo :)";
            succedDialog.SetActive(true);
            progressBar.value += 0.25f;
            if (succesCount == 4)
            {
                progressBar.value = 0;
                showingNumberCount++;

            }

        }
        else
        {
            succesCount = 0;
            failCount++;
            succedDialogText.text = "Tekrar Deneyelim :(";
            progressBar.value = 0;
            succedDialog.SetActive(true);
            if (failCount == 4 && showingNumberCount > 2)
            {
                showingNumberCount--;
            }
        }
        isLevelCompleted = true;
        GameStartRules();
        StartCoroutine(HideSuccessDialog(2.0f));
    }

    private void GameStartRules()
    {
        makeButtonsVisible(false);
        foreach (Transform child in options.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in Digitcircle.transform)
        {
            Destroy(child.gameObject);
        }

        generatedNumbers.Clear();
        playedSounds.Clear();
        enteredOptions.Clear();
        enteredNumbers.Clear();
        currentIndex = 0;
    }
    private IEnumerator CorrectValueChanger()
    {
        isCheckingInput = false;
        yield return new WaitForSeconds(1);
        int minNumber = 1;
        int maxNumber = 9;
        int totalCount = showingNumberCount + listeningAudioCount;
        CreateElementsInDigitCircle(listeningAudioCount);

        while (generatedNumbers.Count < showingNumberCount)
        {
            int randomNum;
            do
            {
                randomNum = Random.Range(minNumber, maxNumber + 1);
            } while (generatedNumbers.Contains(randomNum));

            generatedNumbers.Add(randomNum);
            Debug.Log("Rastgele Sayı " + generatedNumbers.Count + ": " + randomNum);
            yield return null; // Bu satır eklendi
        }

        while (currentIndex < generatedNumbers.Count)
        {
            showingNumberObject.SetActive(true);
            Text.text = generatedNumbers[currentIndex].ToString();
            // GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
            // ChangeImageColor(newImage, Color.green);
            currentIndex++;
            yield return new WaitForSeconds(1);
        }

        showingNumberObject.SetActive(false);
        yield return new WaitForSeconds(1);
        StartCoroutine(PlayNumberofSounds());



    }

     private IEnumerator PlayNumberofSounds()
    {
        isCheckingInput = false;
        allSounds = Resources.LoadAll<AudioClip>("Sounds");
        // Sprite speakerImage = Resources.Load<Sprite>("speaker");
        // CreateElementsInDigitCircle(listeningAudioCount);
        currentIndex = 0;
        if (allSounds != null && allSounds.Length > 0)
        {
            while (playedSounds.Count < listeningAudioCount)
            {
                AudioClip randomSound;
                do
                {
                    randomSound = allSounds[Random.Range(0, allSounds.Length)];
                } while (playedSounds.Contains(randomSound));

                playedSounds.Add(randomSound);
                Debug.Log("Loaded sound name: " + playedSounds.Count + ": " + randomSound.name);
                yield return null;
            }

            // showingSpeakerObject.GetComponent<Image>().sprite = speakerImage;
            // showingSpeakerObject.SetActive(true);

            while (currentIndex < playedSounds.Count)
            {
                AudioClip currentSound = playedSounds[currentIndex];
                audioSource.clip = currentSound;
                audioSource.Play();
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.green);
                yield return new WaitForSeconds(3);
                audioSource.Stop();

                currentIndex++;
                yield return new WaitForSeconds(1);
            }

            // showingSpeakerObject.SetActive(false);

            if (isBackward)
            {
                playedSounds.Reverse();
            }

            StartCoroutine(ReadyToStart());
        }
        else
        {
            Debug.LogError("No sounds found in the 'Sounds' folder in Resources.");
        }
    }


    private void CreateElementsInDigitCircle(int numberOfElements)
    {
        for (int i = 0; i < numberOfElements; i++)
        {
            GameObject newImage = Instantiate(imageDigitCirclePrefab, Digitcircle.transform);

            newImage.transform.localPosition = new Vector3(i * 2, 0, 0);

            newImage.name = "ImageElement" + i;
        }
    }

    private void ChangeImageColor(GameObject imageObject, Color color)
    {
        Image imageComponent = imageObject.GetComponent<Image>();

        if (imageComponent != null)
        {
            imageComponent.color = color;
        }
    }

    private IEnumerator ReadyToStart()
    {
        isCheckingInput = true;
        int startCount = 2;
        countdownDisplay.gameObject.SetActive(true);
        ResetDigitCircleColor();
        while (startCount > 0)
        {
            countdownDisplay.text = startCount.ToString();
            yield return new WaitForSeconds(1);
            startCount--;
        }
        if (isBackward)
        {
            StartCoroutine(ActivateRotation());
            yield return new WaitForSeconds(1.5f);
        }
        countdownDisplay.text = "Hazır Ol...";
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);


        CreateOptions(listeningAudioCount);
        currentIndex = 0;
        options.SetActive(true);
        isLevelCompleted = false;
        StartCoroutine(LevelTimeStart());
    }

    private void ResetDigitCircleColor()
    {
        GameObject[] circles = Digitcircle.GetComponentsInChildren<Transform>(true)
            .Where(t => t != Digitcircle.transform)
            .Select(t => t.gameObject)
            .ToArray();
        foreach (GameObject circle in circles)
        {
            ChangeImageColor(circle, Color.white);
        }
    }
    private IEnumerator CountdownToStart()
    {
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
        StartCoroutine(CorrectValueChanger());
    }

    private IEnumerator LevelTimeStart()
    {
        makeButtonsVisible(true);
        levelTimeCountdown = showingNumberCount * 3;
        levelTime.text = levelTimeCountdown.ToString();
        levelTime.gameObject.SetActive(true);
        while (levelTimeCountdown > 0)
        {
            levelTime.text = levelTimeCountdown.ToString();
            yield return new WaitForSeconds(1);
            levelTimeCountdown--;
            if (isLevelCompleted)
            {
                yield break; // Coroutine'i sonlandır
            }
        }
        makeButtonsVisible(false);
        options.SetActive(false);
        levelTime.gameObject.SetActive(false);
        countdownDisplay.gameObject.SetActive(true);
        countdownDisplay.text = "Süre Doldu...";
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);
        CheckSuccessRate(false);
    }

    private void CheckBackward()
    {
        if (isBackward)
        {
            generatedNumbers.Reverse();
        }
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

        private void changeOptionsInteraction(bool isInteractable)
    {
        Button[] allOptions = options.GetComponentsInChildren<Button>();

        foreach (Button option in allOptions)
        {
            option.interactable = isInteractable;
        }
    }
    private void makeButtonsVisible(bool isVisible)
    {
        deleteButton.gameObject.SetActive(isVisible);
        checkAnswerButton.gameObject.SetActive(isVisible);
    }
}
