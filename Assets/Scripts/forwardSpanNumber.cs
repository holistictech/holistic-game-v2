using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


namespace CasualA.Board
{
    public class forwardSpanNumber : MonoBehaviour
    {
        public TextMeshProUGUI Text;
        public TextMeshProUGUI levelTime;
        public TextMeshProUGUI countdownDisplay;

        public int countdownTime;
        public bool isBackward;

        public GameObject showingNumberObject;
        public GameObject options;
        public GameObject digitCircle;
        public GameObject succeedDialog;
        public GameObject imageDigitCirclePrefab;

        public Button repeatButton;
        public Button optionPrefab;
        public Button checkAnswerButton;
        public Button deleteButton;

        public Slider progressBar;


        private int successCount = 0;
        private int levelTimeCountdown;
        private int failCount = 0;
        private int showingNumberCount = 2;
        private int currentIndex = 0;
        private int minNumber = 1;
        private int lastRandomNum = -1;

        private bool isCheckingInput = false;
        private bool isLevelCompleted = false;
        private bool isRepeat = false;
        private bool isRepeated = false;
        private bool showRepeat = true;

        private List<int> generatedNumbers = new List<int>();
        private List<int> enteredNumbers = new List<int>();

        private ParticleSystem particleSystem;


        private void Start()
        {
            StartCoroutine(CountdownToStart());
        }

        private void Update()
        {
            checkAnswerButton.interactable = (enteredNumbers.Count == generatedNumbers.Count);
            repeatButton.gameObject.SetActive(progressBar.value == 0.75f && failCount == 0 && showRepeat);
        }

        public void Repeat()
        {
            isRepeat = true;
            showRepeat = false;
            levelTime.gameObject.SetActive(false);
            CheckSuccessRate(false);
        }

