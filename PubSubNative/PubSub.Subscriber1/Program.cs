﻿using System;
using System.Configuration;
using System.Threading.Tasks;
using PubSub.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;

namespace PubSub.Subscriber1;

class Program
{
    static void Main()
    {
        using var activator = new BuiltinHandlerActivator();
            
        activator.Register(() => new Handler());

        Configure.With(activator)
            .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
            .Transport(t => t.UseAzureServiceBus(GetConnectionString(), "subscriber1"))
            .Start();

        activator.Bus.Subscribe<StringMessage>().Wait();
        activator.Bus.Subscribe<DateTimeMessage>().Wait();
        activator.Bus.Subscribe<TimeSpanMessage>().Wait();

        Console.WriteLine("This is Subscriber 1");
        Console.WriteLine("Press ENTER to quit");
        Console.ReadLine();
        Console.WriteLine("Quitting...");
    }

    static string GetConnectionString()
    {
        return ConfigurationManager.ConnectionStrings["servicebus"]?.ConnectionString
               ?? throw new ConfigurationErrorsException(@"Could not find 'servicebus' connection string. 

Please provide a valid Azure Service Bus connection string, most likely by going to your service bus namespace in the Azure Portal
and retrieving either 'Primary (...)' or 'Secondary Connection String' from the 'Shared Access Policies' tab.

If you create another SAS connection string, you need to give it 'Manage' rights, because Rebus (by default) wants to help
you create all of the necessary entities (queues, topics, subscriptions).

You may also provide a less priviledges SAS signature, but then you would need to create the entities yourself and disable
Rebus' ability to automatically create these things.");
    }
}

class Handler : IHandleMessages<StringMessage>, IHandleMessages<DateTimeMessage>, IHandleMessages<TimeSpanMessage>
{
    public async Task Handle(StringMessage message)
    {
        Console.WriteLine("Got string: {0}", message.Text);
    }

    public async Task Handle(DateTimeMessage message)
    {
        Console.WriteLine("Got DateTime: {0}", message.DateTime);
    }

    public async Task Handle(TimeSpanMessage message)
    {
        Console.WriteLine("Got TimeSpan: {0}", message.TimeSpan);
    }
}