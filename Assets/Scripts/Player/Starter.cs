using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace Player
{
    public class Starter : MonoBehaviour
    {
        private IPlayerController _controller;

        [Inject]
        public void Construct(IPlayerController controller)
        {
            _controller = controller;
        }

        public void Start()
        {
            _controller.Init();
        }
    }
}

