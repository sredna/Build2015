﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace ContosoShop
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OrderPlacedPage : Page
    {
        public OrderPlacedPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var paymentResult = e.Parameter as ValueSet;
            OrderPlacedMessage.Text = string.Format("Congratulations! You just purchased {0}", paymentResult["ProductName"] as string);            

            //Save this order
            OrderDetails order = new OrderDetails();
            order.ItemName = paymentResult["ProductName"] as string;

            order.OrderPlacedTime = DateTime.UtcNow;

            order.OrderID = GenerateOrderId();

            order.TrackingNumber = GenerateTrackingNumber();

            await this.SaveOrder(order);
        }
       

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        // Serialize the order details
        private async Task SaveOrder(OrderDetails details)
        {
            StorageFile userdetailsfile = await ApplicationData.Current.LocalFolder.CreateFileAsync("OrderDetails", CreationCollisionOption.ReplaceExisting);
            IRandomAccessStream raStream = await userdetailsfile.OpenAsync(FileAccessMode.ReadWrite);
            using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
            {
                // Serialize the Session State.
                DataContractSerializer serializer = new DataContractSerializer(typeof(OrderDetails));
                serializer.WriteObject(outStream.AsStreamForWrite(), details);
                await outStream.FlushAsync();
            }
        }

        //Generate a fake tracking number
        private string GenerateTrackingNumber()
        {            
            Random r = new Random();
            return "1ZE1F022020745" + r.Next(DateTime.UtcNow.Millisecond).ToString();
        }

        // Generate a fake order id
        private string GenerateOrderId()
        {
            Random r = new Random();
            return "002-5368307-" + r.Next(DateTime.UtcNow.Millisecond).ToString();
        }

    }
}
