using UnityEngine;

namespace AdmurinArts
{
    public class NPCTest : MonoBehaviour
{
    [SerializeField] private Animator[] NpcAnims;

    public void Animation_1_Idle()
    {
        for (int i = 0; i < NpcAnims.Length; i++)
        {
            if (NpcAnims[i].gameObject.activeSelf == true)
            {
                NpcAnims[i].SetBool("Run", false);
                Debug.Log("The NPC " + NpcAnims[i].gameObject.name + " is Idling");
            }
        }
    }
    public void Animation_2_Run()
    {
        for (int i = 0; i < NpcAnims.Length; i++)
        {
            if (NpcAnims[i].gameObject.activeSelf == true)
            {
                NpcAnims[i].SetBool("Run", true);
                Debug.Log("The NPC " + NpcAnims[i].gameObject.name + " is Running");

            }
        }
    }
    public void Animation_3_Ability()
    {
        for (int i = 0; i < NpcAnims.Length; i++)
        {
            if (NpcAnims[i].gameObject.activeSelf == true)
            {
                NpcAnims[i].SetBool("Run", false);
                NpcAnims[i].SetBool("Ability", true);
                Debug.Log("The NPC " + NpcAnims[i].gameObject.name + " is using its First Ability");
            }
        }
    }
    public void Animation_4_Other()
    {
        for (int i = 0; i < NpcAnims.Length; i++)
        {
            if (NpcAnims[i].gameObject.activeSelf == true)
            {
                NpcAnims[i].SetBool("Run", false);
                NpcAnims[i].SetTrigger("Other");
                Debug.Log("The NPC " + NpcAnims[i].gameObject.name + " is performing an action");
            }
        }
    }
}
}

