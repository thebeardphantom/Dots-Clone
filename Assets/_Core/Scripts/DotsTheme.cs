using UnityEngine;

namespace DotsClone {
    [CreateAssetMenu]
    public class DotsTheme : ScriptableObject {
        public static readonly DotsTheme defaultTheme = CreateInstance<DotsTheme>();

        public Color dotA = new Color32(235, 93, 70, 255);
        public Color dotB = new Color32(138, 188, 255, 255);
        public Color dotC = new Color32(138, 233, 145, 255);
        public Color dotD = new Color32(153, 93, 181, 255);
        public Color dotE = new Color32(229, 219, 37, 255);

        public Color FromDotType(Dot.Type type) {
            switch(type) {
                case Dot.Type.Cleared:
                    return Color.clear;
                case Dot.Type.DotA:
                    return dotA;
                case Dot.Type.DotB:
                    return dotB;
                case Dot.Type.DotC:
                    return dotC;
                case Dot.Type.DotD:
                    return dotD;
                case Dot.Type.DotE:
                    return dotE;
                default:
                    return Color.grey; // Easy to notice invalid behaviour
            }
        }
    }
}