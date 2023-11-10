using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public abstract class Singleton<T>
        where T: new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                Create();
                return instance;
            }
        }
        public static void init()
        {
            if(instance == null) Create();
        }
        private static void Create()
        {
            instance = new T();
        }
    }
}
