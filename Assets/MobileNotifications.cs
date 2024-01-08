using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using Unity.VisualScripting;
using UnityEngine;

public class MobileNotifications : MonoBehaviour
{
    // This method is used to initialize the notification channel
    public void Start()
    {
        // Create the notification channel
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Notification Channel",
            Importance = Importance.Default,
            Description = "Reminder Notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    // Call this method to send a notification
    public void SendHungerNotification()
    {
        // Create the notification
        var notification = new AndroidNotification()
        {
            Title = "Your Buddy needs you!",
            Text = "Don't forget to feed your pet!",
            SmallIcon = "default",
            LargeIcon = "default",
           // FireTime = System.DateTime.Now.AddSeconds(5), // You might not need this delay
        };

        // Send the notification to the device
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

    public void SendThirstNotification()
    {
        // Create the notification
        var notification = new AndroidNotification()
        {
            Title = "Your Buddy needs you!",
            Text = "Give it some milk!",
            SmallIcon = "default",
            LargeIcon = "default",
            // FireTime = System.DateTime.Now.AddSeconds(5), // You might not need this delay
        };

        // Send the notification to the device
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

    public void SendCleanlinessNotification()
    {
        // Create the notification
        var notification = new AndroidNotification()
        {
            Title = "Your Buddy needs you!",
            Text = "Clean up after your pet!",
            SmallIcon = "default",
            LargeIcon = "default",
            // FireTime = System.DateTime.Now.AddSeconds(5), // You might not need this delay
        };

        // Send the notification to the device
        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }

    // This method might be called when the app loses focus to cancel all notifications
    public void CancelAllNotifications()
    {
        AndroidNotificationCenter.CancelAllNotifications();
    }
}
