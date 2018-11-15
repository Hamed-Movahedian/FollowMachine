using System;
using Bind;
using BindEditor.Internal;

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

        public static GetValue CreateGetValue(Type parameterType)
        {
            var getValue = new GetValue(parameterType);

            getValue.ValueType = 
                ConstValueGUI.IsSupported(parameterType) ? 
                    GetValue.ValueTypes.Const : 
                    GetValue.ValueTypes.Bind;

            return getValue;
        }
    }
}
