//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Framework
{
    [ProtoBuf.ProtoContract]
    public class Address
    {
        [ProtoBuf.ProtoMember(1)]
        public string Line1 { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public string Line2 { get; set; }
    }

    [Serializable]
    public class ProtoEntity
    {
        public int id;
        public string name;

        public ProtoEntity(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
    
    public class ProbobufDemo : MonoBehaviour
    {
        public const string PATH_PROTOBUF = "Scripts/Demo/Protobuf/Protobuf.bytes";

        List<Address> list = new List<Address>();
        public void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                list.Add(new Address() { Line1 = i.ToString(), Line2 = "aldfk", });
            }

            //bool IsSerialize = Serializer.Serialize<List<Address>>(list, PATH_PROTOBUF);

            //Debug.Log(IsSerialize);

            //List<Address> DeList = Serializer.DeSerialize<List<Address>>(PATH_PROTOBUF);

            //for (int i = 0; i < DeList.Count; i++)
            //{
            //    Debug.Log(DeList[i].Line1 + DeList[i].Line2);
            //}
        }
    }
}