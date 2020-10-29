using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MelonLoader;
using UnhollowerRuntimeLib;
using System.Reflection;

namespace AvatarStatsShowAuthor
{
    public static class BuildInfo
    {
        public const string Name = "AvatarStatsShowAuthor";
        public const string Author = "Herp Derpinstine & dave-kun";
        public const string Company = "Lava Gang";
        public const string Version = "1.0.4";
        public const string DownloadLink = "https://github.com/HerpDerpinstine/AvatarStatsShowAuthor";
    }

    public class AvatarStatsShowAuthor : MelonMod
    {
        private static VRC.UI.PageAvatar pageAvatar;
        private static VRC.UI.PageUserInfo pageUserInfo;
        private MethodInfo ShowUserMethod;

        public override void VRChat_OnUiManagerInit()
        {
            ShowUserMethod = typeof(QuickMenu).GetMethods().Where(it => it.GetParameters().Length == 2 && it.GetParameters()[0].ParameterType.ToString() == "System.Int32" && it.GetParameters()[1].ParameterType.ToString() == "System.Boolean").First();
            GameObject pageUserInfoObj = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo");
            if (pageUserInfoObj != null)
            {
                pageUserInfo = pageUserInfoObj.GetComponent<VRC.UI.PageUserInfo>();
                if (pageUserInfo != null)
                {
                    pageAvatar = Resources.FindObjectsOfTypeAll<VRC.UI.PageAvatar>().First(p => (p.transform.Find("Change Button") != null));
                    if (pageAvatar != null)
                    {
                        GameObject statspopup = GameObject.Find("UserInterface/MenuContent/Popups/AvatarStatsPopup");
                        if (statspopup != null)
                        {
                            Transform documentationbutton = statspopup.transform.Find("AvatarStatsMenu/_Buttons/DocumentationButton");
                            if (documentationbutton != null)
                            {
                                RectTransform recttrans = documentationbutton.GetComponent<RectTransform>();
                                if (recttrans != null)
                                {
                                    recttrans.sizeDelta = new Vector2((recttrans.sizeDelta.x - 600), recttrans.sizeDelta.y);
                                    recttrans.localPosition = new Vector3((recttrans.localPosition.x - 300), recttrans.localPosition.y, recttrans.localPosition.z);
                                }

                                Transform buttontxt_trans = documentationbutton.Find("Text");
                                if (buttontxt_trans != null)
                                {
                                    Text buttontxt = buttontxt_trans.GetComponent<Text>();
                                    if (buttontxt != null)
                                        buttontxt.fontSize -= 10;
                                }

                                Transform buttonoverheadtxt_trans = documentationbutton.Find("Text (1)");
                                if (buttonoverheadtxt_trans != null)
                                {
                                    RectTransform recttranstxt = buttonoverheadtxt_trans.GetComponent<RectTransform>();
                                    if (recttranstxt != null)
                                    {
                                        recttranstxt.sizeDelta = new Vector2((recttranstxt.sizeDelta.x + 600), recttranstxt.sizeDelta.y);
                                        recttranstxt.localPosition = new Vector3((recttranstxt.localPosition.x + 300), recttranstxt.localPosition.y, recttranstxt.localPosition.z);
                                    }
                                }

                                Transform showcreatorbutton_trans = DuplicateButton(documentationbutton, "Show Avatar Author", new Vector2(600, 0));
                                Button showcreatorbutton = showcreatorbutton_trans.GetComponent<Button>();
                                showcreatorbutton.onClick = new Button.ButtonClickedEvent();
                                showcreatorbutton.onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(new Action(() => {
                                    if ((pageUserInfo != null) && (pageAvatar != null) && (pageAvatar.avatar != null))
                                    {
                                        VRC.Core.ApiAvatar currentApiAvatar = pageAvatar.avatar.field_Internal_ApiAvatar_0;
                                        if (currentApiAvatar != null)
                                        {
                                            string authorid = currentApiAvatar.authorId;
                                            if (!string.IsNullOrEmpty(authorid))
                                            {
                                                HideCurrentPopup();
                                                VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_Boolean_1(true);
                                                VRC.Core.APIUser.FetchUser(authorid, new Action<VRC.Core.APIUser>((user) =>
                                                {
                                                    QuickMenu.prop_QuickMenu_0.prop_APIUser_0 = user;
                                                    ShowUserMethod.Invoke(QuickMenu.prop_QuickMenu_0, new object[] { 4, false });
                                                }), null);
                                            }
                                        }
                                    }
                                })));
                            }
                        }
                    }
                }
            }
        }

