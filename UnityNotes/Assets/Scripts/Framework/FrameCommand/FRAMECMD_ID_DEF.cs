
namespace UMI.FrameCommand
{
    public enum FRAMECMD_ID_DEF
    {
        FRAME_CMD_INVALID = 0, // ��Ч����
        FRAME_CMD_PLAYERMOVE = 1, // ����ƶ�
        FRAME_CMD_PLAYERSTOPMOVE = 3, // ���ֹͣ�ƶ�
        FRAME_CMD_ATTACKPOSITION = 4, // ����ĳ��λ��
        FRAME_CMD_ATTACKACTOR = 5, // ����ĳ����ɫ
        FRAME_CMD_LEARNSKILL = 6, // ʹ�ü��ܵ���������
        FRAME_CMD_USECURVETRACKSKILL = 9, // ʹ�ù켣�Լ���
        FRAME_CMD_USECOMMONATTACK = 10, // ʹ���չ�����
        FRAME_CMD_SWITCHAOUTAI = 11, // �л���ɫ���Զ�AIģʽ
        FRAME_CMD_SWITCHCAPTAIN = 12, // �л�������ؽ�ɫ
        FRAME_CMD_SWITCHSUPERKILLER = 13, // �л���ɫ��һ����ɱ״̬
        FRAME_CMD_SWITCHGODMODE = 14, // �л���ɫ���޵�״̬
        FRAME_CMD_LEARNTALENT = 15, // ѧϰ��һ���츳
        FRAME_CMD_TESTCOMMANDDELAY = 16, // ���������ӳ�ʱ��
        FRAME_CMD_PLAYATTACKTARGETMODE = 20, // ��ҹ���Ŀ��ģʽ
        FRAME_CMD_SVRNTFCHGKFRAMELATER = 21, // ������֪ͨ�޸��Ӻ�֡��
        FRAME_CMD_PLAYER_BUY_EQUIP = 24, // ��ҹ���װ��
        FRAME_CMD_PLAYER_SELL_EQUIP = 25, // ��ҳ���װ��
        FRAME_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE = 26, // ������Ӿ��ڽ��
        FRAME_CMD_SET_SKILL_LEVEL = 27, // ���ü��ܵȼ�
        FRAME_CMD_PLAYCOMMONATTACKTMODE = 28, // ��ͨ�л�ģʽ
        FRAME_CMD_LOCKATTACKTARGET = 29, // ������������
        FRAME_CMD_Signal_Btn_Position = 30, // ��ť�����ź�-λ������
        FRAME_CMD_Signal_MiniMap_Position = 31, // С��ͼ�����ź�-λ������
        FRAME_CMD_Signal_MiniMap_Target = 32, // С��ͼ�����ź�-Ŀ������
        FRAME_CMD_BUY_HORIZON_EQUIP = 34, // ʹ����Ұ��װ��
        FRAME_CMD_PLAYER_IN_OUT_EQUIPSHOP = 35, // ��ҽ���/�˳�װ���̵�
        FRAME_CMD_CHANGE_USED_RECOMMEND_EQUIP_GROUP = 36, // ��Ҹı䵱ǰʹ�õ��Ƽ�װ��Group
        FRAME_CMD_PLAYLASTHITMODE = 37, // �л�����ģʽ
        FRAME_CMD_PLAYER_CHOOSE_EQUIPSKILL = 38, // ���ѡ��װ��������������
        FRAME_CMD_PLAYER_CHEAT = 39, // ���Բ߻���GMָ��ϼ�
    }
    public enum SC_FRAME_CMD_ID_DEF
    {
        SC_FRAME_CMD_INVALID = 0, // ��Ч����
        SC_FRAME_CMD_PLAYERRUNAWAY = 192, // �������
        SC_FRAME_CMD_PLAYERDISCONNECT = 193, // ��ҵ�����
        SC_FRAME_CMD_PLAYERRECONNECT = 194, // �������������
        SC_FRAME_CMD_ASSISTSTATECHG = 195, // �й�״̬�仯
        SC_FRAME_CMD_CHGAUTOAI = 196, // �л����Զ�ս��
        SC_FRAME_CMD_SVRNTF_GAMEOVER = 197, // ������֪ͨ��Ϸ����-����Ͷ��
        SC_FRAME_CMD_PAUSE_RESUME_GAME = 198, // ��ͣ/�ָ���Ϸ
    }

    public enum CSSYNC_TYPE_DEF
    {
        CSSYNC_CMD_NULL = 0,
        CSSYNC_CMD_USEOBJECTIVESKILL = 128, // ָ��Ŀ�꼼�� k��˵��128��ʼ blahblahblah..
        CSSYNC_CMD_USEDIRECTIONALSKILL = 129, // ָ��������
        CSSYNC_CMD_USEPOSITIONSKILL = 130, // ָ��λ�ü���
        CSSYNC_CMD_MOVE = 131, // �ƶ�
        CSSYNC_CMD_BASEATTACK = 132, // ��ͨ����
    }
}