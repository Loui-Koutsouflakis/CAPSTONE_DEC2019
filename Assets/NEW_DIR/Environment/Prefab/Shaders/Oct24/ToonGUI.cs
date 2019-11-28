//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System;

//public class ToonGUI : ShaderGUI
//{
//    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
//    {
//        MaterialProperty _UseOutline = ShaderGUI.FindProperty("_UseOutline", properties);
//        materialEditor.ShaderProperty(_UseOutline, _UseOutline.displayName);

//        if (_UseOutline.floatValue == 1)
//        {
//            MaterialProperty _OutlineColor = ShaderGUI.FindProperty("_OutlineColor", properties);
//            materialEditor.ShaderProperty(_OutlineColor, _OutlineColor.displayName);

//            MaterialProperty _OutlineWidth = ShaderGUI.FindProperty("_OutlineWidth", properties);
//            materialEditor.ShaderProperty(_OutlineWidth, _OutlineWidth.displayName);
//        }

//        MaterialProperty _UseToon = ShaderGUI.FindProperty("_UseToon", properties);
//        materialEditor.ShaderProperty(_UseToon, _UseToon.displayName);

//        if (_UseToon.floatValue == 1)
//        {
//            MaterialProperty _Ramp = ShaderGUI.FindProperty("_Ramp", properties);
//            materialEditor.ShaderProperty(_Ramp, _Ramp.displayName);

//            MaterialProperty _MainTex = ShaderGUI.FindProperty("_MainTex", properties);
//            materialEditor.ShaderProperty(_MainTex, _MainTex.displayName);

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

//        MaterialProperty _UseNormalMap = ShaderGUI.FindProperty("_UseNormalMap", properties);
//        materialEditor.ShaderProperty(_UseNormalMap, _UseNormalMap.displayName);

//        if (_UseNormalMap.floatValue == 1)
//        {
//            MaterialProperty _NormalMap = ShaderGUI.FindProperty("_NormalMap", properties);
//            materialEditor.ShaderProperty(_NormalMap, _NormalMap.displayName);

//            MaterialProperty _NormalMapIntensity = ShaderGUI.FindProperty("_NormalMapIntensity", properties);
//            materialEditor.ShaderProperty(_NormalMapIntensity, _NormalMapIntensity.displayName);
//        }

//        MaterialProperty _UseEmission = ShaderGUI.FindProperty("_UseEmission", properties);
//        materialEditor.ShaderProperty(_UseEmission, _UseEmission.displayName);

//        if (_UseEmission.floatValue == 1)
//        {
//            MaterialProperty _EmissionMap = ShaderGUI.FindProperty("_EmissionMap", properties);
//            materialEditor.ShaderProperty(_EmissionMap, _EmissionMap.displayName);

//            MaterialProperty _EmissionColor = ShaderGUI.FindProperty("_EmissionColor", properties);
//            materialEditor.ShaderProperty(_EmissionColor, _EmissionColor.displayName);

//            MaterialProperty _EmissionIntensity = ShaderGUI.FindProperty("_EmissionIntensity", properties);
//            materialEditor.ShaderProperty(_EmissionIntensity, _EmissionIntensity.displayName);
//        }

//        MaterialProperty _UseSnow = ShaderGUI.FindProperty("_UseSnow", properties);
//        materialEditor.ShaderProperty(_UseSnow, _UseSnow.displayName);

//        if (_UseSnow.floatValue == 1)
//        {
//            MaterialProperty _SnowText = ShaderGUI.FindProperty("_SnowText", properties);
//            materialEditor.ShaderProperty(_SnowText, _SnowText.displayName);

//            MaterialProperty _Factor = ShaderGUI.FindProperty("_Factor", properties);
//            materialEditor.ShaderProperty(_Factor, _Factor.displayName);

//            MaterialProperty _ObjYScale = ShaderGUI.FindProperty("_ObjYScale", properties);
//            materialEditor.ShaderProperty(_ObjYScale, _ObjYScale.displayName);
//        }
//    }
//}
