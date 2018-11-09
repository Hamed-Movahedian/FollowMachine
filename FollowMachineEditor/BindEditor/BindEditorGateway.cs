using Bind;

namespace BindEditor
{
    public static class BindEditorGateway
    {
        public static void AssinmentGUI(this Assinment bindUnit)
        {
            Internal.BindEditorUtility.AssinmentGUI(bindUnit);
        }

        public static void GetValueGUI(this GetValue value)
        {
            Internal.BindEditorUtility.GetValueGUI(value);
        }
    }
}
