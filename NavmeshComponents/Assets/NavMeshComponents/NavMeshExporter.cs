using UnityEngine;
using UnityEngine.AI;
using System.IO;
using System.Text;

namespace NavMeshComponents
{
    public class NavMeshExporter : MonoBehaviour
    {
        [SerializeField]
        private string fileName = "NavMesh.obj";

        void Start()
        {
            ExportNavMeshToObj();
        }

        private void ExportNavMeshToObj()
        {
            NavMeshTriangulation triangulatedNavMesh = NavMesh.CalculateTriangulation();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# Unity NavMesh to OBJ");

            // Write vertices
            foreach (Vector3 vertex in triangulatedNavMesh.vertices)
            {
                sb.AppendLine($"v {vertex.x} {vertex.y} {vertex.z}");
            }

            sb.AppendLine();

            // Write faces (indices)
            for (int i = 0; i < triangulatedNavMesh.indices.Length; i += 3)
            {
                int idx0 = triangulatedNavMesh.indices[i] + 1;
                int idx1 = triangulatedNavMesh.indices[i + 1] + 1;
                int idx2 = triangulatedNavMesh.indices[i + 2] + 1;
                sb.AppendLine($"f {idx0} {idx1} {idx2}");
            }

            // Save to file
            string path = Path.Combine(Application.dataPath, fileName);
            File.WriteAllText(path, sb.ToString());

            Debug.Log($"NavMesh exported to {path}");
        }
    }
}


//public void GetAIPath(Vector3 startPos, Vector3 targetPos, Action<List<Vector3>> onPathComplete)
//{
//    Log.Console("Navigation: startPos:{0},targetPos:{1}", startPos, targetPos);
//    mPathList.Clear();
//    mDrawPath.Clear();

//    NavMeshHit hit;
//    //起点可能不可寻路，先判断
//    if (NavMesh.SamplePosition(startPos, out hit, 1.0f, NavMesh.AllAreas))
//    {
//        startPos = hit.position;
//        mPathList.Add(startPos);
//    }
//    //找到目标点最近的可寻路点寻路过去
//    if (NavMesh.SamplePosition(targetPos, out hit, 10.0f, NavMesh.AllAreas))
//    {
//        targetPos = hit.position;
//    }

//    if (NavMesh.CalculatePath(startPos, targetPos, NavMesh.AllAreas, mNavPath))
//    {
//        if (mNavPath.status == NavMeshPathStatus.PathComplete)
//        {
//            mIsNav = true;

//            mDrawPath.Add(startPos);
//            for (int idx = 0; idx < mNavPath.corners.Length; idx++)
//            {
//                mPathList.Add(mNavPath.corners[idx]);
//                mDrawPath.Add(mNavPath.corners[idx]);
//            }
//            Log.Console("Navigation: PathList Count:{0}", mPathList.Count);
//            onPathComplete?.Invoke(mPathList);
//        }
//        else
//        {
//            mIsNav = false;
//            UNotifyWin.Instance.Show("寻路失败", () =>
//            {
//                //onPathComplete?.Invoke(new List<Vector3>());
//            });
//            onPathComplete?.Invoke(new List<Vector3>());
//        }
//    }
//    else
//    {
//        mIsNav = false;
//        UNotifyWin.Instance.Show("寻路失败", () =>
//        {
//            //onPathComplete?.Invoke(new List<Vector3>());
//        });
//        onPathComplete?.Invoke(new List<Vector3>());
//    }
//}