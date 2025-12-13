using System.Collections.Generic;
using UnityEngine;

public class CatController : NPCController
{
    [SerializeField]
    private List<Node> sequenceNodes;
    [SerializeField]
    private List<Node> selectorNodes;
    void Start()
    {
        rootNode = new Selector(new List<Node>())
        {

        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
