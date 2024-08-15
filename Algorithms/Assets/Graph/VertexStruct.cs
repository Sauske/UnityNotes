using UnityEngine;

namespace Graph
{
    public struct VertexStruct
    {
        public uint id;
        public uint mapId;//所属mapId，区分类型
        public float value;
        public string flag;
        public Vector3 position;
    }
}