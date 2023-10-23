using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CasualA.Board
{
    public class numPad : MonoBehaviour
    {
        public TextMeshProUGUI Text;
        public TextMeshProUGUI levelTime;
        private int levelTimeCountdown;
        public GameObject showingNumberObject;
        public GameObject numpad;

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

        private ParticleSystem particleSystem;
        private bool isLevelCompleted = false;

        private void Start()
        {
            StartCoroutine(CountdownToStart());
        }

        public void Num(int num)
        {
            if (isCheckingInput && currentIndex < generatedNumbers.Count)
            {
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                TextMeshProUGUI textElement = newImage.GetComponentInChildren<TextMeshProUGUI>();
                if (num == -1)
                {
                    currentIndex = (currentIndex == 0) ? currentIndex : currentIndex - 1;
                    newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                    textElement = newImage.GetComponentInChildren<TextMeshProUGUI>();
                    ChangeImageColor(newImage, Color.white);
                    textElement.text = "";
                    enteredNumbers.RemoveAt(currentIndex);


                }
                else
                {
                    textElement.text = num.ToString();
                    enteredNumbers.Add(num);
                    currentIndex++;
                    ChangeImageColor(newImage, Color.green);
                    if (currentIndex == generatedNumbers.Count)
                    {
                        numpad.SetActive(false);
                        bool isSuccess = IsListEqual(enteredNumbers, generatedNumbers);
                        checkSuccesRate(isSuccess);
                    }
                }




            }
        }

        bool IsListEqual(List<int> list1, List<int> list2)
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

        private void checkSuccesRate(bool isSucces)
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
            numpad.SetActive(false);
            GameStartRules();
            StartCoroutine(HideSuccessDialog(2.0f));
        }

        private void GameStartRules()
        {
            foreach (Transform child in Digitcircle.transform)
            {
                Destroy(child.gameObject);
            }
            generatedNumbers.Clear();
            enteredNumbers.Clear();
            currentIndex = 0;
        }
        private IEnumerator CorrectValueChanger()
        {
            yield return new WaitForSeconds(1);
            int minNumber = 1;
            int maxNumber = 9;
            showingNumberObject.SetActive(true);

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
                Text.text = generatedNumbers[currentIndex].ToString();
                currentIndex++;
                yield return new WaitForSeconds(1);
            }

            showingNumberObject.SetActive(false);
            StartCoroutine(ReadyToStart());

        

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
            int startCount = 2;
            countdownDisplay.gameObject.SetActive(true);
            while (startCount > 0)
            {
                countdownDisplay.text = startCount.ToString();
                yield return new WaitForSeconds(1);
                startCount--;
            }

            countdownDisplay.text = "Hazır Ol...";
            yield return new WaitForSeconds(1);
            countdownDisplay.gameObject.SetActive(false);
            isCheckingInput = true;
            numpad.SetActive(true);
            CreateElementsInDigitCircle(generatedNumbers.Count);
            currentIndex = 0;
            isLevelCompleted = false;
            StartCoroutine(LevelTimeStart());
        }

        private IEnumerator CountdownToStart()
        {
            numpad.SetActive(false);
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
            levelTime.gameObject.SetActive(false);
            countdownDisplay.gameObject.SetActive(true);
            countdownDisplay.text = "Süre Doldu...";
            yield return new WaitForSeconds(1);
            countdownDisplay.gameObject.SetActive(false);
            GameStartRules();
            StartCoroutine(CountdownToStart());
        }
    }


}
