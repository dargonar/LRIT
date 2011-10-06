using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;

namespace DataCenterQueues
{
  public class QueueManager
  {
    public static MessageQueue GetQueue(string name)
    {
      MessageQueue queue = null;
      if (MessageQueue.Exists(name) == false)
        queue = MessageQueue.Create(name, true);
      else
        queue = new MessageQueue(name);

      return queue;
    }

    public static void DropQueue(string name)
    {
        if (MessageQueue.Exists(name) == true)
        {
          MessageQueue.Delete(name);
        }
    }

  }
}
