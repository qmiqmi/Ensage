﻿namespace Debugger.Menus.Dumping
{
    using System;

    using Ensage.Common.Menu;

    internal class Units
    {
        #region Constructors and Destructors

        public Units(Menu mainMenu)
        {
            var menu = new Menu("Units", "unitsDumpMenu");

            var dump = new MenuItem("units", "Get unit info").SetValue(false);
            menu.AddItem(dump);
            dump.ValueChanged += (sender, args) => { OnDump?.Invoke(this, EventArgs.Empty); };

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Events

        public event EventHandler OnDump;

        #endregion
    }
}