using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace UFoxLib.MainMenu
{
    public class MainMenuModEntryPrefab : MonoBehaviour
    {
        public void SetupInfo(UFoxModInfoEntry _mod)
        {
            if (_mod.modImage != null) modImage.sprite = _mod.modImage;
            modName.text = _mod.modName;
            modVersion.text = _mod.modVersion;
        }

       // void OnPointerEnter(PointerEventData eventData)
       //{
        //    if (!_isHoveringOn) _isHoveringOn = true;
        //    ShowVersion();
        //}

        //void OnPointerExit(PointerEventData eventData)
       // {
        //    if (_isHoveringOn) _isHoveringOn = false;
       //     ShowVersion();
        //}

        //void ShowVersion() => showModVersion.SetActive(_isHoveringOn);

        //bool _isHoveringOn;

        public Image modImage;

        public TextMeshProUGUI modName;

        public TextMeshProUGUI modVersion;

        public GameObject showModVersion;
    }
}