        public void Num(int num)
        {
            if (isCheckingInput == true)
            {
                if (num == -1)
                {
                    currentIndex = (currentIndex == 0) ? currentIndex : currentIndex - 1;
                    GameObject newImage = digitCircle.transform.GetChild(currentIndex).gameObject;
                    ChangeImageColor(newImage, Color.white);
                    enteredNumbers.RemoveAt(currentIndex);
                }
                else if (num == -2)
                {
                    options.SetActive(false);
                    makeButtonsVisible(false);
                    bool isSuccess = IsListEqual(enteredNumbers, generatedNumbers);
                    CheckSuccessRate(isSuccess);
                }
                else
                {
                    Debug.Log("Entered Number: " + num);
                    GameObject newImage = digitCircle.transform.GetChild(currentIndex).gameObject;
                    enteredNumbers.Add(num);
                    currentIndex++;
                    ChangeImageColor(newImage, Color.green);
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
            succeedDialog.SetActive(false);
            StartCoroutine(CorrectValueChanger());
        }

        private void CheckSuccessRate(bool isSuccess)
        {
            levelTime.gameObject.SetActive(false);
            TextMeshProUGUI succeedDialogText = succeedDialog.GetComponent<TextMeshProUGUI>();

            if (progressBar.value == 0.75f && failCount == 0 && isRepeat)
            {
                failCount++;
                succeedDialogText.text = "Bir şans daha";
                succeedDialog.SetActive(true);
                isRepeat = false;
            } else
            {
                if (isSuccess)
                {
                    if (failCount == 0 || isRepeated) successCount++;
                    succeedDialogText.text = "Bravo :)";
                    succeedDialog.SetActive(true);
                    if (failCount == 0 || isRepeated) progressBar.value += 0.25f;

                    if (failCount == 1) {
                        isRepeated = true;
                    }

                    if (isRepeated)
                    {
                        isRepeated = false;
                        failCount = 0;
                    }

                    if (successCount == 4)
                    {
                        progressBar.value = 0;
                        successCount = 0;
                        failCount = 0;
                        showingNumberCount++;
                        showRepeat = true;
                    }
                }
                else
                {
                    successCount = 0;
                    failCount++;
                    succeedDialogText.text = "Tekrar Deneyelim :(";
                    progressBar.value = 0;
                    succeedDialog.SetActive(true);
                    if (failCount == 4 && showingNumberCount > 2)
                    {
                        failCount = 0;
                        showingNumberCount--;
                        showRepeat = true;
                    }
                }
            }

            isLevelCompleted = true;
            options.SetActive(false);
            GameStartRules(isRepeat);
            StartCoroutine(HideSuccessDialog(2.0f));
        }

        private void GameStartRules(bool isRepeat)
        {
            options.SetActive(false);
            makeButtonsVisible(false);

            foreach (Transform child in options.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in digitCircle.transform)
            {
                Destroy(child.gameObject);
            }

            if (!isRepeat) generatedNumbers.Clear();
            enteredNumbers.Clear();
            currentIndex = 0;
        }

        private IEnumerator CorrectValueChanger()
        {
            isCheckingInput = false;
            yield return new WaitForSeconds(1.0f);
            CreateElementsInDigitCircle(showingNumberCount);

            while (generatedNumbers.Count < showingNumberCount)
            {
                int randomNum;
                do
                {
                    randomNum = UnityEngine.Random.Range(minNumber, 10);
                } while (generatedNumbers.Contains(randomNum) || IsConsecutive(lastRandomNum, randomNum));

                generatedNumbers.Add(randomNum);
                lastRandomNum = randomNum;
                yield return null;
            }

            Debug.Log("Generated Numbers: " + string.Join(", ", generatedNumbers));

            while (currentIndex < generatedNumbers.Count)
            {
                showingNumberObject.SetActive(true);
                Text.text = generatedNumbers[currentIndex].ToString();

                GameObject newImage = digitCircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.green);

                currentIndex++;
                yield return new WaitForSeconds(0.5f);

                showingNumberObject.SetActive(false);

                yield return new WaitForSeconds(1.0f);
            }

            showingNumberObject.SetActive(false);
            CheckBackward();

            StartCoroutine(ReadyToStart());
        }

        bool IsConsecutive(int lastNum, int newNum)
        {
            if (showingNumberCount == 2) return (Mathf.Abs(lastNum - newNum) == 1);
            else return (Mathf.Abs(lastNum - newNum) == 1 || Mathf.Abs(lastNum - newNum) == 2);
        }

        private void CreateElementsInDigitCircle(int numberOfElements)
        {
            for (int i = 0; i < numberOfElements; i++)
            {
                GameObject newImage = Instantiate(imageDigitCirclePrefab, digitCircle.transform);

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
            countdownDisplay.gameObject.SetActive(true);
            ResetDigitCircleColor();

            if (isBackward)
            {
                StartCoroutine(ActivateRotation());
                yield return new WaitForSeconds(1.5f);
            }

            countdownDisplay.text = "Sıra sende..";

            yield return new WaitForSeconds(1.0f);

            countdownDisplay.gameObject.SetActive(false);
            options.SetActive(true);

            CreateOptions(showingNumberCount);
            currentIndex = 0;
            isLevelCompleted = false;
            StartCoroutine(LevelTimeStart());
        }

        private void ResetDigitCircleColor()
        {
            GameObject[] circles = digitCircle.GetComponentsInChildren<Transform>(true)
                .Where(t => t != digitCircle.transform)
                .Select(t => t.gameObject)
                .ToArray();

            foreach (GameObject circle in circles)
            {
                ChangeImageColor(circle, Color.white);
            }
        }

        private void CreateOptions(int numberOfElements)
        {
            numberOfElements = Mathf.Min(numberOfElements * 2, 8);
            GridLayoutGroup optionsGridLayoutGroup = options.GetComponentInChildren<GridLayoutGroup>();

            if (numberOfElements <= 4)
            {
                optionsGridLayoutGroup.cellSize = new Vector2(200, 200);
                optionsGridLayoutGroup.spacing = new Vector2(100, 100);
            }
            if (numberOfElements >= 6)
            {
                optionsGridLayoutGroup.spacing = new Vector2(50, 50);
            }
            if (numberOfElements >= 8)
            {
                optionsGridLayoutGroup.cellSize = new Vector2(150, 150);
            }

            List<int> allNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            List<int> numbersToAdd = allNumbers.Except(generatedNumbers).ToList();

            // Shuffle the numbersToAdd list
            ShuffleList(numbersToAdd);

            int addingNumber = numberOfElements - generatedNumbers.Count;
            Debug.Log("numberOfElements: " + numberOfElements);
            Debug.Log("generatedNumbers.Count: " + generatedNumbers.Count);

            if (generatedNumbers.Count >= 4)
            {
                addingNumber = 5;
            }

            List<int> optionsNumbers = generatedNumbers.Concat(numbersToAdd.Take(addingNumber)).ToList();

            optionsNumbers.Sort();

            foreach (int number in optionsNumbers)
            {
                Button option = Instantiate(optionPrefab, options.transform);
                option.name = number.ToString();
                option.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
                option.onClick.AddListener(() => Num(int.Parse(option.name)));
            }
        }

        private void ShuffleList(List<int> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        private IEnumerator CountdownToStart()
        {
            options.SetActive(false);
            countdownDisplay.gameObject.SetActive(true);

            countdownDisplay.text = isBackward ? "BackwardSpanNumber'a Hoşgeldiniz!" : "ForwardSpanNumber'a Hoşgeldiniz!";
            yield return new WaitForSeconds(1.0f);

            countdownDisplay.text = "Başlayalım...";
            yield return new WaitForSeconds(1.0f);

            countdownDisplay.gameObject.SetActive(false);
            progressBar.gameObject.SetActive(true);

            StartCoroutine(CorrectValueChanger());
        }

        private IEnumerator LevelTimeStart()
        {
            makeButtonsVisible(true);

            levelTimeCountdown = (showingNumberCount * 3) + 2;
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
            countdownDisplay.gameObject.SetActive(true);
            options.SetActive(false);
            makeButtonsVisible(false);

            countdownDisplay.text = "Süre Doldu...";
            yield return new WaitForSeconds(1);
            countdownDisplay.gameObject.SetActive(false);

            bool isSuccess = IsListEqual(enteredNumbers, generatedNumbers);
            CheckSuccessRate(isSuccess);
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
            Quaternion startRotation = digitCircle.transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0, 0, startRotation.eulerAngles.z + 180.0f);

            while (rotationTime < rotationDuration)
            {
                rotationTime += Time.deltaTime;
                digitCircle.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationTime / rotationDuration);
                yield return null;
            }

            digitCircle.transform.rotation = startRotation;
        }

        private void makeButtonsVisible(bool isVisible)
        {
            deleteButton.gameObject.SetActive(isVisible);
            checkAnswerButton.gameObject.SetActive(isVisible);
        }
    }
}
