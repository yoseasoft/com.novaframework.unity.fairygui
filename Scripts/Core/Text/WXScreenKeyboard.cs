#if UNITY_WEBGL || WEIXINMINIGAME
using UnityEngine;
using WeChatWASM;

namespace FairyGUI
{
    /// <summary>
    /// 
    /// </summary>
    public class WXScreenKeyboard : IKeyboard
    {
        UnityEngine.TouchScreenKeyboard _keyboard;
        private bool _showKeyboard = false;
        public bool done
        {
#if UNITY_2017_2_OR_NEWER
            get
            {
                return _keyboard == null
              || _keyboard.status == UnityEngine.TouchScreenKeyboard.Status.Done
              || _keyboard.status == UnityEngine.TouchScreenKeyboard.Status.Canceled
              || _keyboard.status == UnityEngine.TouchScreenKeyboard.Status.LostFocus;
            }
#else
            get { return _keyboard == null || _keyboard.done || _keyboard.wasCanceled; }
#endif
        }

        public bool supportsCaret
        {
            get { return false; }
        }

        public string GetInput()
        {
            if (_keyboard != null)
            {
                string s = _keyboard.text;

                if (this.done)
                    _keyboard = null;

                return s;
            }
            else
                return null;
        }

        public void Open(string text, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder, int keyboardType, bool hideInput)
        {
            if (_keyboard != null)
                return;

            ShowKeyboard();

            UnityEngine.TouchScreenKeyboard.hideInput = hideInput;
            _keyboard = UnityEngine.TouchScreenKeyboard.Open(text, (TouchScreenKeyboardType)keyboardType, autocorrection, multiline, secure, alert, textPlaceholder);
            Debug.Log($"----- Open text = {text}");
        }

        public void Close()
        {
            if (_keyboard != null)
            {
                HideKeyboard();
                
                _keyboard.active = false;
                _keyboard = null;
            }
        }
        
        #region WX API
        private void ShowKeyboard()
        {
            if (!_showKeyboard)
            {
                ShowKeyboardOption callback = new ShowKeyboardOption()
                {
                    defaultValue = "xxx",
                    maxLength = 20,
                    confirmType = "go"
                };
                WX.ShowKeyboard(callback);

                //绑定回调
                WX.OnKeyboardConfirm(OnConfirm);
                WX.OnKeyboardComplete(OnComplete);
                WX.OnKeyboardInput(OnInput);
            
                Debug.Log($"----- 显示键盘成功 绑定回调");
            }
            else
            {
                Debug.Log($"----- 显示键盘失败 _showKeyboard = {_showKeyboard}");
            }
        }

        private void HideKeyboard()
        {
            if (_showKeyboard)
            {
                WX.HideKeyboard(new HideKeyboardOption());
                //删除掉相关事件监听
                WX.OffKeyboardInput(OnInput);
                WX.OffKeyboardConfirm(OnConfirm);
                WX.OffKeyboardComplete(OnComplete);
                _showKeyboard = false;
                
                Debug.Log($"----- 隐藏键盘成功");
            }
            else
            {
                Debug.Log($"----- 隐藏键盘失败");
            }
        }
        
        public void OnInput(OnKeyboardInputListenerResult v)
        {
            Debug.Log($"----- VX回调 OnInput text = {v.value}");
            // if (input.isFocused)
            // {
            //     input.text = v.value;
            //     Debug.Log($"----- OnInput Focused");
            // }
        }

        public void OnConfirm(OnKeyboardInputListenerResult v)
        {
            // 输入法confirm回调
            Debug.Log($"----- VX回调 OnConfirm {v.value}");
            HideKeyboard();
        }

        public void OnComplete(OnKeyboardInputListenerResult v)
        {
            // 输入法complete回调
            Debug.Log($"----- VX回调 OnComplete {v.value}");
            HideKeyboard();
        }
        
        #endregion
    }
}
#endif
