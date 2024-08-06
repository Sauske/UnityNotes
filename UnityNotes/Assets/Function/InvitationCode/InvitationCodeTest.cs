using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InvitationCode
{
    public class InvitationCodeTest : MonoBehaviour {

        void Start() {
            long id = 531347133124609;
            string code = Invitation.IdToCode(id);

            Debug.LogFormat("输入的ID是 '{0}'， 生成的邀请码是 '{1}'", id, code);
            Debug.LogFormat("邀请码 '{0}' 转化后的ID是 '{1}'", code, Invitation.CodeToId(code));
        }
    }
}
