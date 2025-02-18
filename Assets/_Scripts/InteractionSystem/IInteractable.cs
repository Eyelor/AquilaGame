using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string InteractionPrompt { get; set; }
    public bool Interact(Interactor interactor);
    public string GetPrompt(string name);
}
