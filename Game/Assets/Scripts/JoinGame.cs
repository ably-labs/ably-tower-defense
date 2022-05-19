using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class JoinGame : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

    [SerializeField]
    public TMP_InputField gameIDField;

    void Start()
    {
        startButton.onClick.AddListener(() => {
            if (gameIDField.text == "") return;
            StateManagerBehavior.Instance.GameID = gameIDField.text;
            SceneManager.LoadScene("GameScene");
        });
    }
}
