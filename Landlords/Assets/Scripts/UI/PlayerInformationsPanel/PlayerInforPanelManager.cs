using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PIXEL.Landlords.FrameWork;
using UnityEditor;
using SmartDLL;

namespace PIXEL.Landlords.Sets.PlayerInformatioSets
{
    public class PlayerInforPanelManager : SingletonPattern<PlayerInforPanelManager>
    {
        //系统文件访问器
        private SmartFileExplorer fileExplorer = new SmartFileExplorer();
        private string path;

        [Header("头像设置")]
        private RawImage icon_player_IconImage;
        private Button icon_button_UpLoadIcon;
        private int icon_iconSizeX = 274;
        private int icon_iconSizeY = 274;

        [Header("昵称设置")]
        private InputField rename_InputField;
        private Text rename_ShowText;
        private Button rename_RenameButton;

        private void Start()
        {
            icon_player_IconImage = transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<RawImage>();
            icon_button_UpLoadIcon = transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Button>();

            rename_InputField = transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<InputField>();
            rename_ShowText = transform.GetChild(1).GetChild(2).GetChild(2).GetComponent<Text>();
            rename_RenameButton = transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<Button>();

            rename_RenameButton.onClick.AddListener(delegate { Rename(); });
            rename_InputField.onEndEdit.AddListener(delegate { RenameComplete(); });

            icon_button_UpLoadIcon.onClick.AddListener(delegate { OpenFile(); });
        }

        #region ChangePlayerIcon
        private void OpenFile() 
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            path = EditorUtility.OpenFilePanel("Choose Your Favorite Texture!", "", "png");
            GetImageAndSet();
            return;
#endif
                       //默认初始位置↘   //是否可设置位置   //设置提示内容     //可见文件类型    //可选文件类型
            fileExplorer.OpenExplorer(@"C:\", true, "Choose Your Favorite Image", "png", "png files(*.png)|*.png");
            path = fileExplorer.fileName;
            GetImageAndSet();
        }

        private void GetImageAndSet() 
        {
            //WWW www = new WWW("https://" + "gimg2.baidu.com/image_search/src=http%3A%2F%2Fimg.jj20.com%2Fup%2Fallimg%2Ftp05%2F19100120461512E-0-lp.jpg&refer=http%3A%2F%2Fimg.jj20.com&app=2002&size=f9999,10000&q=a80&n=0&g=0n&fmt=jpeg?sec=1642057315&t=504850d67fe6e65ea7c9d6fb5a5c639d");
            WWW www = new WWW("file:///" + path);
            icon_player_IconImage.texture = www.texture;
            icon_player_IconImage.SetNativeSize();
            icon_player_IconImage.GetComponent<RectTransform>().sizeDelta = new Vector2(icon_iconSizeX, icon_iconSizeY);
            www.Dispose();       
        }
        #endregion

        #region Rename

        private void Rename() 
        {
            rename_ShowText.gameObject.SetActive(!rename_ShowText.gameObject.activeSelf);
            rename_InputField.gameObject.SetActive(!rename_InputField.gameObject.activeSelf);
            rename_InputField.text = rename_ShowText.text;
        }

        private void RenameComplete() 
        {
            rename_ShowText.gameObject.SetActive(!rename_ShowText.gameObject.activeSelf);
            rename_InputField.gameObject.SetActive(!rename_InputField.gameObject.activeSelf);
            rename_ShowText.text = rename_InputField.text;

            PlayerInformations.Player_Name = rename_ShowText.text;
        }

        #endregion
    }
}