﻿using System;

namespace Injectoclean.Tools.BLE
{
    public interface ILockScreen
    {
        void Show(String title);
        void Close();
        void setTitle(String title);
        void set(String title, String content, int timeout);
        void SetwithButton(String title, String content, String CloseButtonName);
    }
}