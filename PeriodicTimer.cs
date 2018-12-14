﻿using System;
using System.Collections.Generic;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SensorExplorer
{
    static class PeriodicTimer
    {
        public static List<SensorData> sensorData = null;
        public static List<SensorDisplay> sensorDisplay = null;
        private static CoreDispatcher _cd = Window.Current.CoreWindow.Dispatcher;
        private static ThreadPoolTimer _periodicTimer = null;

        /// <summary>
        /// Create a periodic timer that fires every time the period elapses.
        /// When the timer expires, its callback handler is called and the timer is reset.
        /// This behavior continues until the periodic timer is cancelled.
        /// </summary>
        public static void Create(int index)
        {
            sensorData[index].ClearReading();
            if (_periodicTimer == null)
            {
                _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), new TimeSpan(0, 0, 1));
            }
        }

        public static void Cancel()
        {
            bool allOff = true;
            for (int i = 0; i < sensorDisplay.Count; i++)
            {
                if (sensorDisplay[i]._isOn)
                {
                    allOff = false;
                    break;
                }
            }

            if (allOff && _periodicTimer != null)
            {
                _periodicTimer.Cancel();
                _periodicTimer = null;
            }
        }

        private async static void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            await _cd.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                for (int i = 0; i < sensorDisplay.Count; i++)
                {
                    if (sensorDisplay[i].StackPanelSensor.Visibility == Visibility.Visible)
                    {
                        sensorDisplay[i]._plotCanvas.Plot(sensorData[i]);
                        sensorDisplay[i].UpdateText(sensorData[i]);
                    }

                    // Update report interval
                    if (sensorData[i]._reportIntervalChanged)
                    {
                        try
                        {
                            if (sensorData[i]._sensorType == Sensor.ACCELEROMETER)
                            {
                                Sensor.AccelerometerStandardList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.AccelerometerStandardList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.ACCELEROMETERLINEAR)
                            {
                                Sensor.AccelerometerLinearList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.AccelerometerLinearList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.ACCELEROMETERGRAVITY)
                            {
                                Sensor.AccelerometerGravityList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.AccelerometerGravityList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.COMPASS)
                            {
                                Sensor.CompassList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.CompassList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.GYROMETER)
                            {
                                Sensor.GyrometerList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.GyrometerList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.INCLINOMETER)
                            {
                                Sensor.InclinometerList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.InclinometerList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.LIGHTSENSOR)
                            {
                                Sensor.LightSensorList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.LightSensorList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.ORIENTATIONSENSOR)
                            {
                                Sensor.OrientationAbsoluteList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.OrientationAbsoluteList[sensorData[i]._count].ReportInterval;
                            }
                            else if (sensorData[i]._sensorType == Sensor.ORIENTATIONRELATIVE)
                            {
                                Sensor.OrientationRelativeList[sensorData[i]._count].ReportInterval = sensorData[i]._reportInterval;
                                sensorData[i]._reportInterval = Sensor.OrientationRelativeList[sensorData[i]._count].ReportInterval;
                            }

                            sensorData[i]._reportIntervalChanged = false;
                        }
                        catch { }

                        // Update UI
                        sensorDisplay[i].UpdateProperty(sensorData[i]._deviceId, sensorData[i]._deviceName, sensorData[i]._reportInterval, sensorData[i]._minReportInterval, sensorData[i]._reportLatency,
                                                        sensorData[i]._category, sensorData[i]._persistentUniqueId, sensorData[i]._manufacturer, sensorData[i]._model, sensorData[i]._connectionType);
                    }
                }
            });
        }
    }
}