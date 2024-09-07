
namespace UMI
{
    public class ViewEventDef
    {
        public const string LOAD_SCENE_COMPLETE = "LOAD_SCENE_COMPLETE";
        //public const string CHANGE_GAME_PLAY_UI = "CHANGE_GAME_PLAY_UI";

        public const string LOAD_PLAYER_COMPLETE = "LOAD_PLAYER_COMPLETE";


        public const string CHANGE_PLAYER_MOVE_LOCK_STATE = "CHANGE_PLAYER_MOVE_LOCK_STATE";

        public const string CLOSE_TASK_UI_ITEM = "CLOSE_TASK_UI_ITEM";

        public const string UPDATE_EYS_TASK = "UPDATE_EYS_TASK";

        public const string SELECT_BAG_HOME_OBJECT = nameof(SELECT_BAG_HOME_OBJECT);
        public const string HOME_OBJECT_MOVE_START = nameof(HOME_OBJECT_MOVE_START);
        public const string HOME_OBJECT_MOVE_END = nameof(HOME_OBJECT_MOVE_END);
    }


    public class NetEventDef
    {
        public const string ENTER_HALL_RSP = "ENTER_HALL_RSP";
    }

    public class LogicEventDef
    {
        public const string UPDATE_HOME_MAP_OBJECT = nameof(UPDATE_HOME_MAP_OBJECT);
        public const string DELETE_HOME_MAP_OBJECT = nameof(DELETE_HOME_MAP_OBJECT);
        public const string ADD_HOME_MAP_OBJECT = nameof(ADD_HOME_MAP_OBJECT);
        public const string REFRESH_HOME_FORGE_AREA_UI = nameof(REFRESH_HOME_FORGE_AREA_UI);
    }
}