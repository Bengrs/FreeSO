﻿using FSO.Client;
using FSO.Client.UI.Framework;
using FSO.Files.Formats.IFF.Chunks;
using FSO.IDE.EditorComponent.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSO.IDE.EditorComponent
{
    public class BHAVViewControl : FSOUIControl
    {
        public BHAVViewControl() : base()
        {

        }

        public void InitBHAV(BHAV bhav, EditorScope scope)
        {
            var mainCont = new UIExternalContainer(1024, 768);
            var bhavCont = new BHAVContainer(bhav, scope);
            mainCont.Add(bhavCont);
            GameFacade.Screens.AddExternal(mainCont);

            SetUI(mainCont);
        }
    }
}