using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;
using Base;
using Base.Interfaces;
using Base.Libraries;
using Imp.Controllers;

namespace ImpControls.Controllers
{
    public class EventController : IEventController
    {
        private readonly Dispatcher dispatcher;
        private readonly Label eventLabel;
        private readonly Label titleLabel;
        private readonly object eventLock = new object();

        private readonly DispatcherTimer eventTimer;
        private readonly Queue<EventText> events;

        
        private bool LastEventInterrupted;
        private string TitleText = "";
        private EventText lastEvent;


        public EventController(Dispatcher dispatcher, Label eventLabel, Label titleLabel)
        {
            this.dispatcher = dispatcher;
            this.eventLabel = eventLabel;
            this.titleLabel = titleLabel;

            events = new Queue<EventText>(4);
            eventTimer = new DispatcherTimer(DispatcherPriority.Normal, dispatcher);
            eventTimer.Tick += EventTimer_Elapsed;
            SetTitleText();
            
        }


        public void ShowError(ImpError error)
        {
            SetEvent(new EventText("Error " + (int) error.Type + ": " + error.Text, 2, EventType.Delayed));
        }


        public void SetEvent(EventText eventText)
        {
            lock (eventLock)
            {
                if (eventText.EventType == EventType.Instant)
                {
                    LastEventInterrupted = true;
                    eventTimer.IsEnabled = false;
                    eventTimer.Interval = new TimeSpan((long) (eventText.Duration * LibImp.SecondToTicks));
                    SetEventText(eventText);
                    eventTimer.IsEnabled = true;
                }
                else
                {
                    events.Enqueue(eventText);
                    eventTimer.Interval = new TimeSpan(1);
                    if (!eventTimer.IsEnabled)
                    {
                        ShowNextEvent();
                    }
                }
            }
        }


        public void SetTitle(string title)
        {
            TitleText = title;
            if (!eventTimer.IsEnabled)
            {
                SetTitleText();
            }
        }


        private void SetEventText(EventText eventText)
        {
            if (EnsureMainThread())
            {
                dispatcher.Invoke(() => SetEventText(eventText));
                return;
            }

            titleLabel.Content = eventText.Text;
            eventLabel.Content = eventText.Text;
        }


        private bool EnsureMainThread()
        {
            return Thread.CurrentThread != dispatcher.Thread;
        }


        private void EventTimer_Elapsed(object sender, EventArgs eventArgs)
        {
            eventTimer.IsEnabled = false;

            if (LastEventInterrupted)
            {
                SetEventText(lastEvent);
                lastEvent.Duration *= 0.6; // reduce duration due to interruption
                eventTimer.Interval = new TimeSpan((long) (lastEvent.Duration * LibImp.SecondToTicks));
                eventTimer.IsEnabled = true;
            }
            else if (events.Count > 0)
            {
                ShowNextEvent();
            }
            else
            {
                SetTitleText();
            }
            LastEventInterrupted = false;
        }


        private void ShowNextEvent()
        {
            lastEvent = events.Dequeue();
            lastEvent.Duration *= 2d / Math.Max(2, events.Count);
            SetEventText(lastEvent);
            eventTimer.Interval = new TimeSpan((long) (lastEvent.Duration * LibImp.SecondToTicks));
            eventTimer.IsEnabled = true;
        }


        private void SetTitleText()
        {
            titleLabel.Content = TitleText;
            eventLabel.Content = "";
        }
    }
}