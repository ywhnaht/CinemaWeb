using CinemaWeb.SupportFile;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

public static class QuartzConfig
{
    public static async Task ConfigureQuartz()
    {
        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        // Tự động thêm thông báo nếu sắp tới suất chiếu
        IJobDetail notificationJob = JobBuilder.Create<NotificationJob>()
            .WithIdentity("notificationJob", "group1")
            .Build();

        ITrigger notificationTrigger = TriggerBuilder.Create()
            .WithIdentity("notificationTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(1)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(notificationJob, notificationTrigger);

        // Tự động xóa thông báo đã đọc
        IJobDetail deleteReadNotificationsJob = JobBuilder.Create<DeleteNotificationJob>()
            .WithIdentity("deleteNotificationsJob", "group1")
            .Build();

        ITrigger deleteReadNotificationsTrigger = TriggerBuilder.Create()
            .WithIdentity("deleteNotificationsTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(24)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(deleteReadNotificationsJob, deleteReadNotificationsTrigger);

        IJobDetail releaseJob = JobBuilder.Create<ChangeSeatStatusJob>()
            .WithIdentity("changeSeatStatusJob", "group1")
            .Build();

        ITrigger releaseTrigger = TriggerBuilder.Create()
            .WithIdentity("changeSeatStatusTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(5)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(releaseJob, releaseTrigger);

        IJobDetail removeSeatStatusJob = JobBuilder.Create<RemoveSeatStatusJob>()
            .WithIdentity("removeSeatStatusJob", "group1")
            .Build();

        ITrigger removeSeatStatusTrigger = TriggerBuilder.Create()
            .WithIdentity("removeSeatStatusTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(5)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(removeSeatStatusJob, removeSeatStatusTrigger);

        IJobDetail removeInvoiceJob = JobBuilder.Create<RemoveInvoiceJob>()
            .WithIdentity("removeInvoiceJob", "group1")
            .Build();

        ITrigger removeInvoiceTrigger = TriggerBuilder.Create()
            .WithIdentity("removeInvoiceTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(5)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(removeInvoiceJob, removeInvoiceTrigger);

        IJobDetail setInvoiceUsedJob = JobBuilder.Create<SetInvoiceUsed>()
            .WithIdentity("setInvoiceUsedJob", "group1")
            .Build();

        ITrigger setInvoiceUsedTrigger = TriggerBuilder.Create()
            .WithIdentity("setInvoiceUsedTrigger", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(5)
                .RepeatForever())
            .Build();

        await scheduler.ScheduleJob(setInvoiceUsedJob, setInvoiceUsedTrigger);
    }
}
