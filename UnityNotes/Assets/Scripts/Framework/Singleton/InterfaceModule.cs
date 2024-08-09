using System;

namespace SFramework
{

    public interface IModule : IDisposable
    {
        void FreeMemory();
    }

    public interface IUpdate
    {
        void OnUpdate(float delta);
    }

    /// <summary>
    /// ��¼ģ��ӿڣ���Ҫ�ڵ�¼�ɹ�����ʼ������ע�������¼�����Ҫʵ�ָýӿ�
    /// ��������޷���֤����ģ��ĵ���˳��
    /// </summary>
    public interface ILogin
    {
        /// <summary>
        /// ��¼�ɹ�������ֻ�������Լ�ģ���ڲ������ݺ�״̬ά��
        /// </summary>
        public void OnLogin();
        /// <summary>
        /// �˳���¼�ɹ�
        /// </summary>
        public void OnLogout();
    }
}


