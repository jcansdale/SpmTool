namespace SpmTool
{
    static class Dx8ToDx9ValueMapper
    {
        public static string BuildHeliFMode(string switchA, string switchB)
        {
            switchA = switchA?.Trim();
            switchB = switchB?.Trim();

            return
                "switch_a= " + switchA + "\n" +
                "switch_b= 0\n" +
                "switch_c= " + switchB + "\n" +
                "size= 18\n" +
                "fmtable: 1 1 1 1 1 1 2 2 2 2 2 2 3 4 4 3 4 4\n" +
                "activePositions=%0006\n";
        }

        public static string MapServoVSource(string servoName, string servoIndex, string generator, string tail, string wing)
        {
            servoName = servoName?.Trim();
            servoIndex = servoIndex?.Trim();
            generator = generator?.Trim();
            tail = tail?.Trim();
            wing = wing?.Trim();

            if (servoName == "LEL" && IsDualEleTail(tail)) return "8";
            if (servoName == "LRU" && generator == "DX7S" && tail == "Dual_Rud") return "7";
            if (servoName == "MOT" && IsSailFlapWing(wing)) return "6";
            if (servoName == "LAL" && IsSailFlapWing(wing)) return "0";
            if (servoName == "RFL" && wing == "Ail_2_Flap_2") return "4";
            if (servoName == "LFL" && wing == "Ail_2_Flap_2") return "5";

            return servoIndex;
        }

        public static string MapServoDirection(string direction, string servoName, bool isSail)
        {
            direction = direction?.Trim();
            servoName = servoName?.Trim();

            if (isSail && servoName == "RFL")
            {
                switch (direction)
                {
                    case "Normal": return "Reverse";
                    case "Reverse": return "Normal";
                    default: return "UNKNOWN_" + direction;
                }
            }

            return direction;
        }

        public static string MapTrimType(string trimType)
        {
            trimType = trimType?.Trim();
            return trimType == "FMode" ? "%0000003F" : "%00000000";
        }

        public static string MapWarningFltMode(string fltMode, bool isSail)
        {
            fltMode = fltMode?.Trim();

            if (isSail)
            {
                return fltMode;
            }

            switch (fltMode)
            {
                case "%0000": return "%0000";
                case "%0020": return "%0004";
                case "%0040": return "%0008";
                case "%0060": return "%000C";
                case "%0080": return "%0000";
                case "%00A0": return "%0004";
                case "%00C0": return "%0008";
                case "%00E0": return "%000C";
                default: return "UNKNOWN_" + fltMode;
            }
        }

        public static string MapWarningHold(string fltMode)
        {
            fltMode = fltMode?.Trim();

            switch (fltMode)
            {
                case "%0000":
                case "%0020":
                case "%0040":
                case "%0060":
                    return "%0000";
                case "%0080":
                case "%00A0":
                case "%00C0":
                case "%00E0":
                    return "%0002";
                default:
                    return "UNKNOWN_" + fltMode;
            }
        }

        public static string MapWarningMotor(string motor, string sailMotor, string activePositions)
        {
            motor = motor?.Trim();
            sailMotor = sailMotor?.Trim();
            activePositions = activePositions?.Trim();

            switch (motor)
            {
                case "%0000":
                    return "%0000";
                case "%0001" when sailMotor == "SpoilStk":
                    return "%0000";
                case "%0001":
                    return activePositions;
                default:
                    return "UNKNOWN_" + motor;
            }
        }

        public static string MapWarningFlaps(string flaps)
        {
            flaps = flaps?.Trim();

            switch (flaps)
            {
                case "%0000": return "%0000";
                case "%0001": return "%0002";
                case "%0002": return "%0004";
                case "%0003": return "%0006";
                case "%0004": return "%0005";
                default: return "UNKNOWN_" + flaps;
            }
        }

        public static string MapSailSubTypeC(string motor, string subTypeC)
        {
            motor = motor?.Trim();
            subTypeC = subTypeC?.Trim();

            switch (motor)
            {
                case "None": return "64";
                case "SpoilStk": return "64";
                case "Gear": return "82";
                case "FModeSw": return "83";
                case "EleDR": return "84";
                case "Flap": return "85";
                case "Aux2": return "86";
                case "AilDR": return "87";
                case "RudDR": return "88";
                case "Mix": return "89";
                case "Trainer": return "92";
                case "Unsupported":
                    switch (subTypeC)
                    {
                        case "68": return "112";
                        case "69": return "113";
                        default: return "UNKNOWN_" + motor;
                    }
                default:
                    return "UNKNOWN_" + motor;
            }
        }

        public static string MapValue(string value, string generator, string tail, string wing)
        {
            value = value?.Trim();
            generator = generator?.Trim();
            tail = tail?.Trim();
            wing = wing?.Trim();

            switch (value)
            {
                case "0": return "0";
                case "1": return "1";
                case "2": return "2";
                case "3": return "3";
                case "4": return "4";
                case "5": return "5";
                case "6": return "6";
                case "7":
                    if (IsDualEleTail(tail)) return "9";
                    if (generator == "DX7S" && tail == "Dual_Rud") return "8";
                    if (wing == "Ail_2_Flap_2") return "79";
                    if (wing == "Ail_2_Flap_1") return "AX2_NOT_AVAILABLE";
                    return "7";
                case "8": return "8";
                case "16":
                    if (IsSailFlapWing(wing)) return "7";
                    return "64";
                case "17": return "65";
                case "18": return "66";
                case "19": return "67";
                case "20": return "68";
                case "21": return "69";
                case "32": return "78";
                case "33": return "79";
                case "40": return "82";
                case "41": return "83";
                case "42": return "84";
                case "43": return "85";
                case "44": return "86";
                case "45": return "87";
                case "46": return "88";
                case "47": return "89";
                case "50": return "92";
                case "63": return "107";
                case "64": return "108";
                case "65": return "109";
                case "66": return "110";
                case "67": return "111";
                case "68": return "112";
                case "69": return "113";
                case "70": return "0";
                case "75": return "0";
                case "95": return "127";
                case "96": return "128";
                case "97": return "129";
                case "98": return "130";
                case "127": return "145";
                case "192":
                    if (IsSailFlapWing(wing)) return "38";
                    return "32";
                case "193": return "33";
                case "194": return "34";
                case "195": return "35";
                case "196":
                    if (wing == "Ail_2_Flap_2") return "37";
                    return "36";
                case "197":
                    if (IsSailFlapWing(wing)) return "32";
                    return "37";
                case "198":
                    if (IsDualEleTail(tail)) return "40";
                    if (generator == "DX7S" && tail == "Dual_Rud") return "39";
                    if (wing == "Ail_2_Flap_2") return "36";
                    return "38";
                case "199": return "39";
                case "200": return "52";
                case "238": return "34";
                case "239": return "35";
                case "242": return "0";
                case "244": return "200";
                case "245": return "201";
                default:
                    return "UNKNOWN_" + value;
            }
        }

        static bool IsDualEleTail(string tail)
        {
            return tail == "Dual_Rud_Ele" || tail == "Dual_Ele";
        }

        static bool IsSailFlapWing(string wing)
        {
            return wing == "Ail_2_Flap_1" || wing == "Ail_2_Flap_2";
        }
    }

    public class Dx8ToDx9XsltExtensions
    {
        public string MapValue(string value, string generator, string tail, string wing)
        {
            return Dx8ToDx9ValueMapper.MapValue(value, generator, tail, wing);
        }

        public string MapSailSubTypeC(string motor, string subTypeC)
        {
            return Dx8ToDx9ValueMapper.MapSailSubTypeC(motor, subTypeC);
        }

        public string MapWarningFltMode(string fltMode, bool isSail)
        {
            return Dx8ToDx9ValueMapper.MapWarningFltMode(fltMode, isSail);
        }

        public string MapWarningHold(string fltMode)
        {
            return Dx8ToDx9ValueMapper.MapWarningHold(fltMode);
        }

        public string MapWarningMotor(string motor, string sailMotor, string activePositions)
        {
            return Dx8ToDx9ValueMapper.MapWarningMotor(motor, sailMotor, activePositions);
        }

        public string MapWarningFlaps(string flaps)
        {
            return Dx8ToDx9ValueMapper.MapWarningFlaps(flaps);
        }

        public string MapServoDirection(string direction, string servoName, bool isSail)
        {
            return Dx8ToDx9ValueMapper.MapServoDirection(direction, servoName, isSail);
        }

        public string MapTrimType(string trimType)
        {
            return Dx8ToDx9ValueMapper.MapTrimType(trimType);
        }

        public string MapServoVSource(string servoName, string servoIndex, string generator, string tail, string wing)
        {
            return Dx8ToDx9ValueMapper.MapServoVSource(servoName, servoIndex, generator, tail, wing);
        }

        public string BuildHeliFMode(string switchA, string switchB)
        {
            return Dx8ToDx9ValueMapper.BuildHeliFMode(switchA, switchB);
        }
    }
}
