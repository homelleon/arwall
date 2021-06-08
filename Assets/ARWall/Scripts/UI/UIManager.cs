using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ARWall.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        private UIState _currentState;

        [SerializeField] private List<UIMode> modes = new List<UIMode>();

        public void ChangeState(UIState state)
        {
            _currentState = state;
            SwitchPanels();
        }

        private void SwitchPanels()
        {
             modes
                .Where(mode => mode.State != _currentState)
                .ToList()
                .ForEach(mode => mode.panels.ForEach(panel => panel.SetActive(false)));

            modes.FindAll(mode => mode.State == _currentState)
                .ToList()
                .ForEach(mode => mode.panels.ForEach(panel => panel.SetActive(true)));

        }
    }


    [Serializable]
    public struct UIMode
    {
        public UIState State;
        public List<GameObject> panels;
    }
}
