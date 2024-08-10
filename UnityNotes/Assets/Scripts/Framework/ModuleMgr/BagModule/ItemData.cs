using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UMI
{

    public enum ITEM_TYPE
    {
        NONE = 0,
        /// <neymar>
        /// 货币
        /// </neymar>
        MONEY,
        /// <neymar>
        /// 背包
        /// </neymar>
        BAG,
        /// <neymar>
        /// 装饰
        /// </neymar>
        ORNAMENT,
        /// <neymar>
        /// 角色
        /// </neymar>
        ROLE,
    }

    public enum ITEM_BAG_SMALL_TYPE
    {
        /// <neymar>
        NONE, /// 无类型
        CHANG_NAME,/// 改名卡
        TRUMPET,  /// 喇叭
        MAGIC, /// 魔法表情
        GIFT, /// 礼包
        CHIP, /// 碎片
        PROP, /// 游戏道具
        GLAMOUR, /// 魅力道具
        EXPERIENCE, /// 经验卡
        GOODS, /// 实物道具
        VIP_EXP, /// VIP体验卡
        DESK_EXP, /// 牌桌体验卡
        ROLE_CHIP, /// 角色碎片
        ROLE_EXP, /// 角色体验卡
                  /// </neymar>
    }

    public class ItemData
    {

        /// <summary>
        /// 背包id
        /// <summary>
        public int id;
        /// <summary>
        /// 道具id
        /// <summary>
        public short itemId;
        /// <summary>
        /// 物品数量
        /// <summary>
        public long num;

        /// <summary>
        /// 过期时间戳
        /// <summary>
        public long expiredTime;
        /// <summary>
        /// 过期时间
        /// </summary>
        /// 

        public ItemData(int _id,int _num)
        {
            this.id = _id;
            this.num = _num;
        }
    }
}
