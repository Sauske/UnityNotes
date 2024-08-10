using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UMI
{

    public class BagModule : BaseModule
    {

        List<ItemData> items = new List<ItemData>();

        Dictionary<ITEM_TYPE, List<ItemData>> itemDic = new Dictionary<ITEM_TYPE, List<ItemData>>();

        public BagModule() : base()
        {

        }
        protected override void Initialize()
        {
            base.Initialize();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override void OnDispose()
        {
            base.OnDispose();
        }

        public void AddRange(List<ItemData> list)
        {
            for (int idx = 0; idx < list.Count; idx++)
            {
                AddItem(list[idx]);
            }
        }

        public void AddItem(ItemData data)
        {
            Add(data, items);
        }

        void Add(ItemData data, List<ItemData> list)
        {
            bool bFind = false;
            for (int idx = 0; idx < list.Count; idx++)
            {
                if (data.itemId == list[idx].itemId)
                {
                    bFind = true;
                    list[idx].num = data.num;
                    break;
                }
            }

            if (!bFind)
            {
                list.Add(data);
            }
        }

        void updata(ItemData data, List<ItemData> list)
        {
            bool bFind = false;
            for (int idx = 0; idx < list.Count; idx++)
            {
                if (data.itemId == list[idx].itemId)
                {
                    bFind = true;
                    list[idx].num = data.num;
                    break;
                }
            }

            if (!bFind)
            {
                list.Add(data);
            }
        }
        public List<ItemData> GetItemList(ITEM_TYPE type)
        {
            List<ItemData> list = null;
            if (itemDic.TryGetValue(type, out list))
            {
                return list;
            }
            return new List<ItemData>();
        }
    }
}
