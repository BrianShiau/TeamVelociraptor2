using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class LoadScene : MonoBehaviour
    {
        public void Load(int index)
        {
            SceneManager.LoadScene(index);
        }

        public void Load(string name)
        {
            SceneManager.LoadScene(name);
        }
    }
}
