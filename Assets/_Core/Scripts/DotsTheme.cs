using UnityEngine;

namespace DotsClone {
    [CreateAssetMenu]
    public class DotsTheme : ScriptableObject {
        public static readonly DotsTheme defaultTheme = CreateInstance<DotsTheme>();

        public Color backgroundColor = Color.white;
        public Color dotA = new Color32(235, 93, 70, 255);
        public Color dotB = new Color32(138, 188, 255, 255);
        public Color dotC = new Color32(138, 233, 145, 255);
        public Color dotD = new Color32(153, 93, 181, 255);
        public Color dotE = new Color32(229, 219, 37, 255);

        public Color FromDotType(DotType type) {
            switch(type) {
                case DotType.Cleared:
                    return Color.clear;
                case DotType.DotA:
                    return dotA;
                case DotType.DotB:
                    return dotB;
                case DotType.DotC:
                    return dotC;
                case DotType.DotD:
                    return dotD;
                case DotType.DotE:
                    return dotE;
                default:
                    return Color.white; // Easy to notice invalid behaviour
            }
        }
    }
}