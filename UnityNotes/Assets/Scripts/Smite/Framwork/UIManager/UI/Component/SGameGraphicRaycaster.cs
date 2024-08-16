//==================================================================================
/// 优化后的UI输入检测
/// 
/// *约定:
/// *需要使用CUIEventScript、CUIMiniEventScript或CUIJoystick接收输入事件的节点，必须带有Image组件
/// *输入事件不再进行冒泡
/// *不带CUIEventScript、CUIMiniEventScript或CUIJoystick，仅带有Image组件的节点，不能阻挡输入事件
/// 
/// @lukechen
/// @2015.08.27
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Framework
{
    public class SGameGraphicRaycaster : GraphicRaycaster
    {
        //是否忽略scrollRect
        public bool ignoreScrollRect = true;

        public enum RaycastMode
        {
            Unity,
            Sgame,
            Sgame_tile,
        };

        struct Coord
        {
            public int x;
            public int y;
            public int numX;
            public int numY;

            public static Coord Invalid = new Coord() { x = -1, y = -1 };

            public bool IsValid()
            {
                return x >= 0 && y >= 0;
            }

            public bool Equals(ref Coord r)
            {
                return r.x == x && r.y == y && r.numX == numX && r.numY == numY;
            }
        };

        class Item
        {
            public Image m_image;
            public RectTransform m_rectTransform;
            public Coord m_coord = Coord.Invalid;

            public static Item Create(Image image)
            {
                if (image == null)
                {
                    return null;
                }

                Item item = new Item();
                item.m_image = image;
                item.m_rectTransform = image.gameObject.transform as RectTransform;

                return item;
            }

            public void Raycast(List<Graphic> raycastResults, Vector2 pointerPosition, Camera eventCamera)
            {
                if (m_image.enabled
                && m_rectTransform.gameObject.activeInHierarchy
                && RectTransformUtility.RectangleContainsScreenPoint(m_rectTransform, pointerPosition, eventCamera)
                )
                {
                    //if (image.Raycast(pointerPosition, eventCamera))
                    {
                        raycastResults.Add(m_image);
                    }
                }
            }
        };

        class Tile
        {
            public ListView<Item> items = new ListView<Item>();
        };

        Canvas canvas_;
        Canvas canvas
        {
            get
            {
                if (canvas_ != null)
                    return canvas_;

                canvas_ = GetComponent<Canvas>();
                return canvas_;
            }
        }

        ListView<Item> m_allItems = new ListView<Item>();

        const int TileCount = 4;

        Tile[] m_tiles = null;

        int m_tileSizeX = 0;
        int m_tileSizeY = 0;

        int m_screenWidth = 0;
        int m_screenHeight = 0;

        Vector3[] corners = new Vector3[4];

        [HideInInspector, System.NonSerialized]
        public bool tilesDirty = false;

        public RaycastMode raycastMode = RaycastMode.Sgame;

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            switch (raycastMode)
            {
                case RaycastMode.Unity:
                    {
                      //  SProfiler.BeginSample("Raycast_Unity");

                        base.Raycast(eventData, resultAppendList);

                     //   SProfiler.EndSample();
                    }
                    break;

                case RaycastMode.Sgame:
                    {
                      //  SProfiler.BeginSample("Raycast_Sgame");

                        Raycast2(eventData, resultAppendList, false);

                     //   SProfiler.EndSample();
                    }
                    break;

                case RaycastMode.Sgame_tile:
                    {
                   //     SProfiler.BeginSample("Raycast_Sgame_tile");

                        Raycast2(eventData, resultAppendList, true);

                   //     SProfiler.EndSample();
                    }
                    break;
            }
        }

        void CalcItemCoord(ref Coord coord, Item item)
        {
            item.m_rectTransform.GetWorldCorners(corners);

            int xMin = int.MaxValue;
            int xMax = int.MinValue;
            int yMin = int.MaxValue;
            int yMax = int.MinValue;

            var canvasCamera = canvas.worldCamera;

            for (int i = 0; i < corners.Length; ++i)
            {
                Vector3 v = CUIUtility.WorldToScreenPoint(canvasCamera, corners[i]);

                xMin = Mathf.Min((int)v.x, xMin);
                xMax = Mathf.Max((int)v.x, xMax);
                yMin = Mathf.Min((int)v.y, yMin);
                yMax = Mathf.Max((int)v.y, yMax);
            }

            coord.x = Mathf.Clamp(xMin / m_tileSizeX, 0, TileCount - 1);
            coord.numX = Mathf.Clamp(xMax / m_tileSizeX, 0, TileCount - 1) - coord.x + 1;
            coord.y = Mathf.Clamp(yMin / m_tileSizeY, 0, TileCount - 1);
            coord.numY = Mathf.Clamp(yMax / m_tileSizeY, 0, TileCount - 1) - coord.y + 1;
        }

        void AddToTileList(Item item)
        {
            int tileStart = item.m_coord.x + item.m_coord.y * TileCount;
            for (int x = 0; x < item.m_coord.numX; ++x)
            {
                for (int y = 0; y < item.m_coord.numY; ++y)
                {
                    int index = y * TileCount + x + tileStart;
                    m_tiles[index].items.Add(item);
                }
            }
        }

        void RemoveFromTileList(Item item)
        {
            if (item.m_coord.IsValid())
            {
                int tileStart = item.m_coord.x + item.m_coord.y * TileCount;
                for (int x = 0; x < item.m_coord.numX; ++x)
                {
                    for (int y = 0; y < item.m_coord.numY; ++y)
                    {
                        int index = y * TileCount + x + tileStart;
                        m_tiles[index].items.Remove(item);
                    }
                }

                item.m_coord = Coord.Invalid;
            }
        }

        public void InitTiles()
        {
            if (m_tiles != null)
                return;

            m_tiles = new Tile[TileCount * TileCount];
            for (int i = 0; i < m_tiles.Length; ++i)
            {
                m_tiles[i] = new Tile();
            }

            m_screenWidth = Screen.width;
            m_screenHeight = Screen.height;

            m_tileSizeX = m_screenWidth / TileCount;
            m_tileSizeY = m_screenHeight / TileCount;
        }

        void UpdateTiles_Editor()
        {
            if ((m_screenWidth == Screen.width && m_screenHeight == Screen.height)
            || m_tiles == null
            || raycastMode != RaycastMode.Sgame_tile
            )
            {
                return;
            }

            m_screenWidth = Screen.width;
            m_screenHeight = Screen.height;

            m_tileSizeX = m_screenWidth / TileCount;
            m_tileSizeY = m_screenHeight / TileCount;

            for (int i = 0; i < m_tiles.Length; ++i)
            {
                m_tiles[i].items.Clear();
            }

            for (int i = 0; i < m_allItems.Count; ++i)
            {
                var item = m_allItems[i];
                CalcItemCoord(ref item.m_coord, item);
                AddToTileList(item);
            }
        }

        public void UpdateTiles()
        {
            if (raycastMode != RaycastMode.Sgame_tile)
                return;

            Coord coord = Coord.Invalid;

            for (int i = 0; i < m_allItems.Count; ++i)
            {
                var item = m_allItems[i];

                CalcItemCoord(ref coord, item);

                if (!coord.Equals(ref item.m_coord))
                {
                    RemoveFromTileList(item);

                    item.m_coord = coord;
                    AddToTileList(item);
                }
            }
        }

        //真机上也跑下Update逻辑，防止有些机器弹出虚拟按键的时候会导致分辨率发生改变<neoyang>
        //#if UNITY_EDITOR || UNITY_STANDALONE
        void Update()
        {
            UpdateTiles_Editor();
        }
        //#endif

        protected override void Start()
        {
            base.Start();

            //优化模式下初始化Items
            InitializeAllItems();
        }

        private void InitializeAllItems()
        {
            if (!(raycastMode == RaycastMode.Sgame || raycastMode == RaycastMode.Sgame_tile))
            {
                return;
            }

            m_allItems.Clear();

            Image[] images = this.gameObject.GetComponentsInChildren<Image>(true);

            for (int i = 0; i < images.Length; i++)
            {
                if (IsGameObjectHandleInput(images[i].gameObject))
                {
                    Item item = Item.Create(images[i]);
                    if (item != null)
                    {
                        m_allItems.Add(item);
                    }
                }
            }

            if (raycastMode == RaycastMode.Sgame_tile)
            {
                InitTiles();

                for (int i = 0; i < m_allItems.Count; ++i)
                {
                    var item = m_allItems[i];
                    CalcItemCoord(ref item.m_coord, item);
                    AddToTileList(item);
                }
            }
        }

        public void RemoveGameObject(GameObject go)
        {
            if (go == null || m_allItems == null)
            {
                return;
            }

            Image image = go.GetComponent<Image>();
            if (image != null)
            {
                if (IsGameObjectHandleInput(go))
                {
                    RemoveItem(image);
                }
            }
        }

        public void RemoveItem(Image image)
        {
            if (image == null || m_allItems == null)
            {
                return;
            }

            for (int i = 0; i < m_allItems.Count; i++)
            {
                if (m_allItems[i].m_image == image)
                {
                    if (this.raycastMode == RaycastMode.Sgame_tile)
                    {
                        RemoveFromTileList(m_allItems[i]);
                    }

                    m_allItems.RemoveAt(i);
                    break;
                }
            }
        }

        public void RefreshGameObject(GameObject go)
        {
            if (!(raycastMode == RaycastMode.Sgame || raycastMode == RaycastMode.Sgame_tile))
            {
                return;
            }

            if (go == null || m_allItems == null)
            {
                return;
            }

            Image[] images = go.GetComponentsInChildren<Image>(true);

            for (int i = 0; i < images.Length; i++)
            {
                if (IsGameObjectHandleInput(images[i].gameObject))
                {
                    RefreshItem(images[i]);
                }
            }
        }

        public void RefreshItem(Image image)
        {
            if (image == null || m_allItems == null)
            {
                return;
            }

            Item item = null;

            for (int i = 0; i < m_allItems.Count; i++)
            {
                if (m_allItems[i].m_image == image)
                {
                    item = m_allItems[i];
                    break;
                }
            }

            if (item != null)
            {
                if (this.raycastMode == RaycastMode.Sgame_tile)
                {
                    RemoveFromTileList(item);
                }
            }
            else
            {
                item = Item.Create(image);

                if (item == null)
                {
                    return;
                }

                m_allItems.Add(item);
            }

            if (this.raycastMode == RaycastMode.Sgame_tile)
            {
                CalcItemCoord(ref item.m_coord, item);
                AddToTileList(item);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            m_allItems.Clear();
            m_RaycastResults.Clear();

            if (m_tiles != null)
            {
                for (int i = 0; i < m_tiles.Length; ++i)
                {
                    m_tiles[i].items.Clear();
                }

                m_tiles = null;
            }
        }

        void Raycast2(PointerEventData eventData, List<RaycastResult> resultAppendList, bool useTiles)
        {
            if (canvas == null)
                return;
            // Convert to view space
            Vector2 pos;
            if (eventCamera == null)
                pos = new Vector2(eventData.position.x / Screen.width, eventData.position.y / Screen.height);
            else
                pos = eventCamera.ScreenToViewportPoint(eventData.position);

            // If it's outside the camera's viewport, do nothing
            if (pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f)
                return;

            float hitDistance = float.MaxValue;

            Ray ray = new Ray();

            if (eventCamera != null)
                ray = eventCamera.ScreenPointToRay(eventData.position);

            //if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && blockingObjects != BlockingObjects.None)
            //{
            //    float dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

            //    if (blockingObjects == BlockingObjects.ThreeD || blockingObjects == BlockingObjects.All)
            //    {
            //        RaycastHit hit;
            //        if (Physics.Raycast(ray, out hit, dist, m_BlockingMask))
            //        {
            //            hitDistance = hit.distance;
            //        }
            //    }

            //    if (blockingObjects == BlockingObjects.TwoD || blockingObjects == BlockingObjects.All)
            //    {
            //        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, dist, m_BlockingMask);

            //        if (hit.collider != null)
            //        {
            //            hitDistance = hit.fraction * dist;
            //        }
            //    }
            //}

            m_RaycastResults.Clear();
            //resultAppendList.Clear();

            var pointerPosition = eventData.position;

            ListView<Item> items = null;

            if (useTiles && m_tiles != null)
            {
                int x = Mathf.Clamp((int)pointerPosition.x / m_tileSizeX, 0, TileCount - 1);
                int y = Mathf.Clamp((int)pointerPosition.y / m_tileSizeY, 0, TileCount - 1);
                int tileIndex = y * TileCount + x;

                items = m_tiles[tileIndex].items;
            }
            else
            {
                items = m_allItems;
            }

            for (int i = 0; i < items.Count; ++i)
            {
                items[i].Raycast(m_RaycastResults, pointerPosition, eventCamera);
            }

            m_RaycastResults.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));

            AppendResultList(ref ray, hitDistance, resultAppendList, m_RaycastResults);
        }

        private bool IsGameObjectHandleInput(GameObject go)
        {
            return (go.GetComponent<CUIEventScript>() != null ||
           //     go.GetComponent<CUIMiniEventScript>() != null ||
           //     go.GetComponent<CUIToggleEventScript>() != null ||
           //     go.GetComponent<CUIJoystickScript>() != null ||
                (!ignoreScrollRect && go.GetComponent<ScrollRect>() != null));
        }

        [NonSerialized]
        private List<Graphic> m_RaycastResults = new List<Graphic>();

        void AppendResultList(ref Ray ray, float hitDistance, List<RaycastResult> resultAppendList, List<Graphic> raycastResults)
        {
            //float hitDistance = float.MaxValue;

            for (var index = 0; index < raycastResults.Count; index++)
            {
                var go = raycastResults[index].gameObject;
                bool appendGraphic = true;

                if (ignoreReversedGraphics)
                {
                    if (eventCamera == null)
                    {
                        // If we dont have a camera we know that we should always be facing forward
                        var dir = go.transform.rotation * Vector3.forward;
                        appendGraphic = Vector3.Dot(Vector3.forward, dir) > 0;
                    }
                    else
                    {
                        // If we have a camera compare the direction against the cameras forward.
                        var cameraFoward = eventCamera.transform.rotation * Vector3.forward;
                        var dir = go.transform.rotation * Vector3.forward;
                        appendGraphic = Vector3.Dot(cameraFoward, dir) > 0;
                    }
                }

                if (appendGraphic)
                {
                    float distance = 0;

                    if (eventCamera == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                        distance = 0;
                    else
                    {
                        // http://geomalgorithms.com/a06-_intersect-2.html
                        distance = (Vector3.Dot(go.transform.forward, go.transform.position - ray.origin) / Vector3.Dot(go.transform.forward, ray.direction));

                        // Check to see if the go is behind the camera.
                        if (distance < 0)
                            continue;
                    }

                    if (distance >= hitDistance)
                        continue;

                    var castResult = new RaycastResult
                    {
                        gameObject = go,
                        module = this,
                        distance = distance,
                        index = resultAppendList.Count,
                        depth = raycastResults[index].depth,
                        sortingLayer = canvas.sortingLayerID,
                        sortingOrder = canvas.sortingOrder
                    };
                    resultAppendList.Add(castResult);
                }
            }
        }
    }
}