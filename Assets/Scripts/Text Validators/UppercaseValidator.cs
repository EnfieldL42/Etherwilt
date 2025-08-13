using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "UppercaseValidator", menuName = "TextMeshPro/Uppercase Validator")]
public class UppercaseValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        // Force the character to uppercase
        ch = char.ToUpper(ch);

        // Insert into the string
        text = text.Insert(pos, ch.ToString());
        pos++;

        return ch;
    }
}
