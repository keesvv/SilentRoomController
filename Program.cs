﻿using System;
using static SilentRoomControllerv2.Program;

namespace SilentRoomControllerv2
{
    public class Program
    {
         /*
         *                                             *
         *    -=+ [ SilentRoomController 2.0 ] +=-     *
         *        Made by Kees van Voorthuizen.        *
         *         Credits to Philips (c) Hue.         *
         *                                             *
         */
        
        public static string[] Args { get; set; }
        public static string BridgeIP { get; set; }
        public static string APIKey { get; set; }
        public static int LightID { get; set; }
        public static HueCommand Command { get; set; }
        public static string CommandArguments { get; set; }

        public static string BaseURI
        {
            get
            {
                return string.Format("http://{0}/api/{1}/", BridgeIP, APIKey);
            }
        }
        public static string APIUri
        {
            get
            {
                return string.Format("http://{0}/api", BridgeIP);
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                // Stores all arguments into a public variable.
                Args = args;
            }
            catch (Exception) { }

            // Checks if a previous setup was successful.
            Utilities.CheckSetup();
        }

        public static void ParseArgs()
        {

            // Indicates if anything is left blank.
            bool isNull = false;

            // Parses the arguments and store them into variables.
            #region Command-Line Arguments Parser
            try
            {
                if (Args[0] == "-id")
                    LightID = int.Parse(Args[1]);
                else isNull = true;

                if (Args[2] == "-command")
                    Command = new HueCommand((Commands)int.Parse(Args[3]));
                else isNull = true;

                try
                {
                    if (Args[4] != null)
                        CommandArguments = Args[4];
                }
                catch (Exception) { }
            }
            catch (Exception) { Utilities.PrintUsage(); }

            #endregion

            // Checks if the anything is left blank.
            try
            {
                if (isNull != true)
                    Command.Execute(BridgeIP, APIKey, LightID, CommandArguments);
                else Utilities.PrintUsage();
            }
            catch (Exception) { }
        }
    }

    public class HueCommand
    {
        public Commands SetCommand { get; set; }
        public HueCommand(Commands command)
        {
            SetCommand = command;
        }

        public void Execute(string ipAddress, string apiKey, int lightID, string arguments = null)
        {
            string command = "";
            string targetURI = BaseURI + "lights/" + lightID + "/state";

            switch (SetCommand)
            {
                case Commands.COMMAND_ON:
                    command = "{\"on\":true}";
                    break;
                case Commands.COMMAND_OFF:
                    command = "{\"on\":false}";
                    break;
                case Commands.COMMAND_TOGGLE:
                    Utilities.ToggleLight(ipAddress, apiKey, lightID);
                    break;
                case Commands.COMMAND_SET_BRIGHTNESS:
                    command = "{\"bri\":" + arguments + "}";
                    break;
                case Commands.COMMAND_SET_HUE:
                    command = "{\"hue\":" + arguments + "}";
                    break;
                default:
                    break;
            }

            Utilities.SendPUTRequest(targetURI, command);
        }
    }

    public enum Commands
    {
        COMMAND_ON = 1,
        COMMAND_OFF = 2,
        COMMAND_TOGGLE = 3,
        COMMAND_SET_BRIGHTNESS = 4,
        COMMAND_SET_HUE = 5
    }
}