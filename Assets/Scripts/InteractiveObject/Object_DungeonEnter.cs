using UnityEngine;

public class Object_DungeonEnter : MonoBehaviour
{
    public UI ui;

    private void Start()
    {
        ui = Player.instance.ui;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && ui != null)
        {
            ui.OpenDungeonUI();
        }
    }
}
