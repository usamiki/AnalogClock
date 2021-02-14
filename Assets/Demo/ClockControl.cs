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
using UnityEngine;
using UnityEngine.UI;

namespace TTukumo
{
    /// <summary>
    /// Demo UI Controller
    /// </summary>
    public class ClockControl : MonoBehaviour
    {
        public AnalogClock Target;

        public Slider FrequencyControl;
        public Text FrequencyText;

        public Slider SecondsPerTickControl;
        public Text SecondsPerTickText;

        public Toggle PowerToggle;

        public Toggle SystemTimeUseToggle;

        public Toggle SmoothSecondHandToggle;
        public Toggle SmoothMinuteHandToggle;
        public Toggle SmoothHourHandToggle;

        public InputField Adjuster;


        void OnEnable()
        {
            FrequencyControl.value = Target.TickFrequency;
            FrequencyControl.onValueChanged.AddListener(v => {
                Target.TickFrequency = Mathf.Pow(2, v);
            });

            SecondsPerTickText.text = Target.SecondsPerTick.ToString();
            SecondsPerTickControl.onValueChanged.AddListener(v => {
                Target.SecondsPerTick = (int)v;
            });

            PowerToggle.onValueChanged.AddListener(v => Target.Power = v);
            SystemTimeUseToggle.onValueChanged.AddListener(v => Target.UseSystemTime = v);
            SmoothSecondHandToggle.onValueChanged.AddListener(v => Target.SmoothSecondHand = v);
            SmoothMinuteHandToggle.onValueChanged.AddListener(v => Target.SmoothMinuteHand = v);
            SmoothHourHandToggle.onValueChanged.AddListener(v => Target.SmoothHourHand = v);

            Adjuster.onEndEdit.AddListener(v => {
                try
                {
                    var time = TimeSpan.Parse(v);
                    Target.CurrentTime = time;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            });
        }

        private void Update()
        {
            PowerToggle.isOn = Target.Power;
            SystemTimeUseToggle.isOn = Target.UseSystemTime;
            SmoothSecondHandToggle.isOn = Target.SmoothSecondHand;
            SmoothMinuteHandToggle.isOn = Target.SmoothMinuteHand;
            SmoothHourHandToggle.isOn = Target.SmoothHourHand;
            FrequencyText.text = Target.TickFrequency.ToString();
            FrequencyText.text = Target.TickFrequency.ToString();
            SecondsPerTickControl.value = Target.SecondsPerTick;
            SecondsPerTickText.text = Target.SecondsPerTick.ToString();
        }

        private void OnDisable()
        {
            FrequencyControl.onValueChanged.RemoveAllListeners();
            SecondsPerTickControl.onValueChanged.RemoveAllListeners();
            PowerToggle.onValueChanged.RemoveAllListeners();
            SystemTimeUseToggle.onValueChanged.RemoveAllListeners();
            SmoothSecondHandToggle.onValueChanged.RemoveAllListeners();
            SmoothMinuteHandToggle.onValueChanged.RemoveAllListeners();
            SmoothHourHandToggle.onValueChanged.RemoveAllListeners();
            Adjuster.onEndEdit.RemoveAllListeners();
        }

    }

}

