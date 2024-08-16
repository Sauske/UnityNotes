
namespace Framework
{
	public class Singleton<T> where T : class, new()
	{
		private static T _instance;

		protected Singleton()
		{

		}

		public static T CustomSetActive
		{
			get
			{
				if (_instance == null)
				{
					CreatInstance();
				}
				return _instance;
			}
		}

		public static T GetInstance()
		{
			if (_instance == null)
			{
				CreatInstance();
			}
			return _instance;
		}


		public static T CreatInstance()
		{
			if (_instance == null)
			{
				_instance = new T();
				(_instance as Singleton<T>).Init();
				return _instance;
			}
			return new T();
		}

		public static void DesdroyInstance()
		{
			if (_instance != null)
			{
				(_instance as Singleton<T>).UnInit();
				_instance = null;
			}
		}

		public virtual void Init()
		{

		}

		public virtual void UnInit()
		{

		}
	}
}
