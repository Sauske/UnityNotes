using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace UnityWebSocket
{
    /// <summary>
    /// proto
    /// <summary>
    public class Commands
	{
		private static Commands _instance = null;

		public static Commands getInstance()
		{
			if (_instance == null)
				_instance = new Commands();
			return _instance;
		}

		public Dictionary<int, string> commandMap = new Dictionary<int, string>();

		public Commands()
		{
			commandMap[10000] = "U10000";
			commandMap[10004] = "U10004";
			commandMap[10010] = "U10010";
			commandMap[10011] = "U10011";
			commandMap[10012] = "U10012";
			commandMap[10013] = "U10013";
			commandMap[10014] = "U10014";
			commandMap[10015] = "U10015";
			commandMap[10016] = "U10016";
			commandMap[10017] = "U10017";
			commandMap[10018] = "U10018";
			commandMap[10019] = "U10019";
			commandMap[10020] = "U10020";
			commandMap[10021] = "U10021";
			commandMap[10022] = "U10022";
			commandMap[10031] = "U10031";
			commandMap[10032] = "U10032";
			commandMap[10035] = "U10035";
			commandMap[10036] = "U10036";
			commandMap[10037] = "U10037";
			commandMap[10038] = "U10038";
			commandMap[10101] = "U10101";
			commandMap[10102] = "U10102";
			commandMap[10103] = "U10103";
			commandMap[10104] = "U10104";
			commandMap[20001] = "U20001";
			commandMap[20002] = "U20002";
			commandMap[20003] = "U20003";
			commandMap[20004] = "U20004";
			commandMap[20005] = "U20005";
			commandMap[20006] = "U20006";
			commandMap[20007] = "U20007";
			commandMap[20008] = "U20008";
			commandMap[20009] = "U20009";
			commandMap[20010] = "U20010";
			commandMap[20011] = "U20011";
			commandMap[20012] = "U20012";
			commandMap[20014] = "U20014";
			commandMap[20015] = "U20015";
			commandMap[20016] = "U20016";
			commandMap[20030] = "U20030";
			commandMap[50000] = "U50000";
			commandMap[50001] = "U50001";
			commandMap[50101] = "U50101";
			commandMap[50102] = "U50102";
			commandMap[50103] = "U50103";
			commandMap[50104] = "U50104";
			commandMap[50105] = "U50105";
			commandMap[50201] = "U50201";
			commandMap[50202] = "U50202";
			commandMap[50203] = "U50203";
			commandMap[50301] = "U50301";
			commandMap[50302] = "U50302";
			commandMap[50303] = "U50303";
			commandMap[59910] = "U59910";
		}
	}
}
