using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdmurinArts 
{ 
    public class NPCSelector : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button[] allAnimationButtons;
    [SerializeField] private Button[] enableAnimationButtons;

    [Header("Game Objects")]
    [SerializeField] private GameObject[] allNpcGOs;
    [SerializeField] private GameObject enableNpc;

    [Header("Text")]
    [SerializeField] private Text npcName;

    public void Start()
    {
        npcName.text = "PRESS THE YELLOW BUTTONS";
    }
    public void ChangeNpcs()
    {
        _DisableAllNpcs();
        _DisableAllButtons();
        _EnableNpc();
        _EnableButton();
        _Rename();
    }

    public void _Rename()
    {
        npcName.text = enableNpc.gameObject.name;
    }

    public void _EnableButton()
    {
        for (int i = 0; i < enableAnimationButtons.Length; i++)
        {
            enableAnimationButtons[i].interactable = true;
        }
    }
    public void _DisableAllButtons()
    {
        for (int i = 0; i < allAnimationButtons.Length; i++)
        {
            allAnimationButtons[i].interactable = false;
        }
    }

    public void _EnableNpc()
    {
        enableNpc.SetActive(true);
    }
    public void _DisableAllNpcs()
    {
        for (int i = 0; i < allNpcGOs.Length; i++)
        {
            allNpcGOs[i].gameObject.SetActive(false);
        }
    }
}
}
    