﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace ShipTrackService
{
    public sealed class TrackingService : IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;        
        AppServiceConnection connection = null;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //Take a service deferral so the service isn't terminated
            this.serviceDeferral = taskInstance.GetDeferral();

            taskInstance.Canceled += OnTaskCanceled;
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            connection = details.AppServiceConnection;

            //Listen for incoming app service requests
            connection.RequestReceived += OnRequestReceived;
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (this.serviceDeferral != null)
            {
                //Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }

        async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            //Get a deferral so we can use an awaitable API to respond to the message
            var messageDeferral = args.GetDeferral();

            var messageRequest = args.Request.Message;

            string trackingNumber = messageRequest["TrackingNumber"] as string;
                        
            string status = FetchTrackingStatus(trackingNumber);

            //Create the response
            var result = new ValueSet();
            result.Add("Status", status);

            //Send the response
            await args.Request.SendResponseAsync(result);

            //Complete the message deferral so the platform knows we're done responding
            messageDeferral.Complete();
        }

        private string FetchTrackingStatus(string trackingNumber)
        {
            return "Processing Order";
        }
    }
}
