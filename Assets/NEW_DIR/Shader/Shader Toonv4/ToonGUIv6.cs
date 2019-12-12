//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System;

//public class ToonGUIv6 : ShaderGUI
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
//            MaterialProperty _Alpha = ShaderGUI.FindProperty("_Alpha", properties);
//            materialEditor.ShaderProperty(_Alpha, _Alpha.displayName);

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

//        MaterialProperty _UseDissolve = ShaderGUI.FindProperty("_UseDissolve", properties);
//        materialEditor.ShaderProperty(_UseDissolve, _UseDissolve.displayName);

//        if (_UseDissolve.floatValue == 1)
//        {
//            MaterialProperty _DissolveTexture = ShaderGUI.FindProperty("_DissolveTexture", properties);
//            materialEditor.ShaderProperty(_DissolveTexture, _DissolveTexture.displayName);

//            MaterialProperty _Strength = ShaderGUI.FindProperty("_Strength", properties);
//            materialEditor.ShaderProperty(_Strength, _Strength.displayName);

//            MaterialProperty _DisOutline = ShaderGUI.FindProperty("_DisOutline", properties);
//            materialEditor.ShaderProperty(_DisOutline, _DisOutline.displayName);

//            MaterialProperty _OutlineStrength = ShaderGUI.FindProperty("_OutlineStrength", properties);
//            materialEditor.ShaderProperty(_OutlineStrength, _OutlineStrength.displayName);
//        }
//    }
//}
