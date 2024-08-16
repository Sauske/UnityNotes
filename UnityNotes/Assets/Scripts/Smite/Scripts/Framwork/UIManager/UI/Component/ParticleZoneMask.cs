//using UnityEngine;
//using System.Collections;
//using UnityEngine.UI;
//using Assets.Scripts.UI;

//public class ParticleZoneMask :  Mask
//{
//    public float minX = 0f;
//    public float maxX = 1f;
//    public float minY = 0f;
//    public float maxY = 1f;

//    private bool m_bInit = false;

//    public void CalcRect()
//    {
//        if (m_bInit) return;

//        Camera CuiCamera = CUIManager.instance.FormCamera;

//        if (CuiCamera != null && rectTransform != null)
//        {
//            Vector3[] corners = new Vector3[4];
//            rectTransform.GetWorldCorners(corners);
//            Vector3[] scrennPostion =
//            {
//                CuiCamera.WorldToScreenPoint(corners[0]),
//                CuiCamera.WorldToScreenPoint(corners[2])
//            };

//            float screenWidth = Screen.width;
//            float screenHeight = Screen.height;

//            minX = scrennPostion[0].x / screenWidth;
//            maxX = scrennPostion[1].x / screenWidth;
//            minY = scrennPostion[0].y / screenHeight;
//            maxY = scrennPostion[1].y / screenHeight;

//            //ParticleSystem[] particlesSystems = transform.GetComponentsInChildren<ParticleSystem>();

//            //foreach (ParticleSystem particleSystem in particlesSystems)
//            //{
//            //    particleSystem.renderer.sharedMaterial.SetFloat("_MinX", minX);
//            //    particleSystem.renderer.sharedMaterial.SetFloat("_MinY", minY);
//            //    particleSystem.renderer.sharedMaterial.SetFloat("_MaxX", maxX);
//            //    particleSystem.renderer.sharedMaterial.SetFloat("_MaxY", maxY);
//            //}

//            m_bInit = true;
//        }
//    }
//    protected override void Start()
//    {
//        base.Start();
//        CalcRect();
//    }
//    public override string ToString()
//    {
//        return string.Format("minx = {0} , maxx = {1} , miny = {2} , maxy = {3}" , minX , maxX , minY , maxY);
//    }
//}
