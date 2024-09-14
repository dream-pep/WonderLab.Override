﻿using System;
using System.Threading;
using Avalonia.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using WonderLab.Classes.Interfaces;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace WonderLab.Services;

public sealed partial class TaskService(IBackgroundTaskQueue queue, ILogger<TaskService> logger) : ObservableObject {
    private readonly ILogger<TaskService> _logger = logger;
    private readonly IBackgroundTaskQueue _taskQueue = queue;

    [ObservableProperty] private ObservableCollection<ITaskJob> taskJobs = [];

    public void QueueJob(ITaskJob job) {
        if (job is null) {
            return;
        }

        _ = Task.Run(async () => {
            await _taskQueue.QueueBackgroundWorkItemAsync(job);
            job.TaskFinished += (_, args) => {
                using (job) {
                    if (TaskJobs.Remove(job)) {
                        _logger.LogInformation("任务已被移出队列！");
                    }
                }
            };

            await Dispatcher.UIThread.InvokeAsync(() => {
                TaskJobs.Add(job);
            });
        });
    }
}

public sealed class BackgroundTaskQueue : IBackgroundTaskQueue {
    private readonly Channel<ITaskJob> _queue;

    public BackgroundTaskQueue(int queueLength) {
        BoundedChannelOptions boundedChannelOptions = new(queueLength) {
            FullMode = BoundedChannelFullMode.Wait
        };

        _queue = Channel.CreateBounded<ITaskJob>(boundedChannelOptions);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(ITaskJob job) {
        if (job == null) {
            ArgumentNullException.ThrowIfNull(job);
        }

        await _queue.Writer.WriteAsync(job);
    }

    public async ValueTask<ITaskJob> DequeueAsync(CancellationToken cancellationToken) {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}