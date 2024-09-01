using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Channels;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using Avalonia.Threading;
using WonderLab.Classes.Datas.ViewData;
using System.Diagnostics;

namespace WonderLab.Services;

public sealed partial class NotificationService : ObservableObject {
    [ObservableProperty] private ObservableCollection<INotification> _notifications = [];

    public void QueueJob(INotification job) {
        if (job is null) {
            return;
        }

        _ = Task.Run(async () => {
            await Dispatcher.UIThread.InvokeAsync(() => {
                Notifications.Add(job);
            });

            (job as NotificationViewData).CloseButtonAction = async () => {
                job.IsCardOpen = true;
                await Task.Delay(TimeSpan.FromSeconds(0.35d)).ContinueWith(x => {
                    Notifications.Remove(job);
                });
            };

            await Task.Delay(4000);
            if (!job.IsCardOpen) {
                job.IsCardOpen = true;
                await Task.Delay(TimeSpan.FromSeconds(0.35d)).ContinueWith(x => {
                    Notifications.Remove(job);
                });
            }
        });
    }
}