﻿using FSO.Client.UI.Framework;
using FSO.SimAntics.NetPlay.EODs.Handlers;
using FSO.SimAntics.NetPlay.Model.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSO.Client.UI.Panels.EODs
{
    public class UIEODController : UIContainer
    {
        public static Dictionary<uint, Type> IDToHandler = new Dictionary<uint, Type>()
        {
            { 0x2a6356a0, typeof(UISignsEOD) },
            { 0x4a5be8ab, typeof(UIDanceFloorEOD) }
        };

        //this class is a container so that it can hold EODs without them being active in Live Mode.
        public UILotControl Lot;
        public EODLiveModeOpt DisplayMode;
        public UIEOD ActiveEOD;
        public uint ActivePID;

        public UIEODController(UILotControl lot)
        {
            Lot = lot;
        }

        public void OnEODMessage(VMNetEODMessageCmd cmd)
        {
            switch (cmd.EventName)
            {
                case "eod_enter":
                    //attempt to create the EOD UI for the given plugin (live mode will detect this)
                    if (cmd.Binary) return; //???
                    Type handlerType = null;
                    if (IDToHandler.TryGetValue(cmd.PluginID, out handlerType))
                    {
                        ActiveEOD = (UIEOD)Activator.CreateInstance(handlerType, this);
                        ActivePID = cmd.PluginID;
                        Add(ActiveEOD);
                    }
                    break;
                case "eod_leave":
                    if (cmd.Binary || ActiveEOD == null) return; //???
                    DisplayMode = null;
                    Remove(ActiveEOD);
                    ActivePID = 0;
                    ActiveEOD = null;
                    break;
                default:
                    //forward to existing ui
                    if (ActiveEOD == null) return; //uh... what UI?
                    if (cmd.Binary)
                    {
                        EODDirectBinaryEventHandler handle = null;
                        if (ActiveEOD.BinaryHandlers.TryGetValue(cmd.EventName, out handle))
                        {
                            handle(cmd.EventName, cmd.BinData);
                        }
                    } else
                    {
                        EODDirectPlaintextEventHandler handle = null;
                        if (ActiveEOD.PlaintextHandlers.TryGetValue(cmd.EventName, out handle))
                        {
                            handle(cmd.EventName, cmd.TextData);
                        }
                    }
                    break;
            }
        }

        public void ShowEODMode(EODLiveModeOpt mode)
        {
            DisplayMode = mode; //gets picked up by live mode
        }

        public void CloseEOD()
        {
            if (ActiveEOD == null) return; //???
            ShowEODMode(null);
            Remove(ActiveEOD);
            ActiveEOD = null;
            ActivePID = 0;
        }
    }
}
