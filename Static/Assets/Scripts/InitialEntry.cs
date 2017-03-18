using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialEntry : MonoBehaviour {

    public Initial[] initials;
    Initial ActiveInitial
    {
        get
        {
            return initials[activeInitalIndex];
        }
    }
    char[] letters = { '_', 'A', 'B', 'C', 'D', 'E', 'F', 'G' }; // All the letters my babies can display.
    int activeInitalIndex = 0;   // The initial which is currently being controlled by the player.

    public Color activeColor;   // The color of the letter when it is active.
    public Color inactiveColor;

    float keyCooldown = 0.1f;   // How often a keypress is registered.
    float sinceLastKeypress = 0f;


    void Start()
    {
        ActiveInitial.Active = true;
    }

	
	void Update ()
    {
        sinceLastKeypress += Time.deltaTime;

        /* PLAYER CONTROL */

        if (sinceLastKeypress >= keyCooldown)
        {
            // If the player pressed up or down, change the character of the active initial.
            if (Input.GetAxisRaw("Vertical") == -1)
            {
                ActiveInitial.charIndex--;
                if (ActiveInitial.charIndex < 0) ActiveInitial.charIndex = letters.Length - 1;
                UpdateActiveInitialChar();
            }

            else if (Input.GetAxisRaw("Vertical") == 1)
            {
                ActiveInitial.charIndex++;
                if (ActiveInitial.charIndex > letters.Length - 1) ActiveInitial.charIndex = 0;
                UpdateActiveInitialChar();
            }

            // If the player pressed a vertical direction, switch active letter.
            else if (Input.GetAxisRaw("Horizontal") == -1)
            {
                ActiveInitial.Active = false;
                activeInitalIndex--;
                if (activeInitalIndex < 0) activeInitalIndex = initials.Length - 1;
                ActiveInitial.Active = true;
            }

            else if (Input.GetAxisRaw("Horizontal") == 1)
            {
                ActiveInitial.Active = false;
                activeInitalIndex++;
                if (activeInitalIndex > initials.Length - 1) activeInitalIndex = 0;
                ActiveInitial.Active = true;
            }

            sinceLastKeypress = 0f;
        }

	}


    void UpdateActiveInitialChar()
    {
        ActiveInitial.SetChar(letters[ActiveInitial.charIndex]);
    }
}
