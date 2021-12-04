using UnityEngine;

namespace PIXEL.Landlords.FrameWork
{
    public class SingletonPattern<T> : MonoBehaviour where T : SingletonPattern<T>
    {
        private static T _instance;

        //定义属性
        public static T Instance
        {
            get 
            {
                return _instance;
            }
        }

        //游戏开始时
        protected virtual void Awake() 
        {
            if (_instance != null)
                Destroy(gameObject);
            else
                _instance = (T)this;
        }

        //当前物体被销毁时
        protected virtual void OnDestroy() 
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}