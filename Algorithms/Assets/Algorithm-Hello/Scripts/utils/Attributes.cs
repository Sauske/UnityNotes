using System;

namespace hello_algo.utils
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestAttibute:Attribute
    {
        public TestAttibute()
        {

        }
    }
}
