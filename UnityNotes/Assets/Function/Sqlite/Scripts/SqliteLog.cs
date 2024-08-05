namespace UMI
{
    /// <summary>
    /// http文件模块的专用日志类
    /// </summary>
    internal class SqliteLog
    {
        private const string LOG_PREFIX = "[Sqlite_log]: ";        // 相关日志前缀
        
        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="log"></param>
        public static void Debug(string log)
        {
           // Logger.Instance.Debug(LOG_PREFIX + log);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="log"></param>
        public static void Warn(string log)
        {
           // Logger.Instance.Warning(LOG_PREFIX + log);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="log"></param>
        public static void Error(string log)
        {
           // Logger.Instance.Error(LOG_PREFIX + log);
        }
    }
}