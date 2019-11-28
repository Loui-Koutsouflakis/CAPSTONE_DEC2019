//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System;

//public class VegeGUI : ShaderGUI
//{
//    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
//    {
//        MaterialProperty _UseVege = ShaderGUI.FindProperty("_UseVege", properties);
//        materialEditor.ShaderProperty(_UseVege, _UseVege.displayName);

//        if (_UseVege.floatValue == 1)
//        {
//            MaterialProperty _TopColor = ShaderGUI.FindProperty("_TopColor", properties);
//            materialEditor.ShaderProperty(_TopColor, _TopColor.displayName);

//            MaterialProperty _BottomColor = ShaderGUI.FindProperty("_BottomColor", properties);
//            materialEditor.ShaderProperty(_BottomColor, _BottomColor.displayName);

//            MaterialProperty _GradThresh = ShaderGUI.FindProperty("_GradThresh", properties);
//            materialEditor.ShaderProperty(_GradThresh, _GradThresh.displayName);

//            MaterialProperty _BendRotationRandom = ShaderGUI.FindProperty("_BendRotationRandom", properties);
//            materialEditor.ShaderProperty(_BendRotationRandom, _BendRotationRandom.displayName);

//            MaterialProperty _BladeForward = ShaderGUI.FindProperty("_BladeForward", properties);
//            materialEditor.ShaderProperty(_BladeForward, _BladeForward.displayName);

//            MaterialProperty _BladeCurve = ShaderGUI.FindProperty("_BladeCurve", properties);
//            materialEditor.ShaderProperty(_BladeCurve, _BladeCurve.displayName);

//            MaterialProperty _BladeHeight = ShaderGUI.FindProperty("_BladeHeight", properties);
//            materialEditor.ShaderProperty(_BladeHeight, _BladeHeight.displayName);

//            MaterialProperty _BladeHeightRandom = ShaderGUI.FindProperty("_BladeHeightRandom", properties);
//            materialEditor.ShaderProperty(_BladeHeightRandom, _BladeHeightRandom.displayName);

//            MaterialProperty _BladeWidth = ShaderGUI.FindProperty("_BladeWidth", properties);
//            materialEditor.ShaderProperty(_BladeWidth, _BladeWidth.displayName);

//            MaterialProperty _BladeWidthRandom = ShaderGUI.FindProperty("_BladeWidthRandom", properties);
//            materialEditor.ShaderProperty(_BladeWidthRandom, _BladeWidthRandom.displayName);

//            MaterialProperty _WindDistortionMap = ShaderGUI.FindProperty("_WindDistortionMap", properties);
//            materialEditor.ShaderProperty(_WindDistortionMap, _WindDistortionMap.displayName);

//            MaterialProperty _WindFrequency = ShaderGUI.FindProperty("_WindFrequency", properties);
//            materialEditor.ShaderProperty(_WindFrequency, _WindFrequency.displayName);

//            MaterialProperty _WindStrength = ShaderGUI.FindProperty("_WindStrength", properties);
//            materialEditor.ShaderProperty(_WindStrength, _WindStrength.displayName);

//            MaterialProperty _TessellationUniform = ShaderGUI.FindProperty("_TessellationUniform", properties);
//            materialEditor.ShaderProperty(_TessellationUniform, _TessellationUniform.displayName);
//        }

//        MaterialProperty _UseToon = ShaderGUI.FindProperty("_UseToon", properties);
//        materialEditor.ShaderProperty(_UseToon, _UseToon.displayName);

//        if (_UseToon.floatValue == 1)
//        {
//            MaterialProperty _Color = ShaderGUI.FindProperty("_Color", properties);
//            materialEditor.ShaderProperty(_Color, _Color.displayName);

//            MaterialProperty _AmbientColor = ShaderGUI.FindProperty("_AmbientColor", properties);
//            materialEditor.ShaderProperty(_AmbientColor, _AmbientColor.displayName);

//            MaterialProperty _SpecularColor = ShaderGUI.FindProperty("_SpecularColor", properties);
//            materialEditor.ShaderProperty(_SpecularColor, _SpecularColor.displayName);

//            MaterialProperty _Glossiness = ShaderGUI.FindProperty("_Glossiness", properties);
//            materialEditor.ShaderProperty(_Glossiness, _Glossiness.displayName);

//            MaterialProperty _RimColor = ShaderGUI.FindProperty("_RimColor", properties);
//            materialEditor.ShaderProperty(_RimColor, _RimColor.displayName);

//            MaterialProperty _RimAmount = ShaderGUI.FindProperty("_RimAmount", properties);
//            materialEditor.ShaderProperty(_RimAmount, _RimAmount.displayName);

//            MaterialProperty _RimThreshold = ShaderGUI.FindProperty("_RimThreshold", properties);
//            materialEditor.ShaderProperty(_RimThreshold, _RimThreshold.displayName);
//        }
//    }
//}
