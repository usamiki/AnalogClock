// Copyright 2021 Tukumo Tumugu
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TTukumo
{
    /// <summary>
    /// This script rotate 3 objects that represents clock hands.
    /// rotation is about Z axis.
    /// </summary>
    public class AnalogClock : MonoBehaviour
    {
        // Set here your clock parts.
        // Rotation Z = 0 is viewed as pointing 12 o'clock.
        public Transform SecondHand;
        public Transform MinuteHand;
        public Transform HourHand;

        public const float FREQUENCY_MAX = 60f;
        public const float FREQUENCY_MIN = 0.1f;

        /// <summary>
        /// Make the second hand animation continuous.
        /// </summary>
        public bool SmoothSecondHand;

        /// <summary>
        /// Make the minutes hand animation continuous.
        /// </summary>
        public bool SmoothMinuteHand;

        /// <summary>
        /// Make the hour hand animation continuous.
        /// </summary>
        public bool SmoothHourHand;

        /// <summary>
        /// This flag makes Power = true on OnEnable()
        /// </summary>
        public bool StartOnEnable;

        /// <summary>
        /// Display the time of System.DateTime.Now().
        /// If UseSystemTime is true, SecondsPerTick and TickFrequency are ignored.
        /// </summary>
        public bool UseSystemTime;

        TimeSpan _secondsParTick = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The number of seconds to advance the hands of the clock for each tick.
        /// </summary>
        public int SecondsPerTick
        {
            get => (int)_secondsParTick.TotalSeconds;
            set { _secondsParTick = TimeSpan.FromSeconds(value); }
        }

        float _tickFrequency = 1f;
        /// <summary>
        /// The number of Ticks for each real time second.
        /// </summary>
        public float TickFrequency
        {
            get => _tickFrequency;
            set
            {
                _tickFrequency = value < FREQUENCY_MIN ?
                    FREQUENCY_MIN : Math.Min(value, FREQUENCY_MAX);
            }
        }

        bool _power;
        /// <summary>
        /// Start/Stop the clock.
        /// </summary>
        public bool Power
        {
            get
            {
                return _power;
            }
            set
            {
                var old = _power;
                _power = value;
                if (!old && _power)
                {
                    StartCoroutine(StartTick());
                }
            }
        }

        private TimeSpan _currentTime;

        /// <summary>
        /// Set the internal TimeSpan. A part of the value that is greater than 24H has no meaning.
        /// Please do not set a value smaller than -24H.
        /// </summary>
        public TimeSpan CurrentTime
        {
            get => _currentTime; set
            {
                _currentTime = value;
                //Hack to avoid an negative TimeSpan. An negative TimeSpan is not displayed correctly.
                if (_currentTime.Ticks < 0)
                    _currentTime += TimeSpan.FromTicks(TimeSpan.TicksPerDay);
            }
        }

        int Seconds => CurrentTime.Seconds;
        int Minutes => CurrentTime.Minutes;
        float Hours12 => CurrentTime.Hours % 12 + CurrentTime.Minutes / 60f;

        float DetailedSeconds => CurrentTime.Seconds + CurrentTime.Milliseconds / 1000f;
        float DetailedMinutes => CurrentTime.Minutes + CurrentTime.Seconds / 60f;
        float DetailedHours12 => CurrentTime.Hours % 12 + CurrentTime.Minutes / 60f + CurrentTime.Seconds / 3600f;

        void FixedUpdate()
        {
            Show();
        }

        long ticksForFixedDeltaTime;
        void Awake()
        {
            ticksForFixedDeltaTime = (long)(Time.fixedDeltaTime * TimeSpan.TicksPerSecond);
        }

        void OnEnable()
        {
            if (StartOnEnable)
            {
                Power = true;
            }
        }

        void OnDisable()
        {
            Power = false;
        }

        IEnumerator StartTick()
        {
            while (Power)
            {
                if (SmoothSecondHand)
                    yield return new WaitForFixedUpdate();
                else
                    yield return new WaitForSecondsRealtime(1f / TickFrequency);

                Tick();
            }
        }

        void Tick()
        {
            if (UseSystemTime)
                CurrentTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
            else if (SmoothSecondHand)
                CurrentTime += TimeSpan.FromTicks((long)(ticksForFixedDeltaTime * TickFrequency * SecondsPerTick));
            else
                CurrentTime += _secondsParTick;
        }

        void Show()
        {
            var s = SecondHand.transform.eulerAngles;
            var m = MinuteHand.transform.eulerAngles;
            var h = HourHand.transform.eulerAngles;
            var secAngle = (SmoothSecondHand ? DetailedSeconds : Seconds) * -6;
            var minAngle = (SmoothMinuteHand ? DetailedMinutes : Minutes) * -6;
            var hourAngle = (SmoothHourHand ? DetailedHours12 : Hours12) * -30;
            SecondHand.transform.eulerAngles = new Vector3(s.x, s.y, secAngle);
            MinuteHand.transform.eulerAngles = new Vector3(m.x, m.y, minAngle);
            HourHand.transform.eulerAngles = new Vector3(h.x, h.y, hourAngle);
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AnalogClock))]
    public class ClockEditor : Editor
    {
        bool _negate;

        public override void OnInspectorGUI()
        {
            var tar = (AnalogClock)target;

            var power = EditorGUILayout.Toggle("Power", tar.Power);
            if (power != tar.Power)
            {
                tar.Power = power;
            }

            DrawDefaultInspector();

            tar.TickFrequency = EditorGUILayout.Slider("Tick Frequency", tar.TickFrequency, AnalogClock.FREQUENCY_MIN, AnalogClock.FREQUENCY_MAX);
            var abs = (int)EditorGUILayout.Slider("Seconds Per Tick (Abs)", Math.Abs(tar.SecondsPerTick), 0, 1000);
            _negate = EditorGUILayout.Toggle("NegateTick", _negate);
            tar.SecondsPerTick = _negate ? -abs : abs;
        }
    }
#endif

}
