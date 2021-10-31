using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Helper
{
    private static readonly StringFast _StringFast = new StringFast ();

    #if UNITY_EDITOR

    [RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadScene ()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name != SceneEnums.GetSceneString (SceneEnums.SceneID.SceneInit))
        {
            UnityEditor.EditorUtility.DisplayDialog ("Warning", string.Format ("Please running the game from the scene with name: {0}", SceneEnums.GetSceneString (SceneEnums.SceneID.SceneInit)), "OK");
        }

        LogGame.Log ("[Helper] Check the status of scene!");
    }

    #endif

    public static string ConvertToTime (double time)
    {
        _StringFast.Clear ();

        double seconds = time % 60;
        double minutes = (time - seconds) / 60 % 60;
        double hours   = (time - seconds - minutes * 60) / 3600;

        // TODO: Clear the old cache.
        _StringFast.Clear ();

        if (hours < 10)
        {
            _StringFast.Append ("0").Append (hours).Append (":");
        }
        else
        {
            _StringFast.Append (hours).Append (":");
        }

        if (minutes < 10)
        {
            // TODO: append the string.
            _StringFast.Append ("0").Append (minutes).Append (":");
        }
        else
        {
            // TODO: append the string.
            _StringFast.Append (minutes).Append (":");
        }

        if (seconds < 10)
        {
            // TODO: append the string.
            _StringFast.Append ("0").Append (seconds);
        }
        else
        {
            // TODO: append the string.
            _StringFast.Append (seconds);
        }

        return _StringFast.ToString ();
    }

    public static string ConvertToTime (int time)
    {
        _StringFast.Clear ();

        // TODO: Get the total of seconds.
        int seconds = time % 60;

        // TODO: Get the total of minutes.
        int minutes = (time - seconds) / 60;

        // TODO: Clear the old cache.
        _StringFast.Clear ();

        if (minutes < 10)
        {
            // TODO: append the string.
            _StringFast.Append ("0").Append (minutes).Append (":");
        }
        else
        {
            // TODO: append the string.
            _StringFast.Append (minutes).Append (":");
        }

        if (seconds < 10)
        {
            // TODO: append the string.
            _StringFast.Append ("0").Append (seconds);
        }
        else
        {
            // TODO: append the string.
            _StringFast.Append (seconds);
        }

        return _StringFast.ToString ();
    }

    public static float FixNumber (ref float value, ref int unit)
    {
        while (value < 1000)
        {
            if (unit == 0)
            {
                break;
            }

            unit--;
            value *= 1000;
        }

        return value;
    }

    public static double FixNumber (ref double value, ref int unit)
    {
        while (value < 1000.0)
        {
            if (unit == 0)
            {
                break;
            }

            unit--;
            value *= 1000.0;
        }

        return value;
    }

    public static int FixUnit (ref double value, ref int unit)
    {
        while (value > 1000000.0)
        {
            value /= 1000.0;
            unit++;
        }

        return unit;
    }

    public static void AddValue (ref double value, ref int unit, double valueAdd, int unitAdd, double times)
    {
        if (unit > unitAdd)
        {
            value = value + valueAdd * Math.Pow (0.001, unit - unitAdd) * times;
        }
        else
        {
            value = valueAdd * times + value * Math.Pow (0.001, unitAdd - unit);
            unit  = unitAdd;
        }

        FixUnit (ref value, ref unit);
    }

    public static void AddValue (ref double value, ref int unit, double valueAdd, int unitAdd)
    {
        if (unit > unitAdd)
        {
            value = value + valueAdd * Math.Pow (0.001, unit - unitAdd);
        }
        else
        {
            value = valueAdd + value * Math.Pow (0.001, unitAdd - unit);
            unit  = unitAdd;
        }

        FixUnit (ref value, ref unit);
    }

    public static void AddPercents (ref double value, ref int unit, double percents, int percent_unit)
    {
        var percent = (value * percents) / 100;

        AddValue (ref value, ref unit, percent, percent_unit);
    }

    public static void MinusValue (double value, int unit, double valueMinus, int unitMinus, out double valueOut, out int unitOut)
    {
        if (unit < unitMinus)
        {
            LogGame.Log ("[Minus] Error minus.");

            value = 0;
            unit  = 0;
        }
        else
        {
            if (unit - unitMinus <= 5)
            {
                value = value - valueMinus * Math.Pow (0.001, unit - unitMinus);
            }
        }

        if (value < 0.0)
        {
            value = 0;
            unit  = 0;
        }

        FixNumber (ref value, ref unit);

        valueOut = value;
        unitOut  = unit;
    }

    public static double GetRealValue (double value, int unit)
    {
        if (unit == 0)
            return value;

        return value * unit;
    }

    public static void GetRealExchangeCoins (out double coins, out int unit, double exchange, int unitExchange, double _CoinExchange, int coinExchangeUnit)
    {
        coins = exchange * _CoinExchange;
        unit  = unitExchange + coinExchangeUnit;

        FixUnit (ref coins, ref unit);
    }

    public static bool IsEqualOrSmaller (double valueBigger, int unitBigger, double valueSmaller, int unitSmaller)
    {
        if (unitBigger > unitSmaller)
        {
            return true;
        }

        if (unitBigger < unitSmaller)
        {
            return false;
        }

        if (valueBigger > valueSmaller)
        {
            return true;
        }

        if (valueBigger < valueSmaller)
        {
            return false;
        }

        return true;
    }

    public static bool IsSmaller (double valueBigger, int unitBigger, double valueSmaller, int unitSmaller)
    {
        if (unitBigger > unitSmaller)
        {
            return false;
        }

        if (unitBigger < unitSmaller)
        {
            return true;
        }

        if (valueBigger > valueSmaller)
        {
            return false;
        }

        if (valueBigger < valueSmaller)
        {
            return true;
        }

        return false;
    }

    public static void GetTimesValue (double coins, int unit, double valueIn, int unitIn, out double valueOut, out int unitOut)
    {
        unitOut  = unit - unitIn;
        valueOut = coins / valueIn;

        FixNumber (ref valueOut, ref unitOut);
    }

    public static void GetCashWithTime (out double coins, out int unit, double coins_earn, int unit_earn, double time)
    {
        coins = coins_earn * time;
        unit = unit_earn;

        Helper.FixUnit (ref coins, ref unit);
    }

    public static DateTime GetUtcTime ()
    {
        return DateTime.UtcNow;
    }

    public static string GetUtcTimeString ()
    {
        return DateTime.UtcNow.ToString (CultureInfo.InvariantCulture);
    }

    public static string GetUtcTimeStringNonHours ()
    {
        return DateTime.UtcNow.ToString ("MM/dd/yyyy");
    }

    public static string UtcNowToString (DateTime utc)
    {
        return utc.ToString (CultureInfo.InvariantCulture);
    }

    public static string GetDefaultUTCTimeString ()
    {
        return "01/01/2000";
    }

    public static DateTime GetDefaultUTCTime ()
    {
        return DateTime.Parse ("01/01/2000");
    }
}