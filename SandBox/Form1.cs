using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Messaging;
using System.Runtime.InteropServices;
using System.IO;

namespace SandBox
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    private void button1_Click(object sender, EventArgs e)
    {
      using( MessageQueue mq = GetMQ() )
      {
        using ( MessageQueueTransaction tr = new MessageQueueTransaction() )
        {
          var msg = new System.Messaging.Message( new Pepe(50,"hola") );
          msg.Recoverable = true;
          msg.Label = "indc";
          tr.Begin();

          mq.Send(msg, tr );
          tr.Commit();
        }
      }    
    }

    private MessageQueue GetMQ()
    {
      MessageQueue mq = null;
      if (MessageQueue.Exists(@".\private$\lacola") == false)
        mq = MessageQueue.Create(@".\private$\lacola", true);
      else
        mq = new MessageQueue(@".\private$\lacola");

      return mq;
    }

    private void button2_Click(object sender, EventArgs e)
    {
      using( MessageQueue mq = GetMQ() )
      {
      using( MessageQueueTransaction tr = new MessageQueueTransaction() )
      {
        
        tr.Begin();
        System.Messaging.Message msg = mq.Receive(TimeSpan.FromSeconds(10), tr);
        if( msg == null )
        {
          MessageBox.Show("Nada");
          return;
        }
        
        Pepe p = (Pepe)msg.Body;
        MessageBox.Show( p.ToString() );
        tr.Commit();

      }
      }

    }
  }

  public class Pepe
  {

      public Pepe()
    {
      a = 1;
      b = "2";
    }

    public Pepe( int ma, string mb )
    {
      a=ma;
      b=mb;
    }

    public override string ToString()
    {
      return string.Format("Pepe: {0}-{1}",a,b);
    }

    public int    a;
    public string b;
  }

  [ComVisible(true)]
  [Guid("C205148C-A24F-412e-8CED-71F8CB79BDF3")]
  public class Menem
  {
    public void ProcessMessage(string label, string body)
    {
      File.WriteAllText( "c:\\toma.queue.txt", body );
    }

  }

}
