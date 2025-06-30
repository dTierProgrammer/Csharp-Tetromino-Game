using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoStacker.Source.Scene
{
    public class SceneManager
    {
        private Stack<IScene> _scenes = new();

        public void EnterScene(IScene scene)
        {
            scene.Load();
            scene.Initialize();
            _scenes.Push(scene);
        }

        public void ExitScene() 
        {
            _scenes.Pop();
        }

        public IScene CurrentScene() 
        {
            return _scenes.Peek();
        }
    }
}
