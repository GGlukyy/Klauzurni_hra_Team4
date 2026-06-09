using UnityEngine;
using UnityEngine.Playables;

public class ButtonScript : MonoBehaviour
{
    private Animator animator;
    private PlayableDirector director;
    private bool isPressed = false; // чтобы не нажать дважды

    void Start()
    {
        animator = GetComponent<Animator>();
        director = GetComponent<PlayableDirector>();
    }

    public void OnButtonPress()
    {
        if (isPressed) return; // блокируем повторное нажатие

        isPressed = true;
        animator.SetTrigger("Press"); // запускаем анимацию кнопки
        director.Play();              // запускаем открытие ворот
    }
}