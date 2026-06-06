using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoaderScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Animator animatorText;

    [SerializeField] private float transitionTime = 2.0f;

    private void Start()
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        if (index == 0)
        {
            animatorText.gameObject.SetActive(false);
            animator.gameObject.SetActive(true);
        }
        else if (index == 1)
        {
            animator.gameObject.SetActive(false);
            animatorText.gameObject.SetActive(true);
        }
        else if (index == 2)
        {
            animatorText.gameObject.SetActive(false);
            animator.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        EndCubeTouched.CubeEntered += LoadNextLevel;
    }

    private void OnDisable()
    {
        EndCubeTouched.CubeEntered -= LoadNextLevel;
    }

    public void LoadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(LoadScene(nextIndex));
    }

    private IEnumerator LoadScene(int levelIndex)
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentIndex == 0)
        {
            animator.gameObject.SetActive(false);
            animatorText.gameObject.SetActive(true);
            animatorText.SetTrigger("Start");
        }
        else if (currentIndex == 1)
        {
            animatorText.gameObject.SetActive(false);
            animator.gameObject.SetActive(true);
            animator.SetTrigger("Start");
        }

        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}