        private static MethodInfo VRCUiManager_Instance_get = null;
        private static VRCUiManager GetVRCUiManager()
        {
            if (VRCUiManager_Instance_get == null)
                VRCUiManager_Instance_get = typeof(VRCUiManager).GetMethods().First(x => (x.ReturnType == typeof(VRCUiManager)));
            if (VRCUiManager_Instance_get == null)
                return null;
            return (VRCUiManager)VRCUiManager_Instance_get.Invoke(null, new object[0]);
        }
        private static void HideCurrentPopup()
        {
            VRCUiManager vRCUiManager = GetVRCUiManager();
            if (vRCUiManager == null)
                return;
            if (!vRCUiManager.field_Internal_Dictionary_2_String_VRCUiPage_0.ContainsKey("POPUP"))
                return;
            VRCUiPage item = vRCUiManager.field_Internal_Dictionary_2_String_VRCUiPage_0["POPUP"];
            item.gameObject.SetActive(false);
            if (item.onPageDeactivated != null)
                item.onPageDeactivated.Invoke();
            vRCUiManager.field_Internal_Dictionary_2_String_VRCUiPage_0.Remove("POPUP");
        }

        private static Transform DuplicateButton(Transform baseButton, string buttonText, Vector2 posDelta)
        {
            GameObject buttonGO = new GameObject("DuplicatedButton", new Il2CppSystem.Type[] {
                Il2CppType.Of<Button>(),
                Il2CppType.Of<Image>()
            });

            RectTransform rtO = baseButton.GetComponent<RectTransform>();
            RectTransform rtT = buttonGO.GetComponent<RectTransform>();

            buttonGO.transform.SetParent(baseButton.parent);
            buttonGO.GetComponent<Image>().sprite = baseButton.GetComponent<Image>().sprite;
            buttonGO.GetComponent<Image>().type = baseButton.GetComponent<Image>().type;
            buttonGO.GetComponent<Image>().fillCenter = baseButton.GetComponent<Image>().fillCenter;
            buttonGO.GetComponent<Button>().colors = baseButton.GetComponent<Button>().colors;
            buttonGO.GetComponent<Button>().targetGraphic = buttonGO.GetComponent<Image>();

            rtT.localScale = rtO.localScale;

            rtT.anchoredPosition = rtO.anchoredPosition;
            rtT.sizeDelta = rtO.sizeDelta;

            rtT.localPosition = rtO.localPosition + new Vector3(posDelta.x, posDelta.y, 0);
            rtT.localRotation = rtO.localRotation;

            GameObject textGO = new GameObject("Text", new Il2CppSystem.Type[] { Il2CppType.Of<Text>() });
            textGO.transform.SetParent(buttonGO.transform);

            RectTransform rtO2 = baseButton.Find("Text").GetComponent<RectTransform>();
            RectTransform rtT2 = textGO.GetComponent<RectTransform>();
            rtT2.localScale = rtO2.localScale;

            rtT2.anchorMin = rtO2.anchorMin;
            rtT2.anchorMax = rtO2.anchorMax;
            rtT2.anchoredPosition = rtO2.anchoredPosition;
            rtT2.sizeDelta = rtO2.sizeDelta;

            rtT2.localPosition = rtO2.localPosition;
            rtT2.localRotation = rtO2.localRotation;

            Text tO = baseButton.Find("Text").GetComponent<Text>();
            Text tT = textGO.GetComponent<Text>();
            tT.text = buttonText;
            tT.font = tO.font;
            tT.fontSize = tO.fontSize;
            tT.fontStyle = tO.fontStyle;
            tT.alignment = tO.alignment;
            tT.color = tO.color;

            return buttonGO.transform;
        }
    }
}
 
