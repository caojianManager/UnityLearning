// ///<summary>
// ///ClassName:TextComponentExtension.cs
// ///Explain:--------------
// ///Author:Cao Jian
// ///Date:2022-06-29
// ///</summary>

using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent.CVRPageViewController
{
    public class TextComponentExtension : MonoBehaviour
    {
        public static float GetTextWidth(Text text)
        {
            var rectExtents = text.cachedTextGenerator.rectExtents.size * 0.5f;
            var generationSettings = text.GetGenerationSettings(rectExtents);
            var width = text.cachedTextGeneratorForLayout.GetPreferredWidth(text.text, generationSettings);
            return width;
        }
    }
}