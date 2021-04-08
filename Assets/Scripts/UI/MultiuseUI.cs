using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiuseUI : MonoBehaviour
{
    #region Singleton

    public static MultiuseUI instace;
    public WindowManager windowManager;

    public GameObject multiuseUI;

    private void Awake()
    {
        if(instace == null)
        {
            instace = this;
        }
    }

    #endregion
}
