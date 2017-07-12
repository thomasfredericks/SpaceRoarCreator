using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : MonoBehaviour {

    public Text textUI;
    const int DNA_LENGTH = 6;
    char[] dna;
    public char emptyCharSymbol = '_';
    int textLength = 0;
    public SpaceShipGeneratorScript spaceShipGeneratorScript;

    SpaceShip ship;

    // Use this for initialization
    void Start () {
        dna = new char[DNA_LENGTH];
        PadDna();
        
    }
	
	// Update is called once per frame
	void Update () {

       
        if (Input.inputString.Length > 0 )
        {
            foreach (char c in Input.inputString)
            {
                
                // BACKSPACE
                if (c == "\b"[0])
                {
                    if (textLength != 0)
                    {
                        textLength--;

                        if (ship != null)
                        {
                            Destroy(ship.main);
                            ship = null;
                        }
                    }
                }
                else if ( (c > 64 && c < 91) || (c > 96 && c < 123) )
                {

                    char capticalChar = c;

                    if (capticalChar > 96) capticalChar = (char)(capticalChar - 32);

                    if (textLength < DNA_LENGTH)
                    {
                        dna[textLength] = capticalChar;
                        textLength++;

                        if (textLength == DNA_LENGTH)
                        {
                            
                            if (ship != null)
                            {
                                Destroy(ship.main);
                                ship = null;
                            }

                            ship = spaceShipGeneratorScript.GenerateShip(dna);
                           
                        }
                    }

                    
                }
            }


            PadDna();

            
               
        }
       
    }

    void PadDna()
    {
        

        for ( int i = textLength; i < DNA_LENGTH; i++ )
        {
            
            dna[i] = emptyCharSymbol;

        }
        textUI.text = new string(dna);
    }

 

  
}
